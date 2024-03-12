using Sandbox.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Bydrive;

public class TimeTrialData
{
	public const string RACE_DATA = "time_trials.json";
	public static List<TimeTrialData> Read()
	{
		return FileSystem.Data.ReadJson<List<TimeTrialData>>( RACE_DATA );
	}
	public static List<TimeTrialData> ReadForTrack(string track, Dictionary<string, string> trackVariables)
	{
		return Read()?.Where(d => d.Track == track && d.TrackVariables.SequenceEqual(trackVariables)).ToList();
	}
	public static void WriteNew(TimeTrialData data)
	{
		var current = Read() ?? new();
		current.Add( data );
		Write(current );
	}

	private static void Write(List<TimeTrialData> allData)
	{
		FileSystem.Data.WriteJson( RACE_DATA, allData );
	}
	public string PlayerName { get; set; }
	public string PlayerCountry { get; set; }
	public string Track { get; set; }
	public string Vehicle { get; set; }
	public Dictionary<string, string> TrackVariables { get; set; }
	[JsonIgnore] public float TotalTime => LapTimes.Sum();
	public List<float> LapTimes { get; set; }

	public TimeTrialData( string playerName, string track, string vehicle, Dictionary<string, string> trackVariables, List<float> lapTimes )
	{
		PlayerName = playerName;
		PlayerCountry = RegionInfo.CurrentRegion.TwoLetterISORegionName;
		Track = track;
		Vehicle = vehicle;
		TrackVariables = trackVariables;
		LapTimes = lapTimes;
	}
}
