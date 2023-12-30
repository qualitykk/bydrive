using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redrome;

public class RaceCheckpoint : Component
{
	[Property] public List<RaceCheckpoint> NextCheckpoints { get; set; }

	protected override void DrawGizmos()
	{
		const float NEXT_CHECKPOINT_RADIUS = 4f;
		if ( !Gizmo.IsSelected ) return;
		if ( NextCheckpoints == null || !NextCheckpoints.Any() ) return;

		Color drawColor = Color.Cyan;

		foreach(var checkpoint in NextCheckpoints)
		{
			if ( !checkpoint.IsValid() ) continue;

			Vector3 nextPosition = Transform.World.PointToLocal( checkpoint.Transform.Position );
			Gizmo.Draw.Line( Vector3.Zero, nextPosition );
			Gizmo.Draw.SolidSphere( nextPosition, NEXT_CHECKPOINT_RADIUS );
		}
	}
}
