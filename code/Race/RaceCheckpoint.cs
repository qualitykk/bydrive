using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class RaceCheckpoint : Component, Component.ITriggerListener
{
	const float DEFAULT_RESPAWN_POS_OFFSET = 10f;
	[Property] public List<RaceCheckpoint> NextCheckpoints { get; set; }
	[Property] public Transform RespawnTransform { get; set; } = new( new Vector3(0, 0, DEFAULT_RESPAWN_POS_OFFSET) );
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

	public Transform GetWorldRespawn() => Transform.World.ToWorld( RespawnTransform );
	protected override void DrawGizmos()
	{
		const float CHECKPOINT_RADIUS = 6f;
		const float RESPAWN_ROTATION_FORWARD = 32f;

		Color pointColor = Color.Cyan;
		Color lineColor = Color.White;
		float pointRadius = CHECKPOINT_RADIUS;

		Gizmo.Draw.Color = pointColor;
		Gizmo.Draw.SolidSphere( Vector3.Zero, pointRadius );

		if(Gizmo.IsSelected)
		{
			Gizmo.Draw.LineSphere( RespawnTransform.Position, pointRadius );
			Gizmo.Draw.Line( new Line( RespawnTransform.Position, RespawnTransform.Rotation.Forward, RESPAWN_ROTATION_FORWARD ) );
		}

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
