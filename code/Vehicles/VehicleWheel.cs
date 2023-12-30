namespace Redrome;

public sealed class VehicleWheel : Component
{
	private const float DEFAULT_SIZE = 14f;
	private const float DEFAULT_WIDTH = 8f;
	[Property] public float Radius { get; set; } = DEFAULT_SIZE;
	[Property] public float Width { get; set; } = DEFAULT_WIDTH;
	[Property] public bool IsDriving { get; set; } = true;
	[Property] public bool IsTurning { get; set; } = true;
	private float _previousLength;
	private float _currentLength;
	public bool Raycast( float length, bool doPhysics, float dt )
	{
		if( !GameObject.Components.TryGet<Rigidbody>( out var physicsComponent ) )
		{
			return false;
		}

		PhysicsBody physics = physicsComponent.PhysicsBody;
		length += Radius;

		var rotation = Transform.Rotation;
		var wheelAttachPos = physics.MassCenter;
		var wheelExtend = wheelAttachPos - rotation.Up * (length * Transform.Scale);

		var tr = Scene.Trace.PhysicsTrace.Ray( wheelAttachPos, wheelExtend )
			.WithoutTags("Vehicle") // HACK: No .Ignore() yet, force it to ignore other vehicles for now
			.Run();

		if ( !tr.Hit || !doPhysics )
		{
			return tr.Hit;
		}

		_previousLength = _currentLength;
		_currentLength = (length * Transform.Scale.x) - tr.Distance;

		// i have no clue what this does
		var springVelocity = (_currentLength - _previousLength) / dt;
		var springForce = 50.0f * _currentLength;
		var damperForce = (1.5f + (1.0f - tr.Fraction) * 3.0f) * springVelocity;
		var velocity = physics.Velocity;
		var speed = velocity.Length;
		var speedDot = MathF.Abs( speed ) > 0.0f ? MathF.Abs( MathF.Min( Vector3.Dot( velocity, rotation.Up.Normal ) / speed, 0.0f ) ) : 0.0f;
		var speedAlongNormal = speedDot * speed;
		var correctionMultiplier = (1.0f - tr.Fraction) * (speedAlongNormal / 1000.0f);
		var correctionForce = correctionMultiplier * 50.0f * speedAlongNormal / dt;

		physics.Velocity += tr.Normal * (springForce + damperForce + correctionForce) * dt;
		Log.Info( physics.Velocity );

		return true;
	}

	protected override void DrawGizmos()
	{
		if(!Gizmo.IsSelected) return;

		Gizmo.Draw.Color = Color.Magenta;

		Vector3 direction = Vector3.Left * Width;
		Gizmo.Draw.SolidCylinder( -direction, direction, Radius );

		const float FORWARD_HELPER_SIZE = 4f;
		Gizmo.Draw.Line( Vector3.Zero, direction * FORWARD_HELPER_SIZE );
	}
}
