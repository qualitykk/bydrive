using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;
public partial class CompletionPage
{
	private LeagueRank GetCurrentRank()
	{
		return CurrentSave?.GetRank() ?? SaveFile.DefaultRank;
	}

	private LeagueRank GetNextRank()
	{
		return CurrentSave?.GetNextRank() ?? SaveFile.Ranks.Last();
	}
	private string GetAvatar()
	{
		return $"avatarbig:{Game.SteamId}";
	}

	void OnClickBack()
	{
		this.Navigate( "/" );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( CurrentSave, CurrentSave?.Score );
	}
}
