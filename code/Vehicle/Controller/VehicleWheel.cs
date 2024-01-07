using Sandbox.Diagnostics;

namespace Bydrive;

[Category( "Vehicle" )]
public sealed class VehicleWheel : Component
{
	private const float DEFAULT_SIZE = 14f;
	private const float DEFAULT_WIDTH = 8f;
	[Property] public float Radius { get; set; } = DEFAULT_SIZE;
	[Property] public float Width { get; set; } = DEFAULT_WIDTH;
	[Property] public bool IsDriving { get; set; } = true;
	[Property] public bool IsTurning { get; set; } = true;
	private Vector3 initialPosition;
	private float previousTraceRemainder;
	private float currentTraceRemainder;

	protected override void OnStart()
	{
		initialPosition = Transform.LocalPosition;
	}
	public Vector3 GetAttachmentWorldPosition()
	{
		return GameObject.Parent.Transform.World.PointToWorld( initialPosition);
	}
	public bool Raycast( float lengthExtend, bool doPhysics, float dt )
	{
		if( !GameObject.Components.TryGet<Rigidbody>( out var physicsComponent, FindMode.Enabled | FindMode.InParent | FindMode.InSelf ) )
		{
			return false;
		}

		PhysicsBody physics = physicsComponent.PhysicsBody;
		float length = Radius + lengthExtend;

		Rotation rotation = GameObject.Parent.Transform.Rotation;
		Vector3 wheelAttachPos = GetAttachmentWorldPosition();
		Vector3 wheelExtend = wheelAttachPos - rotation.Up * (length * Transform.Scale);

		var tr = Scene.Trace.Ray( wheelAttachPos, wheelExtend )
			.WithoutTags("nocollide")
			.IgnoreGameObject(GameObject.Parent)
			.IgnoreGameObject(GameObject )
			.UsePhysicsWorld()
			.Run();

		if ( !tr.Hit || !doPhysics )
		{
			return tr.Hit;
		}

		// i have no clue what this does
		// right now its mostly a copy of the old vehicle entity that used to be in the sandbox gamemode
		// TODO: Make this more like drome racers

		previousTraceRemainder = currentTraceRemainder;
		currentTraceRemainder = (length * Transform.Scale.z) - tr.Distance;

		const float SPRING_MAGIC1 = 50.0f;

		const float DAMPER_MAGIC1 = 1.5f;
		const float DAMPER_MAGIC2 = 3.0f;

		const float CORRECTION_MAGIC1 = 1000.0f;
		const float CORRECTION_MAGIC2 = 50.0f;

		Vector3 velocity = physics.GetVelocityAtPoint(wheelAttachPos);
		float traceFractionRemainder = 1.0f - tr.Fraction;

		float springForce = SPRING_MAGIC1 * currentTraceRemainder;
		float springVelocity = (currentTraceRemainder - previousTraceRemainder) / dt;

		float damperForce = (DAMPER_MAGIC1 + traceFractionRemainder * DAMPER_MAGIC2) * springVelocity;

		float speed = velocity.Length;
		float speedDownwardsFraction = 0.0f;
		if( MathF.Abs( speed ) > 0.0f)
		{
			speedDownwardsFraction = MathF.Abs( MathF.Min( Vector3.Dot( velocity, rotation.Up.Normal ) / speed, 0.0f ) );
		}

		float downwardsSpeed = speedDownwardsFraction * speed;

		float correctionMultiplier = traceFractionRemainder * (downwardsSpeed / CORRECTION_MAGIC1);
		float correctionForce = correctionMultiplier * CORRECTION_MAGIC2 * downwardsSpeed / dt;

		Vector3 impulse = tr.Normal * (springForce + damperForce + correctionForce) * dt;
		physics.ApplyImpulseAt(wheelAttachPos, impulse);
		//Log.Info( $"{springForce} | {damperForce} | {correctionForce}" );

		return true;
	}

	protected override void DrawGizmos()
	{
		Gizmo.Draw.Color = Color.Magenta;

		Vector3 direction = Vector3.Left * Width;
		Gizmo.Draw.SolidCylinder( -direction, direction, Radius );

		const float FORWARD_HELPER_SIZE = 4f;
		Gizmo.Draw.Line( Vector3.Zero, direction * FORWARD_HELPER_SIZE );

		const float EXTEND_GIZMO_LENGTH = 20f;

		Gizmo.Draw.Color = Color.Red;
		float length = Radius + EXTEND_GIZMO_LENGTH;

		Rotation rotation = GameObject.Parent.Transform.Rotation;
		Vector3 wheelAttachPos = Transform.Local.PointToLocal(initialPosition);
		Vector3 wheelExtend = wheelAttachPos - rotation.Up * (length * Transform.Scale);

		Gizmo.Draw.Line( wheelAttachPos, wheelExtend );
	}
}
