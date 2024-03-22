using Sandbox.Diagnostics;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Bydrive;

[Category( "Vehicle" )]
public sealed class VehicleWheel : Component
{
	private const float DEFAULT_SIZE = 14f;
	private const float DEFAULT_WIDTH = 8f;
	[Property] public VehicleController Vehicle { get; set; }
	[Property] public ModelRenderer Renderer { get; set; }
	[Property] public float Radius { get; set; } = DEFAULT_SIZE;
	[Property] public float Width { get; set; } = DEFAULT_WIDTH;
	[Property] public bool IsDriving { get; set; } = true;
	[Property] public bool IsTurning { get; set; } = true;
	public Vector3 InitialPosition { get; private set; }
	public Rotation InitialRotation { get; private set; }
	protected override void OnEnabled()
	{
		InitialPosition = Transform.LocalPosition;
		InitialRotation = Transform.LocalRotation;
	}
	public Vector3 GetAttachmentWorldPosition()
	{
		return GameObject.Parent.Transform.World.PointToWorld( InitialPosition);
	}

	public bool Raycast( bool doPhysics, float dt )
	{
		if ( Vehicle == null ) return false;

		Rigidbody physicsComponent = Vehicle.Rigidbody;

		float downExtend = Radius * 1.2f;

		PhysicsBody physics = physicsComponent.PhysicsBody;
		Vector3 wheelAttachPosition = GetAttachmentWorldPosition();
		Vector3 wheelExtend = wheelAttachPosition + Vector3.Down * (downExtend + 1f);

		var tr = Scene.Trace.Ray( wheelAttachPosition, wheelExtend )
			.WithTag(TraceTags.SOLID)
			.WithoutTags("nocollide")
			.IgnoreGameObject(GameObject.Parent)
			.IgnoreGameObject(GameObject )
			.Run();
		Transform.Position = tr.EndPosition + Vector3.Up * Radius;

		if ( !tr.Hit || !doPhysics )
		{
			return tr.Hit;
		}

		// Spring Forces

		const float SPRING_STRENGTH = 800f;
		const float SPRING_DAMPING = 100f;

		Vector3 velocity = physics.GetVelocityAtPoint(wheelAttachPosition);
		Vector3 springDirection = physicsComponent.Transform.Rotation.Up;

		float springOffset = wheelAttachPosition.Distance(tr.EndPosition);
		float springVelocity = Vector3.Dot( springDirection, velocity );

		float springForce = (springOffset * SPRING_STRENGTH) - (springVelocity * SPRING_DAMPING);

		physics.ApplyImpulseAt(wheelAttachPosition, springDirection * springForce);

		// Correction / Steering force
		const float TIRE_GRIP = 1f;
		const float TIRE_MASS = 10f;

		float tireGrip = TIRE_GRIP;
		tireGrip += 0.5f * Vehicle.BreakInput;

		Vector3 correctionDirection = Transform.Rotation.Right;
		float steerVelocity = Vector3.Dot(correctionDirection, velocity);
		float correctionForce = -steerVelocity * tireGrip / dt;

		physics.ApplyImpulseAt( wheelAttachPosition, correctionDirection * TIRE_MASS * correctionForce );

		if(IsDriving)
		{
			float throttle = Vehicle.ThrottleInput;
			// Forward/Reversing force
			Vector3 forward = Transform.Rotation.Forward;
			if (throttle != 0)
			{
				float forwardVelocity = Vector3.Dot( forward, physics.Velocity );
				float speedFraction = forwardVelocity / Vehicle.GetMaxSpeed();

				float acceleration = Vehicle.GetAcceleration();
				if(speedFraction >= 0.8f)
				{
					acceleration *= 0.2f;
				}

				float torque = acceleration * throttle * 10;
				physics.ApplyImpulseAt( wheelAttachPosition, torque * forward );
			}
		}

		return true;
	}

	protected override void DrawGizmos()
	{
		Gizmo.Draw.Color = Color.Magenta;

		Vector3 direction = Vector3.Left * Width;
		Gizmo.Draw.SolidCylinder( -direction, direction, Radius );

		if ( !Gizmo.IsSelected )
			return;

		const float FORWARD_HELPER_SIZE = 40f;
		Gizmo.Draw.Line( Vector3.Zero, Vector3.Forward * FORWARD_HELPER_SIZE );

		const float EXTEND_GIZMO_LENGTH = 20f;

		Gizmo.Draw.Color = Color.Red;
		float length = Radius + EXTEND_GIZMO_LENGTH;

		Vector3 up = Transform.Local.PointToLocal( Transform.LocalPosition + Vector3.Up );
		Vector3 wheelAttachPos = Transform.Local.PointToLocal(InitialPosition);
		Vector3 wheelExtend = wheelAttachPos - up * (length * Transform.Scale);

		Gizmo.Draw.Line( wheelAttachPos, wheelExtend );
		Gizmo.Draw.SolidSphere( wheelAttachPos, 4f );
		Gizmo.Draw.LineSphere( wheelExtend, 4f );
	}
}
