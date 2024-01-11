using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Icon("fast_forward")]
public class LinearVelocity : Component
{
	[Property] public Vector3 LocalVelocity { get; set; }

	protected override void OnUpdate()
	{
		Vector3 endPos = GetNextPosition();
		var tr = Scene.Trace.Ray( Transform.Position, endPos )
							.Run();

		Transform.Position = tr.Hit ? tr.HitPosition : endPos;
	}

	protected virtual Vector3 GetNextPosition()
	{
		Vector3 velocity = LocalVelocity * Transform.LocalRotation;
		return Transform.Position + Transform.Local.VelocityToWorld( velocity * Time.Delta);
	}
}

[Icon("keyboard_return")]
public class LinearVelocityBounce : LinearVelocity
{
	const float DEFAULT_BOUNCE_DISTANCE = 8f;
	[Property] public TagSet IgnoreTags { get; set; }
	[Property] public float BounceDistance { get; set; } = DEFAULT_BOUNCE_DISTANCE;
	[Property] public Action<int> OnBounce { get;set; }
	[Property] public int MaxBounces { get; set; }
	public int BounceCount { get; private set; }
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if(MaxBounces > 0 && BounceCount > MaxBounces )
		{
			Destroy();
		}
	}
	protected override Vector3 GetNextPosition()
	{
		Vector3 worldVelocity = Transform.World.VelocityToWorld( LocalVelocity );
		Vector3 velocity = LocalVelocity * Transform.LocalRotation * Time.Delta;

		var tr = Scene.Trace.Ray( new(Transform.Position, worldVelocity ), BounceDistance )
			.WithoutTags(IgnoreTags)
			.Run();

		Vector3 bounceVelocity = Vector3.Zero;
		if(tr.Hit)
		{
			bounceVelocity = tr.Normal.Normal * velocity.x;
			OnBounce?.Invoke(BounceCount);
			BounceCount++;
		}

		return Transform.LocalPosition + velocity + bounceVelocity;
	}
}
