using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class SaveFile
{
	public struct LeagueRank
	{
		public int Points { get;set; }
		public string Title { get;set; }
		public string Icon { get;set; }
		public LeagueRank( int points, string title, string icon = "" )
		{
			Points = points;
			Title = title;
			Icon = icon;
		}

		public override string ToString()
		{
			return Title;
		}
	}
	public static readonly LeagueRank DefaultRank = new( 0, "Amateur" );
	public static readonly List<LeagueRank> Ranks = new()
	{
		DefaultRank,
		new( 150,   "Rookie" ),
		new( 500,   "Prospect" ),
		new( 1000,  "Professional" ),
		new( 2000,  "Contender" ),
		new( 3400,  "Challenger" ),
		new( 5000,  "Racer" ), // TODO: Come up with something more epic sounding
		new( 7000,  "Master" ),
		new( 8200,  "Legend" ),
		new( 10000, "Champion" ),
	};
	public int Score { get; set; }
	public LeagueRank GetRank()
	{
		return Ranks.LastOrDefault( rank => rank.Points <= Score );
	}
	public LeagueRank GetNextRank()
	{
		var rank = Ranks.FirstOrDefault( rank => rank.Points > Score );
		if (string.IsNullOrWhiteSpace(rank.Title))
			return Ranks.Last();

		return rank;
	}

	[ConCmd("st_league_setscore")]
	private static void Command_SetScore(int amount)
	{
		if ( !Story.Active )
		{
			Log.Warning( "No save file loaded!" );
			return;
		}

		CurrentSave.Score = amount;
	}
}
