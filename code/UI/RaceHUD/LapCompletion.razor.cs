﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class LapCompletion
{
	const string SOUND_LAP = "/sounds/effects/lap_complete.sound";
	const string SOUND_LAP_FINAL = "/sounds/effects/lap_complete_final.sound";
	public bool DebugCheckpoints { get; set; } = false;
	int highestLap = 1;
	public LapCompletion()
	{
		if ( Race != null )
			Race.OnReset += OnReset;
	}

	private void OnReset()
	{
		highestLap = 1;
	}

	private int GetDisplayLaps()
	{
		if ( Race == null ) return 1;

		int lap = (int)MathF.Min( GetCurrentLap(), Race?.GetMaxLaps() ?? RaceParameters.DEFAULT_MAX_LAPS );
		if ( lap > highestLap )
		{
			RaceNotifications.AddLineLocal( new( $"Completed Lap {lap - 1}!", UI.Colors.Notification.Success, 10f, "sports_score" ) );
			highestLap = lap;

			if(highestLap == Race?.GetMaxLaps())
			{
				SoundManager.Instance.Play( SOUND_LAP_FINAL, GameSoundChannel.Effect );
			}
			else
			{
				SoundManager.Instance.Play( SOUND_LAP, GameSoundChannel.Effect );
			}
		}
		return highestLap;
	}
	private int GetCurrentLap()
	{
		return GetLocalParticipant()?.GetLap() ?? 0;
	}
	private float GetCurrentCompletion()
	{
		return GetLocalParticipant()?.GetCompletion() ?? -1f;
	}
	private bool InLastLap()
	{
		return GetDisplayLaps() == (Race?.GetMaxLaps() ?? RaceParameters.DEFAULT_MAX_LAPS);
	}
	/// <summary>
	/// the hash determines if the system should be rebuilt. If it changes, it will be rebuilt
	/// </summary>
	protected override int BuildHash() => HashCode.Combine( GetCurrentCompletion(), highestLap, DebugCheckpoints);
}
