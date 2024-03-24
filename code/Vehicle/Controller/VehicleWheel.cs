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
	[Property] public float Mass { get; set; } = DEFAULT_WIDTH;
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
			.IgnoreGameObjectHierarchy(GameObject.Parent)
			.Run();
		Transform.Position = tr.EndPosition + Vector3.Up * Radius;

		if ( !tr.Hit || !doPhysics )
		{
			return tr.Hit;
		}

		// Spring Forces

		const float SPRING_STRENGTH = 600f;
		const float SPRING_DAMPING = 200f;

		Vector3 velocity = physics.GetVelocityAtPoint(wheelAttachPosition);
		Vector3 localVelocity = Vehicle.Transform.Local.VelocityToLocal( velocity );
		Vector3 springDirection = physicsComponent.Transform.Rotation.Up;

		Vector3 forward = Transform.Rotation.Forward;
		float forwardVelocity = localVelocity.x;

		float springOffset = wheelAttachPosition.Distance(tr.EndPosition);
		float springVelocity = localVelocity.z;

		float springForce = (springOffset * SPRING_STRENGTH) - (springVelocity * SPRING_DAMPING);
		Gizmo.Draw.Arrow( wheelExtend, wheelExtend + springDirection * 50f );

		physics.ApplyImpulseAt(wheelAttachPosition, springDirection * springForce);

		// Correction / Steering force

		Vector3 steerDirection = Transform.Rotation.Right;
		float steerVelocity = localVelocity.y;
		float tireGrip = Vehicle.GetSlideGrip( steerVelocity );
		if(Vehicle.BreakInput > 0f)
		{
			tireGrip *= Vehicle.BreakInput.Remap(0, 1, 1, 3);
		}

		float correctionForce = -steerVelocity * tireGrip;
		Gizmo.Draw.WorldText( $"{steerVelocity:00.0}", new( Transform.Position + Vector3.Up * 130, Rotation.FromRoll(90f)) );

		physics.ApplyForceAt( wheelAttachPosition, steerDirection * Mass * correctionForce );

		if(IsDriving)
		{
			float throttle = Vehicle.ThrottleInput;
			// Forward/Reversing force
			
			if (throttle != 0)
			{
				float acceleration = Vehicle.GetAcceleration(forwardVelocity);
				float torque = acceleration * throttle * 10;
				physics.ApplyImpulseAt( wheelAttachPosition, torque * forward );
			}
		}

		return true;
	}

	protected override void OnUpdate()
	{
		const bool DEBUG = true;
		if ( Vehicle == null ) return;
		if ( !DEBUG ) return;

		PhysicsBody physics = Vehicle.Rigidbody.PhysicsBody;
		Vector3 velocity = physics.GetVelocityAtPoint( GetAttachmentWorldPosition() );
		Vector3 localVelocity = Vehicle.Transform.Local.VelocityToLocal( velocity );
		Vector3 forward = Transform.Rotation.Forward;
		float forwardVelocity = Vector3.Dot( forward, physics.Velocity );
		// Spring force
		Gizmo.Draw.Color = Color.Green;
		Gizmo.Draw.WorldText( $"{velocity.z:000.00}", new( Transform.Position + Vector3.Up * 90, Rotation.FromRoll( 90f ) ) );

		// Correction / Steering force

		Gizmo.Draw.Color = Color.Red;
		float tireGrip = Vehicle.GetSlideGrip( forwardVelocity );
		if ( Vehicle.BreakInput > 0f )
		{
			tireGrip *= Vehicle.BreakInput.Remap( 0, 1, 1, 3 );
		}

		Vector3 correctionDirection = Transform.Rotation.Right;
		float steerVelocity = velocity.y;
		float correctionForce = -steerVelocity * tireGrip;
		Gizmo.Draw.Arrow( Transform.Position, Transform.Position + correctionDirection * correctionForce * 5f );
		Gizmo.Draw.WorldText( $"{correctionForce:000.00}", new( Transform.Position + Vector3.Up * 60, Rotation.FromRoll(90f) ), size: 10 );

		// Forward/Reversing force


		Gizmo.Draw.Color = Color.Blue;
		float throttle = Vehicle.ThrottleInput;
		float acceleration = Vehicle.GetAcceleration( forwardVelocity );
		float torque = acceleration * throttle * 10;
		Gizmo.Draw.Arrow( Transform.Position, Transform.Position + forward * torque / 10);
	}

	protected override void DrawGizmos()
	{
		const float FORWARD_HELPER_SIZE = 40f;
		const float EXTEND_GIZMO_LENGTH = 20f;

		Gizmo.Draw.Color = Color.Magenta;
		Vector3 direction = Vector3.Left * Width;
		Gizmo.Draw.SolidCylinder( -direction, direction, Radius );

		if ( !Gizmo.IsSelected )
			return;

		Gizmo.Draw.Line( Vector3.Zero, Vector3.Forward * FORWARD_HELPER_SIZE );

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
