using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class RaceResults : PanelComponent
{
	private IReadOnlyDictionary<RoundParticipant, int> oldScores;
	private IReadOnlyDictionary<RoundParticipant, int> scores;
	private List<KeyValuePair<RoundParticipant, int>> totalPlacement;
	private bool showScores = false;
	private bool finished => Race?.IsFinished ?? false;
	private bool ShouldDraw()
	{
		return Race != null && Race.HasParticipantFinished( GetLocalParticipant() );
	}

	private IReadOnlyList<RaceManager.ParticipantFinishInformation> GetPlacements()
	{
		//return Race.Participants
		return Race?.FinishedParticipants ?? default;
	}

	private string GetPlacementTag( int placement )
	{
		int lastPlace = Race?.LastPlace ?? 1;

		return placement switch
		{
			1 => "first",
			2 => "second",
			3 => "third",
			int p when p == lastPlace => "last",
			_ => "middle",
		};
	}

	private List<TimeTrialRecording> GetPreviousData()
	{
		return TimeTrialRecording.Read( RaceContext.CurrentDefinition.ResourcePath, RaceContext.CurrentVariables );
	}

	private void OnClickNext()
	{
		if(!finished )
		{
			oldScores = RaceContext.GetAllScores();
			Race.Finish();
			if ( RaceContext.IsMultiRace )
			{
				scores = RaceContext.GetAllScores();
				showScores = true;
				totalPlacement = scores.OrderByDescending(kv => kv.Value).ToList();
			}
		}
		else
		{
			RaceContext.NextOrStop();
		}
	}
	private string GetNextLabel()
	{
		if ( !finished ) return "Next";
		return "Done";
	}

	private void OnClickRestart()
	{
		Race.Setup(true);
	}

	protected override int BuildHash() => HashCode.Combine( GetPlacements(), GetPlacements()?.Count, ShouldDraw() );
}
