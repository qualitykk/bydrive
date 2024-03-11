using Sandbox.Diagnostics;
using System.Diagnostics;

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
		float length = Radius * 2 + lengthExtend;

		Rotation rotation = GameObject.Parent.Transform.Rotation;
		Vector3 wheelAttachPos = GetAttachmentWorldPosition();
		Vector3 wheelExtend = wheelAttachPos - Vector3.Up * (length * Transform.Scale);


		var tr = Scene.Trace.Ray( wheelAttachPos, wheelExtend )
			.WithTag(TraceTags.SOLID)
			.WithoutTags("nocollide")
			.IgnoreGameObject(GameObject.Parent)
			.IgnoreGameObject(GameObject )
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

		const float SPRING_FORCE_PER_REMAINDER = 50.0f;

		const float DAMPER_FORCE_CONSTANT = 1.5f;
		const float DAMPER_FORCE_PER_REMAINDER = 3.0f;

		const float CORRECTION_FORCE_CONSTANT = 50f;
		const float CORRECTION_BASE_DOWN_SPEED = 1000.0f;

		Vector3 velocity = physics.GetVelocityAtPoint(wheelAttachPos);
		float traceFractionRemainder = 1.0f - tr.Fraction;

		float springForce = SPRING_FORCE_PER_REMAINDER * currentTraceRemainder;
		float springVelocity = (currentTraceRemainder - previousTraceRemainder) / dt;

		float damperForce = (DAMPER_FORCE_CONSTANT + traceFractionRemainder * DAMPER_FORCE_PER_REMAINDER) * springVelocity;

		float speed = velocity.Length;
		float speedDownwardsFraction = 0.0f;
		if( MathF.Abs( speed ) > 0.0f)
		{
			speedDownwardsFraction = MathF.Abs( MathF.Min( Vector3.Dot( velocity, rotation.Up.Normal ) / speed, 0.0f ) );
		}

		float downwardsSpeed = speedDownwardsFraction * speed;

		float correctionMultiplier = traceFractionRemainder * (downwardsSpeed / CORRECTION_BASE_DOWN_SPEED);
		float correctionForce = correctionMultiplier * CORRECTION_FORCE_CONSTANT * downwardsSpeed / dt;

		Vector3 impulse = tr.Normal * (springForce + damperForce + correctionForce) * dt;
		physics.ApplyImpulseAt(wheelAttachPos, impulse);

		if ( springForce > 300f )
		{
			//Log.Info( $"spring: {springForce}" );
		}
		if ( damperForce > 100f )
		{
			//Log.Info( $"damp: {damperForce} ({springVelocity})" );
		}
		if (correctionForce > 100f)
		{
			//Log.Info( $"corr: {correctionForce}" );
		}

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
		float length = Radius * 2 + EXTEND_GIZMO_LENGTH;

		Vector3 up = Transform.Local.PointToLocal( Transform.LocalPosition + Vector3.Up );
		Vector3 wheelAttachPos = Transform.Local.PointToLocal(initialPosition);
		Vector3 wheelExtend = wheelAttachPos - up * (length * Transform.Scale);

		Gizmo.Draw.Line( wheelAttachPos, wheelExtend );
		Gizmo.Draw.SolidSphere( wheelAttachPos, 4f );
		Gizmo.Draw.LineSphere( wheelExtend, 4f );
	}
}
