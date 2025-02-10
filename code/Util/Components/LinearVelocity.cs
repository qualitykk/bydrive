using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Icon("fast_forward")]
public class LinearVelocity : Component
{
	[Property] public Vector3 LocalVelocity { get; set; }
	[Property] public Action OnHit { get; set; }
	[Property] public TagSet IgnoreTags { get; set; }
	[Property] public bool DestroyOnHit { get; set; }

	protected override void OnUpdate()
	{
		Vector3 endPos = GetNextPosition();
		var tr = Scene.Trace.Ray( WorldPosition, endPos )
							.WithoutTags( IgnoreTags )
							.Run();

		if(tr.Hit)
		{
			OnHit?.Invoke();
			if(DestroyOnHit)
			{
				Destroy();
				return;
			}
		}
		WorldPosition = tr.EndPosition;
	}

	protected virtual Vector3 GetNextPosition()
	{
		Vector3 velocity = Transform.World.VelocityToWorld( LocalVelocity ) * Time.Delta;
		return WorldPosition + velocity;
	}
}

[Icon("keyboard_return")]
public class LinearVelocityBounce : LinearVelocity
{
	const float DEFAULT_BOUNCE_DISTANCE = 8f;
	[Property] public float BounceDistance { get; set; } = DEFAULT_BOUNCE_DISTANCE;
	[Property] public Action<int> OnBounce { get;set; }
	[Property] public int MaxBounces { get; set; }
	public int BounceCount { get; private set; }
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if(MaxBounces > 0 && BounceCount > MaxBounces )
		{
			GameObject.Destroy();
		}
	}

	// For debugging purposes
	private Vector3 lastEndPos;
	private Vector3 lastNormal;
	protected override Vector3 GetNextPosition()
	{
		Vector3 velocity = Transform.World.VelocityToWorld( LocalVelocity ) * Time.Delta;

		var tr = Scene.Trace.Ray( WorldPosition, WorldPosition + velocity.Normal * BounceDistance )
			.WithoutTags(IgnoreTags)
			.Run();

		if(tr.Hit)
		{
			Vector3 endPos = tr.EndPosition;
			Vector3 normal = tr.Normal;
			lastEndPos = endPos;
			lastNormal = normal;

			LocalRotation = Rotation.LookAt( normal );
			OnBounce?.Invoke(BounceCount);
			BounceCount++;

			return endPos + normal * velocity.x * Time.Delta;
		}

		lastEndPos = default;
		lastNormal = default;
		return WorldPosition + velocity;
	}

	protected override void DrawGizmos()
	{
		base.DrawGizmos();

		Gizmo.Draw.Color = Color.Yellow;
		Gizmo.Draw.Line( Vector3.Zero, Vector3.Forward * BounceDistance );

		if ( lastNormal != default )
		{
			Vector3 endpos = Transform.Local.PointToLocal( lastEndPos );
			Vector3 normal = Transform.Local.PointToLocal( lastNormal );

			Gizmo.Draw.Color = Color.Red;
			Gizmo.Draw.LineSphere( endpos, 32f );
			Gizmo.Draw.Line( endpos, endpos + normal * BounceDistance ); ;
		}
	}
}
