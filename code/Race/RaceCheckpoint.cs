using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class RaceCheckpoint : Component, Component.ITriggerListener
{
	[Property] public List<RaceCheckpoint> NextCheckpoints { get; set; }
	/// <summary>
	/// Do you HAVE to pass this checkpoint before passing onto the next key checkpoint? Prevents unintended shortcuts.
	/// </summary>
	[Property] public bool IsRequired { get; set; } = true;
	public void OnTriggerEnter( Collider other )
	{
		if ( !other.Components.TryGet( out RaceParticipant completion, FindMode.Enabled | FindMode.InParent | FindMode.InSelf ) )
		{
			return;
		}

		completion.PassCheckpoint( this );
	}

	public void OnTriggerExit( Collider other )
	{

	}

	protected override void DrawGizmos()
	{
		const float CHECKPOINT_RADIUS = 16f;
		//if ( !Gizmo.IsSelected ) return;

		Color pointColor = Color.Cyan;
		float pointRadius = CHECKPOINT_RADIUS;
		Color lineColor = Color.White;

		Gizmo.Draw.Color = pointColor;
		Gizmo.Draw.SolidSphere( Vector3.Zero, pointRadius );

		Gizmo.Draw.Color = lineColor;

		if ( NextCheckpoints == null || !NextCheckpoints.Any() ) return;

		foreach ( var checkpoint in NextCheckpoints)
		{
			if ( !checkpoint.IsValid() ) continue;

			Vector3 nextPosition = Transform.World.PointToLocal( checkpoint.Transform.Position );
			Gizmo.Draw.Line( Vector3.Zero, nextPosition );
		}
	}
}
