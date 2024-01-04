using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class RaceManager
{
	public IReadOnlyList<RaceParticipant> FinishedParticipants => finishedParticipants;

	private Dictionary<RaceParticipant, float> participantLapCompletion = new();
	private Dictionary<RaceParticipant, int> participantLapCount = new();
	private List<RaceParticipant> finishedParticipants = new();
	public float GetRaceCompletion( RaceParticipant participant )
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
	public bool IsFinished(RaceParticipant participant)
	{
		return GetRaceCompletion( participant ) >= MaxLaps;
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

	public void UpdateCompletion( RaceParticipant participant )
	{
		RaceCheckpoint checkpoint = participant.LastCheckpoint;
		if ( checkpoint == null || !checkpointOrder.TryGetValue( checkpoint, out int order ) )
		{
			// Participant either hasnt passed any checkpoints or passed checkpoints arent valid this race.
			return;
		}

		float lapCompletion = (float)order / (maxCheckpointOrder + 1);
		participantLapCompletion[participant] = lapCompletion;

		if ( IsFinished( participant ) && !finishedParticipants.Contains(participant) )
		{
			participant.OnFinished();
			ParticipantFinished( participant );
			finishedParticipants.Add( participant );
		}
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
