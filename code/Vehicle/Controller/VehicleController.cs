using Sandbox;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Bydrive;

[Icon( "electric_car" )]
[Category( "Vehicle" )]
public sealed partial class VehicleController : Component
{
	const float AUTO_RESPAWN_TIME = 2f;
	const int AUTO_RESPAWN_DAMAGE = 1;
	[RequireComponent, Title("Physics Body")] public Rigidbody Rigidbody { get; set; }
	[AutoReference] public VehicleDefinition Definition { get; set; }
	public PhysicsBody Body => Rigidbody?.PhysicsBody;
	public float Speed { get; private set; }
	public float TurnDirection { get; private set; }
	private RaceParticipant GetParticipant()
	{
		return GameObject.Components.GetInDescendantsOrSelf<RaceParticipant>();
	}
	protected override void OnStart()
	{
		Initialise();
	}
	public void Initialise()
	{
		Body.Velocity = Vector3.Zero;
		Body.AngularVelocity = Vector3.Zero;

		InitialiseWheels();
		InitialiseCombat();
		InitialiseAbilities();
		InitialiseItems();

		ResetStats();
		ResetInput();
	}
	protected override void OnUpdate()
	{
		TickSounds();
		VerifyInput();

		TickAbilities();
		TickStats();
		TickItems();
	}

	protected override void OnFixedUpdate()
	{
		Move();
	}

	protected override void OnDestroy()
	{
		foreach(var sound in activeSounds)
		{
			sound.Stop();
		}
	}

	#region Movement

	private bool canDrive;
	private float CalculateTurnFactor( float direction, float forwardsSpeed )
	{
		return direction * GetTurnFactor(forwardsSpeed);
	}

	private void Move()
	{
		float dt = Time.Delta;
		Rotation rotation = Body.Rotation;
		float scale = Transform.Scale.z;
		float maxSpeed = GetMaxSpeed();

		//Acceleration direction here appears to simply refer to the input, and therefore the speed.
		accelerateDirection = ThrottleInput.Clamp( -1, 1 );
		TurnDirection = TurnDirection.LerpTo( TurnInput.Clamp( -1, 1 ), 1.0f - MathF.Pow( 0.0003f, dt ) );

		Vector3 localVelocity = Transform.Local.VelocityToLocal(Body.Velocity);
		float forwardSpeed = MathF.Abs(localVelocity.x);

		//If all four wheels are touching the ground
		bool fullyGrounded = drivingWheelsOnGround && turningWheelsOnGround;

		UpdateWheels();

		//This is the lateral velocity of the car without its z component. Multiplied by the rotation to get its velocity relative to the vehicle
		Vector3 relativeVelocity = rotation * localVelocity.WithZ( 0 );
		//Not really sure what this does, if I had to take take a guess, this is getting the change in our velocity to the power of 5 clamped between 0 and 1
		float velocityDelta = MathF.Pow( (relativeVelocity.Length / maxSpeed).Clamp( 0, 1 ), 5.0f ).Clamp( 0, 1 );
		if ( velocityDelta < 0.01f ) velocityDelta = 0;

		// Gets forward angle as single number
		float angle = (rotation.Forward.Normal * MathF.Sign( localVelocity.x )).Normal.Dot( relativeVelocity.Normal ).Clamp( 0.0f, 1.0f );
		angle = angle.LerpTo( 1.0f, 1.0f - velocityDelta );

		// Check if we havent flipped over or gotten stuck in any other way
		const float DRIVABLE_PITCH = 50f;
		const float DRIVABLE_ROLL = 60f;
		float roll = rotation.Roll();
		float pitch = rotation.Pitch();
		bool couldDrive = canDrive;
		canDrive = MathF.Abs(roll) < DRIVABLE_ROLL && MathF.Abs(pitch) < DRIVABLE_PITCH;

		if(canDrive)
		{
			if(!couldDrive)
			{
				GetParticipant()?.RespawnCancel();
			}

			// Turn even when not on ground
			// Calculate turn factor takes in our turn direction and the absolute value of our velocity this basically all effects how fast we can turn
			const float TURN_AIR_MULTIPLIER = 0.1f;
			float turnSpeed = GetTurnSpeed();
			float turnAmount = MathF.Sign( localVelocity.x ) * turnSpeed * CalculateTurnFactor( TurnDirection, MathF.Abs( localVelocity.x ) ) * dt;
			if (!turningWheelsOnGround )
			{
				turnAmount *= TURN_AIR_MULTIPLIER;
			}

			Body.AngularVelocity += rotation * new Vector3( 0f, 0f, turnAmount );
		}
		else
		{
			if(couldDrive)
			{
				RaceNotifications.AddObject( this, new( "Unsafe vehicle position, respawning...", UI.Colors.Notification.Danger, AUTO_RESPAWN_TIME, "warning" ) );
				GetParticipant()?.RespawnIn( AUTO_RESPAWN_TIME, AUTO_RESPAWN_DAMAGE );
			}
		}

		// Gets grip (damping delta) based on velocity
		turningGrip = turningGrip.LerpTo( angle, 1.0f - MathF.Pow( 0.001f, dt ) );

		//Angular Damping, lerps to 5 based off grip
		float maxAngularDamping = GetAngularDamping();
		var angularDamping = 0.0f;
		angularDamping = angularDamping.LerpTo( maxAngularDamping, turningGrip );
		Body.AngularDamping = fullyGrounded ? angularDamping : 0.5f;

		//If we're on the ground
		if ( wheelsOnGround )
		{
			Body.GravityScale = 1f;

			// Get our local velocity
			localVelocity = rotation.Inverse * Body.Velocity;
			// Wheel rotation speed
			wheelSpeed = localVelocity.x.SnapToGrid(5f);

			// Forward grip, gets lerped between 1x and 9x based on whether or not we are breaking
			var forwardGrip = 0.1f;
			forwardGrip = forwardGrip.LerpTo( forwardGrip * 9, BreakInput );

			// Get the forward speed then a value representing how close to the 'top speed' we are from 0-1
			float speedFactor = (forwardSpeed / maxSpeed).Clamp( 0.0f, 1.0f );

			// If we are moving gast enough, we basically make it so our grip is a lot less the closer our velocity 
			float turningGripDamping = 0.0f;
			if ( speedFactor > 0.5f )
			{
				turningGripDamping = turningGripDamping.LerpTo( 0.4f, 1 - angle );
			}

			// Velocity damping function, we pass the current velocity ,rotation, and a vector3 with our grip and forward grip 

			Vector3 dampenedVelocity = VelocityDamping( Body.Velocity, rotation, new Vector3( forwardGrip, turningGrip - turningGripDamping, 0 ), dt ); ;
			Body.Velocity = dampenedVelocity;
		}
		else
		{
			Body.GravityScale = 1.25f;

			Vector3 tracePosition = WorldPosition;
			var tr = Scene.Trace.Ray( tracePosition, tracePosition + rotation.Down * 50 )
				.IgnoreGameObject( GameObject )
				.Run();
		}

		localVelocity = rotation.Inverse * Body.Velocity;
		Speed = localVelocity.x;

		const float MAX_ANGULAR_VELOCITY_X = 0.5f;
		const float MAX_ANGULAR_VELOCITY_Y = 0.8f;
		Vector3 clampedAngularVelocity = new Vector3( Body.AngularVelocity );
		clampedAngularVelocity.x = clampedAngularVelocity.x.Clamp( -MAX_ANGULAR_VELOCITY_X, MAX_ANGULAR_VELOCITY_X );
		clampedAngularVelocity.y = clampedAngularVelocity.y.Clamp( -MAX_ANGULAR_VELOCITY_Y, MAX_ANGULAR_VELOCITY_Y );

		Body.AngularVelocity = clampedAngularVelocity;
	}
	/// <summary>
	/// Dampens our velocity
	/// </summary>
	internal static Vector3 VelocityDamping( Vector3 velocity, Rotation rotation, Vector3 damping, float dt )
	{
		//Get our local velocity
		var localVelocity = rotation.Inverse * velocity;
		//Calculate the damping power
		var dampingPow = new Vector3( MathF.Pow( 1.0f - damping.x, dt ), MathF.Pow( 1.0f - damping.y, dt ), MathF.Pow( 1.0f - damping.z, dt ) );
		return rotation * (localVelocity * dampingPow);
	}

	#endregion
	protected override void DrawGizmos()
	{
		const float POSITION_HELPER_RADIUS = 4f;
		const float FORWARD_HELPER_SIZE = 40f;
		const float EXTEND_GIZMO_LENGTH = 20f;
		Gizmo.Draw.SolidSphere( ItemSpawnPosition, POSITION_HELPER_RADIUS );

		if(Wheels != null && Wheels.Any())
		{
			foreach ( var wheel in Wheels )
			{
				Gizmo.Draw.Color = Color.Magenta;
				Vector3 wheelPosition = wheel.InitialPosition;
				Rotation wheelRotation = wheel.InitialRotation;

				/*
				Transform debugTransform = wheel.Transform;
				debugTransform.Position += Vector3.Up * 80f;
				debugTransform.Rotation = Rotation.From( 0, 90f, 90f );
				Gizmo.Draw.WorldText( $"{wheel.WorldRotation.Yaw()}", debugTransform, size: 10 );
				*/

				Vector3 wheelSize = wheelRotation.Left * wheel.Width;
				Gizmo.Draw.SolidCylinder( wheelPosition - wheelSize, wheelPosition + wheelSize, wheel.Radius );
				Gizmo.Draw.Line( wheelPosition, wheelPosition + wheelRotation.Forward * FORWARD_HELPER_SIZE );

				Gizmo.Draw.Color = Color.Red;
				float length = wheel.Radius + EXTEND_GIZMO_LENGTH;

				Vector3 wheelAttachPos = wheel.InitialPosition;
				Vector3 wheelExtend = wheelAttachPos - wheelRotation.Up * (length * Transform.Scale);

				Gizmo.Draw.Line( wheelAttachPos, wheelExtend );
				Gizmo.Draw.SolidSphere( wheelAttachPos, 4f );
				Gizmo.Draw.LineSphere( wheelExtend, 4f );
			}
		}
	}
}
