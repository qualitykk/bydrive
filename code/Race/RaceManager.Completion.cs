using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class RaceManager
{
	public struct ParticipantFinishInformation
	{
		public int Placement { get; set; }
		public RaceParticipant Participant { get; set; }
		public float Time { get; set; }

		public ParticipantFinishInformation( int placement, RaceParticipant participant, float time )
		{
			Placement = placement;
			Participant = participant;
			Time = time;
		}
	}
	public IReadOnlyList<RaceParticipant> ParticipantPlacements => completionOrderedParticipants;
	public IReadOnlyList<ParticipantFinishInformation> FinishedParticipants => finishedParticipants;
	public int LastPlace => Participants.Count;

	private Dictionary<RaceParticipant, float> participantLapCompletion = new();
	private Dictionary<RaceParticipant, int> participantLapCount = new();
	private List<RaceParticipant> completionOrderedParticipants = new();
	private List<ParticipantFinishInformation> finishedParticipants = new();
	public float GetParticipantCompletion( RaceParticipant participant )
	{
		if ( !participant.IsValid() )
		{
			return 0f;
		}

		if ( !participantLapCompletion.TryGetValue( participant, out float completion ) )
		{
			return 0f;
		}

		if ( !participantLapCount.TryGetValue( participant, out int laps ) )
		{
			return completion;
		}

		return completion + laps;
	}
	public int GetParticipantLap(RaceParticipant participant)
	{
		if ( !participant.IsValid() )
		{
			return 0;
		}

		if ( !participantLapCount.TryGetValue( participant, out int laps ) )
		{
			return 0;
		}

		return laps;
	}
	public int GetParticipantPlacement(RaceParticipant participant)
	{
		return completionOrderedParticipants.IndexOf( participant ) + 1;
	}
	public ParticipantFinishInformation GetParticipantFinish(RaceParticipant participant)
	{
		return finishedParticipants.FirstOrDefault(f => f.Participant == participant );
	}
	public bool IsFinished(RaceParticipant participant)
	{
		return GetParticipantCompletion( participant ) >= MaxLaps;
	}

	public void InitialiseLapProgress(List<RaceParticipant> participants)
	{
		const int STARTING_LAP = -1;

		foreach ( var participant in participants)
		{
			participantLapCompletion.Add( participant, 0f );
			participantLapCount.Add( participant, STARTING_LAP );
		}
	}
	public void ResetLapProgress()
	{
		participantLapCompletion.Clear();
		participantLapCount.Clear();
		finishedParticipants.Clear();
	}

	public void UpdateCompletion()
	{
		foreach ( var participant in Participants )
		{
			RaceCheckpoint checkpoint = participant.LastCheckpoint;
			if ( checkpoint == null || !checkpointOrder.TryGetValue( checkpoint, out int order ) )
			{
				// Participant either hasnt passed any checkpoints or passed checkpoints arent valid this race.
				continue;
			}

			float lapCompletion = (float)order / (maxCheckpointOrder + 1);
			participantLapCompletion[participant] = lapCompletion;

			if ( IsFinished( participant ) && !finishedParticipants.Any( f => f.Participant == participant ) )
			{
				participant.OnFinished();
				ParticipantFinished( participant );

				ParticipantFinishInformation info = new( finishedParticipants.Count + 1, participant, TimeSinceRaceStart );
				finishedParticipants.Add( info );
			}
		}

		completionOrderedParticipants.Clear();
		foreach(var participant in Participants.OrderByDescending(GetParticipantCompletion).ThenBy(ClosestKeyCheckpointDistance))
		{
			completionOrderedParticipants.Add( participant );
		}
	}

	/// <summary>
	/// Gets 
	/// </summary>
	/// <param name="participant"></param>
	/// <returns></returns>
	private float ClosestKeyCheckpointDistance(RaceParticipant participant)
	{
		Vector3 position = participant.Transform.Position;
		var next = participant.NextKeyCheckpoints;
		return next.Min( n => n.Transform.Position.DistanceSquared( position ) );
	}

	private void CheckLapCount( RaceParticipant participant, RaceCheckpoint checkpoint )
	{
		if ( checkpoint == StartCheckpoint )
		{
			participantLapCount[participant]++;
		}
	}

	private void ParticipantFinished(RaceParticipant participant )
	{
		Music.Play( WinMusic );
	}
}
