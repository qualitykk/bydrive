using Sandbox;
using System.ComponentModel.DataAnnotations;
using static Bydrive.RaceMatchInformation;

namespace Bydrive;

[Icon( "electric_car" )]
[Category( "Vehicle" )]
public sealed partial class VehicleController : Component
{
	const float AUTO_RESPAWN_TIME = 2f;
	const int AUTO_RESPAWN_DAMAGE = 1;
	[Property, Required, Title("Physics Body")] public Rigidbody Rigidbody { get; set; }
	public PhysicsBody Body => Rigidbody?.PhysicsBody;
	public float Speed { get; private set; }
	public float TurnDirection { get; private set; }
	private RaceParticipant GetParticipant()
	{
		return GameObject.Components.Get<RaceParticipant>( FindMode.EnabledInSelfAndDescendants );
	}
	public override void Reset()
	{
		base.Reset();
		InitialiseCombat();
		InitialiseAbilities();
		ResetStats();
		ResetInput();
	}
	protected override void OnUpdate()
	{
		TickSounds();
		VerifyInput();

		TickAbilities();
		TickStats();
		Move();
	}

	#region Movement

	private float turnLean;
	private float airTilt;

	private bool canDrive;
	private float CalculateTurnFactor( float direction, float forwardsSpeed )
	{
		const float MAX_TURN_FACTOR = 0.9f;
		// Turning rate is at its highest a certain forwards speed 
		// After that, it decreases

		float lowSpeedFactor = MathF.Min( forwardsSpeed / GetTurnSpeedIdealDistance(), 1 );
		float highSpeedFactor = 1.0f - (forwardsSpeed / GetMaxSpeed()).Clamp( 0, GetTurnSpeedVelocityFactor() );
		float factor = direction * lowSpeedFactor * highSpeedFactor;
		factor = MathF.Abs( factor ).Clamp( 0, MAX_TURN_FACTOR ) * MathF.Sign( factor );
		return factor;
	}

	private void Move()
	{
		float dt = Time.Delta;
		Rotation rotation = Body.Rotation;
		float scale = Transform.Scale.z;
		float maxSpeed = GetMaxSpeed();
		float baseAcceleration = Stats.Acceleration;
		float acceleration = GetAcceleration();

		//Tilting is the forward and backward tilt caused by acceleration or decelleration of the vehicle
		float targetTilt = 0;
		//Lean is the rotation around an axis going through the car from back to font, represents the forces caused by turning at high speeds
		float targetLean = 0;

		//Acceleration direction here appears to simply refer to the input, and therefore the speed.
		accelerateDirection = ThrottleInput.Clamp( -1, 1 );
		TurnDirection = TurnDirection.LerpTo( TurnInput.Clamp( -1, 1 ), 1.0f - MathF.Pow( 0.0003f, dt ) );

		//Same as above, but for the roll and tilt inpuuts, slower than turning.
		airTilt = airTilt.LerpTo( TiltInput.Clamp( -1, 1 ), 1.0f - MathF.Pow( 0.0001f, dt ) );

		Vector3 localVelocity = Transform.Local.VelocityToLocal(Body.Velocity);
		float forwardSpeed = MathF.Abs(localVelocity.x);

		//If all four wheels are touching the ground
		bool fullyGrounded = drivingWheelsOnGround && turningWheelsOnGround;

		//If the front and back wheels are on the ground
		if ( wheelsOnGround )
		{
			const float MAX_LEAN_SPEED = 500f;
			// Check lean
			var speedFraction = MathF.Min( forwardSpeed / MAX_LEAN_SPEED, 1 );

			targetTilt = accelerateDirection.Clamp( -1.0f, 1.0f );
			targetLean = speedFraction * TurnDirection;
		}

		//Lerp our acceleration tilt to our target tilt
		AccelerationTilt = AccelerationTilt.LerpTo( targetTilt, 1.0f - MathF.Pow( 0.01f, dt ) );
		//Do the same as the above, but with the lean.
		turnLean = turnLean.LerpTo( targetLean, 1.0f - MathF.Pow( 0.01f, dt ) );

		UpdateWheels();

		//Set a local variable for if we can use air control
		bool canAirControl = false;

		//This is the lateral velocity of the car without its z component. Multiplied by the rotation to get its velocity relative to the vehicle
		Vector3 relativeVelocity = rotation * localVelocity.WithZ( 0 );
		//Not really sure what this does, if I had to take take a guess, this is getting the change in our velocity to the power of 5 clamped between 0 and 1
		float velocityDelta = MathF.Pow( (relativeVelocity.Length / maxSpeed).Clamp( 0, 1 ), 5.0f ).Clamp( 0, 1 );
		if ( velocityDelta < 0.01f ) velocityDelta = 0;

		//So as far as I can tell, this is how grip gets applied, it takes a dot product of the forward velocity with our velocity and sets our grip accordingly
		var angle = (rotation.Forward.Normal * MathF.Sign( localVelocity.x )).Normal.Dot( relativeVelocity.Normal ).Clamp( 0.0f, 1.0f );
		angle = angle.LerpTo( 1.0f, 1.0f - velocityDelta );
		grip = grip.LerpTo( angle, 1.0f - MathF.Pow( 0.001f, dt ) );

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
			const float TURN_AIR_MULTIPLIER = 0.35f;
			float turnSpeed = GetTurnSpeed();
			float turnAmount = MathF.Sign( localVelocity.x ) * turnSpeed * CalculateTurnFactor( TurnDirection, MathF.Abs( localVelocity.x ) ) * dt;
			if ( !turningWheelsOnGround )
			{
				turnAmount *= TURN_AIR_MULTIPLIER;
			}
			Body.AngularVelocity += rotation * new Vector3( 0f, 0f, turnAmount );

			if ( drivingWheelsOnGround )
			{
				const float REVERSING_ACCELERATION_MULTIPLIER = 0.8f;
				//Basically we're saying as the angle of the cars totaly velocity versus the angle of the cars forward direction approaches 90 degrees
				//Give us a big boost to the speed of our forward acceleration in order to not loose all momentum when drifting. 
				var fac = 1.0f;
				float f = MathF.Pow( 1 - angle, 4f );
				fac = fac.LerpTo( 10f, f );

				//The speed factor decreases the amount of acceleration we have depending on how fast we're currently going
				float speedFactor = 1.0f - MathF.Pow( forwardSpeed / maxSpeed, 3.5f );
				speedFactor = speedFactor.Clamp( 0.0f, 1.0f );

				//Calculate our acceleration based on our input..
				//Slow down acceleration if going backwards
				float forwardimpulse = speedFactor * (accelerateDirection < 0f ? baseAcceleration * REVERSING_ACCELERATION_MULTIPLIER : acceleration * fac) * accelerateDirection * dt;
				//Use this to then get the impulse and apply it to our body's velocity
				var impulse = rotation * new Vector3( forwardimpulse, 0, 0 );
				Body.Velocity += impulse;
			}
		}
		else
		{
			if(couldDrive)
			{
				RaceNotifications.Add( this, new( "Unsafe vehicle position, respawning...", UI.Colors.Notification.Danger, AUTO_RESPAWN_TIME, "warning" ) );
				GetParticipant()?.RespawnIn( AUTO_RESPAWN_TIME, AUTO_RESPAWN_DAMAGE );
			}
		}

		//Angular Damping, lerps to 5 based off grip
		const float MAX_ANGULAR_DAMPING = 5f;
		var angularDamping = 0.0f;
		angularDamping = angularDamping.LerpTo( MAX_ANGULAR_DAMPING, grip );

		Body.LinearDamping = 0;
		Body.AngularDamping = fullyGrounded ? angularDamping : 0.5f;

		//If we're on the ground
		if ( wheelsOnGround )
		{
			Body.GravityScale = 1f;

			// Get our local velocity
			localVelocity = rotation.Inverse * Body.Velocity;
			// Wheel rotation speed
			WheelSpeed = localVelocity.x;

			airTilt = 0;

			// Forward grip, gets lerped between 0.1 and 0.9 based on whether or not we are breaking
			var forwardGrip = 0.1f;
			forwardGrip = forwardGrip.LerpTo( 0.9f, BreakInput );

			// Get the forward speed then a value representing how close to the 'top speed' we are from 0-1
			float speedFactor = (forwardSpeed / maxSpeed).Clamp( 0.0f, 1.0f );

			// If we are moving gast enough, we basically make it so our grip is a lot less the closer our velocity 
			var fac = 0.0f;
			if ( speedFactor > 0.5f )
			{
				float f = (1 - angle);
				fac = fac.LerpTo( 0.4f, f );
			}

			// Velocity damping function, we pass the current velocity ,rotation, and a vector3 with our grip and forward grip 

			Vector3 dampenedVelocity = VelocityDamping( Body.Velocity, rotation, new Vector3( forwardGrip, grip - fac, 0 ), dt ); ;
			Body.Velocity = dampenedVelocity;
		}
		else
		{
			Body.GravityScale = 1.25f;

			Vector3 tracePosition = Transform.Position;
			var tr = Scene.Trace.Ray( tracePosition, tracePosition + rotation.Down * 50 )
				.IgnoreGameObject( GameObject )
				.Run();

			canAirControl = !tr.Hit;
		}

		if ( canAirControl && airTilt != 0 )
		{
			float offset = 50;
			var s = Body.Position + (rotation * Body.LocalMassCenter) + (rotation.Down * (10 * scale));
			var st = Body.MassCenter;
			var tr = Scene.Trace.Ray( st, st + rotation.Right * (40 * scale) )
				.IgnoreGameObject( GameObject )
				.Run();

			bool dampen = false;

			if ( !tr.Hit && airTilt.Clamp( -1, 1 ) != 0 )
			{
				var force = 200.0f;
				Body.ApplyForceAt( Body.MassCenter + rotation.Forward * (offset * airTilt), (rotation.Down * airTilt) * (airTilt * (Body.Mass * force)) );

				dampen = true;
			}

			if ( dampen )
				Body.AngularVelocity = VelocityDamping( Body.AngularVelocity, rotation, 0.95f, dt );
		}

		localVelocity = rotation.Inverse * Body.Velocity;
		Speed = localVelocity.x;

		const float MAX_ANGULAR_VELOCITY_X = 0.5f;
		const float MAX_ANGULAR_VELOCITY_Y = 0.8f;
		Body.AngularVelocity = Body.AngularVelocity.WithX( Body.AngularVelocity.x.Clamp( -MAX_ANGULAR_VELOCITY_X, MAX_ANGULAR_VELOCITY_X ) ).WithY( Body.AngularVelocity.y.Clamp( -MAX_ANGULAR_VELOCITY_Y, MAX_ANGULAR_VELOCITY_Y ) );
	}
	/// <summary>
	/// Dampens our velocity
	/// </summary>
	private static Vector3 VelocityDamping( Vector3 velocity, Rotation rotation, Vector3 damping, float dt )
	{
		//Get our local velocity
		var localVelocity = rotation.Inverse * velocity;
		//Calculate the damping power
		var dampingPow = new Vector3( MathF.Pow( 1.0f - damping.x, dt ), MathF.Pow( 1.0f - damping.y, dt ), MathF.Pow( 1.0f - damping.z, dt ) );
		return rotation * (localVelocity * dampingPow);
	}

	#endregion

	private List<SoundHandle> sounds = new();
	private void TickSounds()
	{
		foreach(var sound in sounds)
		{
			sound.Position = Transform.Position;
			sound.Update();
		}
	}
	public SoundHandle PlaySound(SoundEvent sound)
	{
		var handle = Sound.Play( sound, Transform.Position );
		sounds.Add( handle );
		return handle;
	}
	protected override void DrawGizmos()
	{
		const float POSITION_HELPER_RADIUS = 4f;
		Gizmo.Draw.SolidSphere( ItemSpawnPosition, POSITION_HELPER_RADIUS );
	}
}
