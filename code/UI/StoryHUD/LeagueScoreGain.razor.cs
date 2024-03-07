using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class LeagueScoreGain
{
	const float SCORE_LERP_TIME = 1f;

	public static LeagueScoreGain Instance { get; set; }
	public static void Show(int oldScore, int score)
	{
		Instance?.ShowGain( oldScore, score );
	}

	bool show => gainActive || rankupActive;
	bool gainActive;
	int startScore;
	int displayScore;
	int endScore;

	bool rankupActive;
	LeagueRank startRank;
	LeagueRank displayNextRank => SaveFile.GetNextRank(displayScore);
	LeagueRank endRank;
	int maxScore => displayNextRank.Points;
	RealTimeUntil timeUntilAnimationEnd;
	protected override void OnEnabled()
	{
		base.OnEnabled();
		Instance = this;
	}
	protected override void OnDisabled()
	{
		base.OnDisabled();
		if ( Instance == this )
			Instance = null;
	}
	public void ShowGain( int oldScore, int score )
	{
		gainActive = true;
		startScore = oldScore;
		endScore = score;
		startRank = SaveFile.GetScoreRank( startScore );
		endRank = SaveFile.GetScoreRank( score );
		timeUntilAnimationEnd = SCORE_LERP_TIME;
	}
	public void ShowRankup(LeagueRank from, LeagueRank to)
	{
		rankupActive = true;
		startRank = from;
		endRank = to;
	}
	protected override void OnUpdate()
	{
		if ( !show )
		{
			UI.MakeMenuInactive( Panel );
			return;
		}

		UI.MakeMenu( Panel );

		if(gainActive)
		{
			displayScore = MathX.Lerp( startScore, endScore, MathF.Pow((float)timeUntilAnimationEnd.Fraction,2.5f) ).CeilToInt();
		}

		if(Input.Pressed(InputActions.DIALOG_SKIP) || Input.Pressed(InputActions.USE))
		{
			if(gainActive && timeUntilAnimationEnd && !startRank.Equals( endRank ) )
			{
				ShowRankup( startRank, endRank );
				gainActive = false;

				return;
			}

			gainActive = false;
			rankupActive = false;
		}
	}

	protected override int BuildHash()
	{
		return HashCode.Combine(RealTime.Now, displayScore);
	}

	[ConCmd("ui_show_scoregain")]
	private static void Command_Show(int oldscore, int score)
	{
		Show( oldscore, score );
	}
}
