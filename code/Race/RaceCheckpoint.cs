using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class RaceCheckpoint : Component, Component.ITriggerListener
{
	const float DEFAULT_RESPAWN_POS_OFFSET = 32f;
	[Property] public List<RaceCheckpoint> NextCheckpoints { get; set; }
	[Property] public Vector3 RespawnPosition { get; set; } = new Vector3( 0, 0, DEFAULT_RESPAWN_POS_OFFSET );
	[Property] public Angles RespawnRotation { get; set; } = Angles.Zero;
	public Transform RespawnTransform => new( RespawnPosition, RespawnRotation.ToRotation() );
	/// <summary>
	/// Do you HAVE to pass this checkpoint before passing onto the next key checkpoint? Prevents unintended shortcuts.
	/// </summary>
	[Property] public bool IsRequired { get; set; } = true;
	public IReadOnlyList<RaceCheckpoint> PreviousCheckpoints => previousCheckpointsReferences;
	private List<RaceCheckpoint> previousCheckpointsReferences = new();
	internal static void RebuildCheckpointReferences()
	{
		IEnumerable<RaceCheckpoint> checkpoints = GameManager.ActiveScene.GetAllComponents<RaceCheckpoint>();
		foreach(var checkpoint in checkpoints )
		{
			checkpoint.previousCheckpointsReferences.Clear();
		}

		foreach(var checkpoint in checkpoints )
		{
			if ( checkpoint.NextCheckpoints != null && checkpoint.NextCheckpoints.Any() )
			{
				foreach ( var next in checkpoint.NextCheckpoints )
				{
					next.previousCheckpointsReferences.Add( checkpoint );
				}
			}
		}
	}
	protected override void OnFixedUpdate()
	{
		if(!previousCheckpointsReferences.Any() && RaceContext.FinishedLoading )
		{
			RebuildCheckpointReferences();
		}
	}
	protected override void OnEnabled()
	{
		if(RaceContext?.FinishedLoading == true)
			RebuildCheckpointReferences();
	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		var participant = other.Components.GetInAncestorsOrSelf<RaceParticipant>();
		if ( participant == null )
		{
			return;
		}

		participant.PassCheckpoint( this );
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
	}

	public Transform GetWorldRespawn() => Transform.World.ToWorld( RespawnTransform ).EnsureNotNaN();

	protected override void DrawGizmos()
	{
		const float CHECKPOINT_RADIUS = 18f;
		const float RESPAWN_RADIUS = 6f;
		const float RESPAWN_ROTATION_FORWARD = 32f;

		Color pointColor = IsRequired ? Color.Red : Color.Orange;
		Color lineColor = Color.White;

		Gizmo.Draw.Color = pointColor;
		Gizmo.Draw.SolidSphere( Vector3.Zero, CHECKPOINT_RADIUS );

		if(Gizmo.IsSelected && IsRequired)
		{
			Gizmo.Draw.LineSphere( RespawnTransform.Position, RESPAWN_RADIUS );
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
