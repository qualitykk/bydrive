using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

/// <summary>
/// Keeps the object a certain distance away from the floor.
/// </summary>
[Icon( "keyboard_double_arrow_up" )]
public sealed class HoverObject : Component
{
	const float DEFAULT_DISTANCE = 32f;
	const float DEFAULT_TRACE_DISTANCE = 1024f;
	[Property] public TagSet IgnoreTags { get; set; }
	[Property] public float Distance { get; set; } = DEFAULT_DISTANCE;
	[Property] public float TraceDistance { get; set; } = DEFAULT_TRACE_DISTANCE;

	protected override void OnUpdate()
	{
		var tr = TraceHover();
		if(tr.Hit)
		{
			WorldPosition = WorldPosition.WithZ( tr.EndPosition.z + Distance );
		}
	}

	private SceneTraceResult TraceHover()
	{
		const float DISTANCE_ADD = 32f;
		Vector3 startPos = WorldPosition;
		Vector3 direction = WorldRotation.Down;
		return Scene.Trace.Ray( new Ray( startPos, direction ), MathF.Max(TraceDistance, Distance + DISTANCE_ADD) )
							.WithoutTags( IgnoreTags )
							.Run();
	}
}
