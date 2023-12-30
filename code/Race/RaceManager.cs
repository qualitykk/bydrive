using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redrome;

[EditorHandle( "materials/gizmo/ui.png" )]
[Icon( "flag" )]
public sealed partial class RaceManager : Component
{
	const int DEFAULT_MAX_LAPS = 3;
	public static RaceManager Current { get; private set; }
	[Property] public int MaxLaps { get; set; } = DEFAULT_MAX_LAPS;
	[Property] public RaceCheckpoint StartCheckpoint {get;set;}
	public List<RaceParticipant> Participants { get; private set; } = new();
	protected override void OnAwake()
	{
		if(Current != null)
		{
			Current.Enabled = false;
		}
		Current = this;

		OrderCheckpoints();
		OnRaceStart();
	}

	private void OnRaceStart()
	{
		Participants.Clear();
		ResetLapProgress();

		Participants = Scene.GetAllComponents<RaceParticipant>().ToList();
		InitialiseLapProgress(Participants);

		foreach(var participant in Participants)
		{
			participant.PassCheckpoint( StartCheckpoint, true );
		}
	}

	protected override void OnFixedUpdate()
	{
		foreach(var participant in Participants)
		{
			UpdateCompletion( participant );
		}
	}

	public void CheckpointPassed( RaceParticipant participant, RaceCheckpoint checkpoint )
	{
		CheckLapCount( participant, checkpoint );
	}

	protected override void DrawGizmos()
	{
		const float START_POSITION_RADIUS = 36f;
		const float CHECKPOINT_TEXT_OFFSET = 128f;
		const float TEXT_SIZE = 24f;
		const float WORLD_TEXT_SIZE = 16f;

		if(StartCheckpoint == null)
		{
			Gizmo.Draw.Color = Color.Red;
			Gizmo.Draw.ScreenText( "No start point set for race manager!", new( 200 ), size: TEXT_SIZE );
			return;
		}

		Color startColor = Color.Orange;
		Color checkpointColor = Color.Yellow;
		Vector3 startPosition = Transform.World.PointToLocal( StartCheckpoint.Transform.Position );

		Gizmo.Draw.Color = startColor;
		Gizmo.Draw.LineSphere( startPosition, START_POSITION_RADIUS );

		if ( !checkpointOrder.Any() ) return;

		foreach(var kv in checkpointOrder)
		{
			Vector3 checkpointPosition = Transform.World.PointToLocal( kv.Key.Transform.Position );
			Gizmo.Draw.Color = checkpointColor;
			Gizmo.Draw.Text( $"{kv.Key.GameObject.Name} [{kv.Value}]", kv.Key.Transform.World.WithPosition(checkpointPosition + Vector3.Up * CHECKPOINT_TEXT_OFFSET), size: WORLD_TEXT_SIZE );
		}
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(checkpointOrder, participantLapCompletion, participantLapCount);
	}
}
