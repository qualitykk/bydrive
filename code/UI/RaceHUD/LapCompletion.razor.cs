using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class LapCompletion
{
	[Property] public bool DebugCheckpoints { get; set; } = false;
	int highestLap = 1;
	private int GetDisplayLaps()
	{
		if ( Race == null ) return 1;

		int lap = (int)MathF.Min( GetCurrentLap(), Race?.GetMaxLaps() ?? RaceParameters.DEFAULT_MAX_LAPS );
		if ( lap > highestLap )
		{
			RaceNotifications.Add( new( $"Completed Lap {lap - 1}!", UI.Colors.Notification.Success, 10f, "sports_score" ) );
			highestLap = lap;
		}
		return highestLap;
	}
	private int GetCurrentLap()
	{
		return GetLocalParticipantInstance()?.GetLap() ?? 0;
	}
	private float GetCurrentCompletion()
	{
		return GetLocalParticipantInstance()?.GetCompletion() ?? -1f;
	}
	private bool InLastLap()
	{
		return GetDisplayLaps() == (Race?.GetMaxLaps() ?? RaceParameters.DEFAULT_MAX_LAPS);
	}
	/// <summary>
	/// the hash determines if the system should be rebuilt. If it changes, it will be rebuilt
	/// </summary>
	protected override int BuildHash() => HashCode.Combine( Game.ActiveScene, GetCurrentCompletion(), DebugCheckpoints, CameraManager.Instance.CurrentCameraMode );
}
