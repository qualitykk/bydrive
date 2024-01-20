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
		public List<float> LapTimes { get; set; }
		public ParticipantFinishInformation( int placement, RaceParticipant participant, float time, List<float> lapTimes )
		{
			Placement = placement;
			Participant = participant;
			Time = time;
			LapTimes = lapTimes;
		}
	}
	public bool IsTimeTrial { get; set; }
	public IReadOnlyList<ParticipantFinishInformation> FinishedParticipants => finishedParticipants;
	public IReadOnlyDictionary<RaceParticipant, List<float>> ParticipantLapTimes => participantLapTimes;
	private List<RaceParticipant> completionOrderedParticipants = new();
	private Dictionary<RaceParticipant, List<float>> participantLapTimes = new();
	private Dictionary<RaceParticipant, int> participantLastLap = new();
	public ParticipantFinishInformation GetParticipantFinish( RaceParticipant participant )
	{
		return finishedParticipants.FirstOrDefault( f => f.Participant == participant );
	}
	public bool IsFinished( RaceParticipant participant )
	{
		const float COMPLETION_FINISH_TOLERANCE = 0.001f;
		return GetParticipantCompletion( participant ).SnapToGrid( COMPLETION_FINISH_TOLERANCE ) >= MaxLaps;
	}

	private void ParticipantFinished( RaceParticipant participant )
	{
		participant.OnFinished();

		List<float> lapTimes = new();
		float lastTime = 0;
		foreach(float time in participantLapTimes[participant] )
		{
			lapTimes.Add( time - lastTime );
			lastTime = time;
		}
		ParticipantFinishInformation info = new( finishedParticipants.Count + 1, participant, TimeSinceRaceStart, lapTimes );
		finishedParticipants.Add( info );
		Music.Play( WinMusic ); participant.OnFinished();
	}

	private void UpdateTimeSplits(RaceParticipant participant)
	{
		if ( !participantLastLap.TryGetValue(participant, out int lastLap) )
		{
			participantLastLap[participant] = 0;
		}
		int lap = GetParticipantLap( participant );
		if ( lap > 1 && GetParticipantLap(participant) != lastLap)
		{
			participantLastLap[participant] = GetParticipantLap( participant );
			if(participantLapTimes.TryGetValue(participant, out List<float> lapTimes) )
			{
				lapTimes.Add( TimeSinceRaceStart );
			}
			else
			{
				participantLapTimes.Add( participant, new() { TimeSinceRaceStart } );
			}
		}
	}
}
