using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class RaceTimer
{
	private bool timeTrial => Race?.IsTimeTrial == true;

	private float GetRaceTime()
	{
		if ( Race == null || !Race.HasStarted )
			return 0f;

		var participant = GetLocalParticipantInstance();
		if ( Race.HasParticipantFinished( participant ) )
		{
			return Race.GetParticipantFinish( participant ).Time;
		}

		return Race.TimeSinceRaceStart;
	}

	private float GetLapTime()
	{
		if ( Race == null || !Race.HasStarted )
			return 0f;

		RaceParticipant participant = GetLocalParticipantInstance();
		if ( Race.HasParticipantFinished( participant ) )
		{
			return Race.GetParticipantFinish( participant ).LapTimes.LastOrDefault();
		}
		else if ( !Race.ParticipantLapFinishTimestamps.ContainsKey( participant ) )
		{
			return Race.TimeSinceRaceStart;
		}

		return Race.TimeSinceRaceStart - Race.ParticipantLapFinishTimestamps[participant].LastOrDefault();
	}

	private string GetClasses()
	{
		string classes = "";
		if ( Race.IsTimeTrial )
		{
			classes += "timetrial ";
		}

		return classes;
	}
	/// <summary>
	/// the hash determines if the system should be rebuilt. If it changes, it will be rebuilt
	/// </summary>
	protected override int BuildHash() => HashCode.Combine( Race?.HasStarted, Time.Now );
}
