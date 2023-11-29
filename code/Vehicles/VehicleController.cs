using Sandbox;
namespace Redrome;
public sealed partial class VehicleController : BaseComponent
{
	#region Stats
	// TODO: Get these from vehicle stats
	[Property] float MaxSpeed { get; set; } = 512f;
	[Property] float Acceleration { get; set; } = 256f;
	[Property] float BreakSpeed { get; set; } = 384f;
	[Property] float TurnSpeed { get; set; } = 90f;
	[Property, Title("Physics Body")] PhysicsComponent PhysicsComponent { get; set; }
	PhysicsBody vehicleBody => PhysicsComponent?.GetBody();
	#endregion
	private GameTransform transform => GameObject.Transform;
	float wishVelocity;
	protected override void OnEnabled()
	{
	}
	protected override void OnUpdate()
	{
		// For now, just use modified PlayerController code
		BuildInput();

		Move();
	}

	private void Move()
	{
	}
	
}
