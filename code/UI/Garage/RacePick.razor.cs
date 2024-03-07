using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class RacePick
{
	RaceSetupManager manager => RaceSetupManager.Current;
	ChallengeDefinition currentChallenge => RaceSetupManager.Current?.SelectedChallenge;
	private List<string> GetRewardLines()
	{
		List<string> lines = new();
		string display = currentChallenge.RewardDisplay;
		if ( !string.IsNullOrWhiteSpace( display ) )
		{
			lines.AddRange( display.Split( "\n" ) );
		}

		return lines;
	}

	void OnClickChallenge( ChallengeDefinition challenge )
	{
		manager.SelectedChallenge = challenge;
	}

	private Dictionary<ChallengeDefinition, ChallengeState> GetChallenges()
	{
		if ( !Story.Active )
		{
			return ChallengeDefinition.All.ToDictionary( def => def, def => ChallengeState.InProgress );
		}
		else
		{
			return CurrentSave.GetUnlockedChallenges().OrderBy( kv => kv.Value ).ToDictionary( kv => kv.Key, kv => kv.Value );
		}
	}

	private string GetTrackNames()
	{
		if ( currentChallenge.IsSingle )
		{
			return currentChallenge.Races.FirstOrDefault().Track?.Name;
		}

		return "Multi-Race";
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( currentChallenge );
	}
}
