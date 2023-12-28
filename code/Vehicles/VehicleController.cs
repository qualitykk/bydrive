using Sandbox;
namespace Redrome;
public sealed partial class VehicleController : Component
{
	#region Stats
	// TODO: Get these from vehicle stats
	[Property] public float MaxSpeed { get; set; } = 512f;
	[Property] public float Acceleration { get; set; } = 256f;
	[Property] public float BreakSpeed { get; set; } = 384f;
	[Property] public float TurnSpeed { get; set; } = 90f;
	[Property, Title("Physics Body")] public Rigidbody Rigidbody { get; set; }
	#endregion
	public PhysicsBody Body => Rigidbody?.PhysicsBody;
	public float Speed { get; set; }
	protected override void OnUpdate()
	{
		// For now, just use modified PlayerController code
		BuildInput();
		Move();
	}

	private float turnDirection;
	private float turnLean;
	private float airRoll;
	private float airTilt;
	private void Move()
	{
		float dt = Time.Delta;

		//Tilting is the forward and backward tilt caused by acceleration or decelleration of the vehicle
		float targetTilt = 0;
		//Lean is the rotation around an axis going through the car from back to font, represents the forces caused by turning at high speeds
		float targetLean = 0;

		//Acceleration direction here appears to simply refer to the input, and therefore the speed.
		accelerateDirection = throttleInput.Clamp( -1, 1 );

		turnDirection = turnDirection.LerpTo( turnInput.Clamp( -1, 1 ), 1.0f - MathF.Pow( 0.001f, dt ) );

		//Same as above, but for the roll and tilt inpuuts, slower than turning.
		airRoll = airRoll.LerpTo( rollInput.Clamp( -1, 1 ), 1.0f - MathF.Pow( 0.0001f, dt ) );
		airTilt = airTilt.LerpTo( tiltInput.Clamp( -1, 1 ), 1.0f - MathF.Pow( 0.0001f, dt ) );

		Vector3 localVelocity = Transform.World.PointToLocal(Body.Velocity);

		//If all four wheels are touching the ground
		bool fullyGrounded = drivingWheelsOnGround && turningWheelsOnGround;

		//If the front and back wheels are on the ground
		if ( wheelsOnGround )
		{
			//This is where the magic happens, forward speed is calculated as the absolute value of our forward velocity
			var forwardSpeed = MathF.Abs( localVelocity.x );
			//No fucking clue what this does other than get the minimum beteeen 1 and the speed divided by 100, don't know what its used for
			var speedFraction = MathF.Min( forwardSpeed / 500.0f, 1 );

			targetTilt = accelerateDirection.Clamp( -1.0f, 1.0f );
			targetLean = speedFraction * turnDirection;
		}

		//Lerp our acceleration tilt to our target tilt
		AccelerationTilt = AccelerationTilt.LerpTo( targetTilt, 1.0f - MathF.Pow( 0.01f, dt ) );
		//Do the same as the above, but with the lean.
		turnLean = turnLean.LerpTo( targetLean, 1.0f - MathF.Pow( 0.01f, dt ) );

		UpdateWheels();

		//Set a local variable for if we can use air control
		bool canAirControl = false;

		//This is the lateral velocity of the car without its z component. Multiplied by the rotation to get its velocity relative to the vehicle
		Vector3 relativeVelocity = Transform.Rotation * localVelocity.WithZ( 0 );
		//Not really sure what this does, if I had to take take a guess, this is getting the change in our velocity to the power of 5 clamped between 0 and 1
		var vDelta = MathF.Pow( (relativeVelocity.Length / 1000.0f).Clamp( 0, 1 ), 5.0f ).Clamp( 0, 1 );
		if ( vDelta < 0.01f ) vDelta = 0;

		//So as far as I can tell, this is how grip gets applied, it takes a dot product of the forward velocity with our velocity and sets our grip accordingly
		var angle = (Transform.Rotation.Forward.Normal * MathF.Sign( localVelocity.x )).Normal.Dot( relativeVelocity.Normal ).Clamp( 0.0f, 1.0f );
		angle = angle.LerpTo( 1.0f, 1.0f - vDelta );
		grip = grip.LerpTo( angle, 1.0f - MathF.Pow( 0.001f, dt ) );


		//Looks like we're using some good old fashioned two wheel drive, this seems where the main acceleration is calculated and applied
		if ( drivingWheelsOnGround )
		{
			//Get our current forward speed
			var forwardSpeed = MathF.Abs( localVelocity.x );

			//Basically we're saying as the angle of the cars totaly velocity versus the angle of the cars forward direction approaches 90 degrees
			//Give us a big boost to the speed of our forward acceleration in order to not loose all momentum when drifting. 
			var fac = 1.0f;
			float f = MathF.Pow( (1 - angle), 4f );
			fac = fac.LerpTo( 10f, f );


			//The speed factor decreases the amount of acceleration we have depending on how fast we're currently going
			var speedFactor = (1.0f - MathF.Pow( (forwardSpeed / 2200.0f), 3.5f )).Clamp( 0.0f, 1.0f );
			//Calculate our acceleration based on our input..
			float acceleration = speedFactor * (accelerateDirection < 0.0f ? Acceleration * 0.8f : Acceleration * fac) * accelerateDirection * dt;
			//Use this to then get the impulse and apply it to our body's velocity
			var impulse = Transform.Rotation * new Vector3( acceleration, 0, 0 );
			Body.Velocity += impulse;
		}

		//Angular Damping, lerps to 5 based off grip
		var angularDamping = 0.0f;
		angularDamping = angularDamping.LerpTo( 5.0f, grip );

		Body.LinearDamping = 0;
		Body.AngularDamping = fullyGrounded ? angularDamping : 0.5f;

		//If we're on the ground
		if ( wheelsOnGround )
		{
			// Get our local velocity
			localVelocity = Transform.Rotation.Inverse * Body.Velocity;
			// Wheel speed? Presumably for the speed of the wheels
			WheelSpeed = localVelocity.x;

			// This appears to be how much we turn when are wheels are on the ground, as in how fast we can turn which is controlled by the sign of our local velocitys x speed multiplied by the turn sped and 
			// Calculate turn factor takes in our turn direction and the absolute value of our velocity this basically all effects how fast we can turn
			var turnSpeed = 25f;
			var turnAmount = turningWheelsOnGround ? (MathF.Sign( localVelocity.x ) * turnSpeed * CalculateTurnFactor( turnDirection, MathF.Abs( localVelocity.x ) ) * dt) : 0.0f;
			Body.AngularVelocity += Transform.Rotation * new Vector3( 0, 0, turnAmount );

			airRoll = 0;
			airTilt = 0;

			// Forward grip, gets lerped between 0.1 and 0.9 based on whether or not we are breaking
			var forwardGrip = 0.1f;
			forwardGrip = forwardGrip.LerpTo( 0.9f, breakInput );


			// Get the forward speed then a value representing how close to the 'top speed' we are from 0-1
			var forwardSpeed = MathF.Abs( localVelocity.x );
			var speedFactor = (forwardSpeed / MaxSpeed).Clamp( 0.0f, 1.0f );


			// If we are moving gast enough, we basically make it so our grip is a lot less the closer our velocity 
			var fac = 0.0f;
			if ( speedFactor > 0.5f )
			{
				float f = (1 - angle);
				fac = fac.LerpTo( 0.4f, f );
			}

			// Velocity damping function, we pass the current velocity ,rotation, and a vector3 with our grip and forward grip 
			Body.Velocity = VelocityDamping( Body.Velocity, Transform.Rotation, new Vector3( forwardGrip, grip - fac, 0 ), dt );
		}
		else
		{
			var s = Transform.Position + (Transform.Rotation * Transform.Position);
			var tr = Scene.Trace.Ray( s, s + Transform.Rotation.Down * 50 )
				//.Ignore( this ) TODO: FIXME
				.Run();

			canAirControl = !tr.Hit;
		}

		// TODO: AIR CONTROL

		/*
		if ( canAirControl && (airRoll != 0 || airTilt != 0) )
		{
			float offset = 50;
			var s = selfBody.Position + (rotation * selfBody.LocalMassCenter) + (rotation.Right * airRoll * offset) + (rotation.Down * (10 * Scale));
			var st = selfBody.MassCenter;
			var tr = Trace.Ray( st, st + rotation.Right * airRoll * (40 * Scale) )
				.Ignore( this )
				.Run();

			var tr2 = Scene.Trace.Ray( s, s + rotation.Up * (60 * Scale) )
				.Ignore( this )
				.Run();


			bool dampen = false;

			if ( CurrentInput.roll.Clamp( -1, 1 ) != 0 )
			{
				var force = (tr.Hit || tr2.Hit) ? 1600.0f : 100.0f;
				var roll = (tr.Hit || tr2.Hit) ? CurrentInput.roll.Clamp( -1, 1 ) : airRoll;
				body.ApplyForceAt( selfBody.MassCenter + rotation.Left * (offset * roll), (rotation.Down * roll) * (roll * (body.Mass * force)) );

				dampen = true;
			}

			if ( !tr.Hit && CurrentInput.tilt.Clamp( -1, 1 ) != 0 )
			{
				var force = 200.0f;
				body.ApplyForceAt( selfBody.MassCenter + rotation.Forward * (offset * airTilt), (rotation.Down * airTilt) * (airTilt * (body.Mass * force)) );

				dampen = true;
			}

			if ( dampen )
				body.AngularVelocity = VelocityDamping( body.AngularVelocity, rotation, 0.95f, dt );
		}
		*/

		localVelocity = Transform.Rotation.Inverse * Body.Velocity;
		Speed = localVelocity.x;
	}

	private static float CalculateTurnFactor( float direction, float speed )
	{
		var turnFactor = MathF.Min( speed / 500.0f, 1 );
		var yawSpeedFactor = 1.0f - (speed / 1000.0f).Clamp( 0, 0.6f );

		return direction * turnFactor * yawSpeedFactor;
	}

	//This function dampens our velocity
	private static Vector3 VelocityDamping( Vector3 velocity, Rotation rotation, Vector3 damping, float dt )
	{
		//Get our local velocity
		var localVelocity = rotation.Inverse * velocity;
		//Calculate the damping power
		var dampingPow = new Vector3( MathF.Pow( 1.0f - damping.x, dt ), MathF.Pow( 1.0f - damping.y, dt ), MathF.Pow( 1.0f - damping.z, dt ) );
		return rotation * (localVelocity * dampingPow);
	}
}
