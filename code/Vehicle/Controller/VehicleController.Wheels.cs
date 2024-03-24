using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleController
{
	public class Wheel
	{
		private const float DEFAULT_SIZE = 14f;
		private const float DEFAULT_WIDTH = 8f;
		public ModelRenderer Renderer { get; set; }
		public float Radius { get; set; } = DEFAULT_SIZE;
		public float Width { get; set; } = DEFAULT_WIDTH;
		public float Mass { get; set; } = DEFAULT_WIDTH;
		public bool IsDriving { get; set; }
		public bool IsTurning { get; set; }
		public Vector3 InitialPosition { get; set; }
		public Angles InitialRotation { get; set; }
		[Hide, JsonIgnore] public Transform Transform;
		internal Angles InitialModelRotation { get; set; }
	}

	[Property] public List<Wheel> Wheels { get; set; }

	private bool wheelsOnGround;
	private bool drivingWheelsOnGround;
	private bool turningWheelsOnGround;
	private float accelerateDirection;
	private float turningGrip;

	private float wheelAngle = 0.0f;
	private float wheelRevolute = 0.0f;
	private float accelerationTilt;
	private float wheelSpeed;
	private void InitialiseWheels()
	{
		if ( Wheels == null || !Wheels.Any() ) return;

		foreach ( var wheel in Wheels)
		{
			wheel.Transform = new( wheel.InitialPosition, wheel.InitialRotation );
			wheel.InitialModelRotation = wheel.Renderer.Transform.LocalRotation.Angles();
		}
	}
	private void UpdateWheels()
	{
		if ( Wheels == null || !Wheels.Any() ) return;

		wheelAngle = wheelAngle.LerpTo( TurnDirection * 30, 1.0f - MathF.Pow( 0.001f, Time.Delta ) );
		wheelRevolute += (wheelSpeed / 30.0f).RadianToDegree() * Time.Delta;
		wheelRevolute %= 360f;

		RaycastWheels( true, Time.Delta );

		foreach(var wheel in Wheels)
		{
			Angles displayAngles = wheel.InitialModelRotation;
			if (wheel.IsTurning)
			{
				wheel.Transform.Rotation = wheel.Transform.Rotation.Angles().WithYaw( wheel.InitialRotation.yaw + wheelAngle );
				displayAngles.yaw += wheelAngle;
			}
			wheel.Renderer.Transform.LocalRotation = displayAngles.WithPitch( wheelRevolute);
			//wheel.Renderer.Transform.LocalPosition = wheel.Transform.Position;
		}
	}

	private void RaycastWheels(bool doPhysics, float dt )
	{
		wheelsOnGround = false;
		drivingWheelsOnGround = false;
		turningWheelsOnGround = false;

		List<(Wheel, SceneTraceResult)> wheelTraces = new();

		foreach ( var wheel in Wheels )
		{
			wheelTraces.Add( new( wheel, Raycast( wheel, dt, 1f ) ) );
		}

		foreach((var wheel, var tr) in wheelTraces)
		{
			if ( !tr.Hit ) continue;

			wheelsOnGround = true;
			if ( wheel.IsDriving )
				drivingWheelsOnGround = true;

			if ( wheel.IsTurning )
				turningWheelsOnGround = true;

			PhysicsBody physics = Rigidbody.PhysicsBody;
			Vector3 wheelAttachPosition = Transform.World.PointToWorld( wheel.InitialPosition );

			wheel.Transform.Position = tr.EndPosition + Vector3.Up * wheel.Radius;
			// Spring Forces
			float springStrength = GetSpringStrength();
			float stringDamping = GetSpringDamping();

			Vector3 velocity = physics.GetVelocityAtPoint( wheelAttachPosition );
			Vector3 localVelocity = Transform.Local.VelocityToLocal( velocity );
			Vector3 springDirection = Rigidbody.Transform.Rotation.Up;

			Vector3 forward = Transform.Rotation.Forward;
			float forwardVelocity = localVelocity.x;

			float springFraction = 1 - tr.Fraction; // What fraction of the wheel is below ground
			float springOffset = springFraction * wheel.Radius;
			float springVelocity = localVelocity.z;
			//Log.Info( $"{springOffset}" );

			float springForce = (springOffset * springStrength) - (springVelocity * stringDamping);

			physics.ApplyImpulseAt( wheelAttachPosition, springDirection * springForce );

			// Correction / Steering force

			Vector3 steerDirection = wheel.Transform.Rotation.Right;
			float steerVelocity = localVelocity.y;
			float tireGrip = GetSlideGrip( steerVelocity );
			if ( BreakInput > 0f )
			{
				tireGrip *= BreakInput.Remap( 0, 1, 1, 3 );
			}

			float correctionForce = -steerVelocity * tireGrip;
			//Gizmo.Draw.WorldText( $"{steerVelocity:00.0}", new( wheel.Transform.Position + Vector3.Up * 130, Rotation.FromRoll( 90f ) ) );

			physics.ApplyForceAt( wheelAttachPosition, steerDirection * wheel.Mass * correctionForce );

			if ( wheel.IsDriving )
			{
				float throttle = ThrottleInput;
				// Forward/Reversing force

				if ( throttle != 0 )
				{
					float acceleration = GetAcceleration( forwardVelocity );
					float torque = acceleration * throttle * 10;
					physics.ApplyImpulseAt( wheelAttachPosition, torque * forward );
				}
			}
		}
	}

	private SceneTraceResult Raycast( Wheel wheel, float dt, float extend = 1.2f )
	{
		float downExtend = wheel.Radius * extend;
		Vector3 wheelAttachPosition = Transform.World.PointToWorld(wheel.InitialPosition);
		Vector3 wheelExtend = wheelAttachPosition + Vector3.Down * (downExtend + 1f);

		var tr = Scene.Trace.Ray( wheelAttachPosition, wheelExtend )
			.WithTag( TraceTags.SOLID )
			.WithoutTags( "nocollide" )
			.IgnoreGameObjectHierarchy( GameObject )
			.Run();

		return tr;
	}
}
