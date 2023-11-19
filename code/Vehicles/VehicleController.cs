using Sandbox;
namespace Redrome;
public sealed class VehicleController : BaseComponent
{
	#region Stats
	// TODO: Get these from vehicle stats
	[Property] float MaxSpeed { get; set; } = 512f;
	[Property] float Acceleration { get; set; } = 256f;
	[Property] float BreakSpeed { get; set; } = 384f;
	[Property] float TurnSpeed { get; set; } = 90f;
	[Property] GameObject Body { get; set; }
	#endregion
	private float driveForwardYaw;
	private GameTransform transform => GameObject.Transform;
	float wishVelocity;
	public override void OnEnabled()
	{
		driveForwardYaw = transform.Rotation.Yaw();
	}
	public override void Update()
	{
		// For now, just use modified PlayerController code
		BuildInput();

		Move();
	}

	private void Move()
	{
		//if ( Body == null ) return;
		Angles steeredAngles = transform.Rotation.Angles().WithYaw( driveForwardYaw );

		Vector3 velocity = wishVelocity * steeredAngles.Forward;
		transform.Position += velocity * Time.Delta;

		// EXTREMELY simple vehicle turn, make this a bit more realistic
		transform.Rotation = steeredAngles.ToRotation();
	}
	public void BuildInput()
	{
		BuildAccelerationInput();
		BuildDirectionInput();
	}

	private void BuildAccelerationInput()
	{
		const float FRICTION_SPEED_LOSS = 200f;
		const float MAX_SPEED_REVERSE = -90f;

		float addedVelocity = 0;
		if ( Input.Down( "Forward" ) ) addedVelocity = Acceleration;
		if ( Input.Down( "Backward" ) ) addedVelocity = -BreakSpeed;

		if ( addedVelocity == 0 )
		{
			float friction = FRICTION_SPEED_LOSS * Time.Delta;
			if ( wishVelocity <= friction )
				wishVelocity = 0;
			else
				wishVelocity -= friction;

			return;
		}
		wishVelocity += addedVelocity * Time.Delta;

		if ( wishVelocity > MaxSpeed )
		{
			wishVelocity = MaxSpeed;
		}
		else if ( wishVelocity < MAX_SPEED_REVERSE )
		{
			wishVelocity = MAX_SPEED_REVERSE;
		}
	}

	private void BuildDirectionInput()
	{
		float addedTurn = 0;
		if ( Input.Down( "Left" ) ) addedTurn += TurnSpeed;
		if ( Input.Down( "Right" ) ) addedTurn -= TurnSpeed;

		driveForwardYaw += addedTurn * Time.Delta ;
	}
}
