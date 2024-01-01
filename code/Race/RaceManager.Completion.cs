using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redrome;

public partial class RaceManager
{
	private Dictionary<RaceParticipant, float> participantLapCompletion = new();
	private Dictionary<RaceParticipant, int> participantLapCount = new();
	public float GetRaceCompletion( RaceParticipant participant )
	{
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
	}

	private void CheckLapCount( RaceParticipant participant, RaceCheckpoint checkpoint )
	{
		if ( checkpoint == StartCheckpoint )
		{
			participantLapCount[participant]++;
		}
	}
}
