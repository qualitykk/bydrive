using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public static class TimeTrialLeaderboard
{
	private static readonly string[] supportedLeaderboardTracks = new[] { "main_backstreets", "main_forest" };
	private static readonly Dictionary<string, string> supportedTrackToLeaderboard = new()
	{
		{"main_backstreets", "t_01" },
		{"main_forest", "t_02" },
	};
	public static bool HasLeaderboard(string track)
	{
		return supportedLeaderboardTracks.Contains(track);
	}
	private static string GetLeaderboardName(string track, Dictionary<string, string> trackVariables)
	{
		if ( !HasLeaderboard( track ) )
			throw new InvalidOperationException( $"Track {track} has no leaderboard!" );

		if ( !trackVariables.TryGetValue( "route", out string route ) )
			return track;

		string leaderboardName = supportedTrackToLeaderboard[track];
		leaderboardName += "_" + string.Join( "", route.Split( '_', ' ' ).Select( part => part.ElementAt( 0 ) ) );

		return leaderboardName;
	}
	public static List<ITimeTrialData> GetTimes(string track, Dictionary<string, string> trackVariables, string leaderboardGroup = "global", int limit = 10)
	{
		List<ITimeTrialData> data = new();

		if(HasLeaderboard(track))
		{
			data.AddRange( GetLeaderboardTimes( track, trackVariables, leaderboardGroup, limit ) );
		}

		var localRecordings = GetRecordings( track, trackVariables );
		if(localRecordings.Any())
		{
			data.AddRange( localRecordings );
		}

		return data.Take(limit).ToList();
	}

	public static List<TimeTrialLeaderboardEntry> GetLeaderboardTimes( string track, Dictionary<string, string> trackVariables, string group = "global", int limit = 10 )
	{
		if(!HasLeaderboard(track))
			return new();

		var leaderboard = Sandbox.Services.Leaderboards.Get( GetLeaderboardName(track, trackVariables) );
		leaderboard.Group = group;
		leaderboard.MaxEntries = limit;
		leaderboard.Refresh();

		List<TimeTrialLeaderboardEntry> entries = new();

		if(leaderboard.Entries.Any())
		{
			foreach(var entry in leaderboard.Entries)
			{
				entries.Add( new(entry, track, trackVariables));
			}
		}

		return entries;
	}

	public static List<TimeTrialRecording> GetRecordings(string track, Dictionary<string, string> trackVariables)
	{
		// TODO: Platform for time trial replays?

		return TimeTrialRecording.Read( track, trackVariables );
	}

	public static void SubmitTime(string track, Dictionary<string, string> trackVariables, float time)
	{
		const float CHEAT_TIME_LIMIT = 35f;
		if ( !HasLeaderboard( track ) ) return;
		if ( time <= CHEAT_TIME_LIMIT ) return;

		string leaderboardName = GetLeaderboardName( track, trackVariables );
		double leaderboardAmount = time;

		Sandbox.Services.Stats.SetValue( leaderboardName, leaderboardAmount );
	}
}
public class TimeTrialLeaderboardEntry : ITimeTrialData
{
	public long SteamId { get; }
	public string PlayerName { get; }
	public string Track { get; }
	public Dictionary<string, string> TrackVariables { get; }
	public float TotalTime { get; }

	public TimeTrialLeaderboardEntry( Sandbox.Services.Leaderboards.Entry entry, string track, Dictionary<string, string> trackVariables)
	{
		SteamId = entry.SteamId;
		PlayerName = entry.DisplayName;
		Track = track;
		TrackVariables = trackVariables;
		TotalTime = (float)entry.Value;
	}
}
