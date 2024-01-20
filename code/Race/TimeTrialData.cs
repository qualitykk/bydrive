using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bydrive;

public class TimeTrialData
{
	public const string RACE_DATA = "time_trials.json";
	public static List<TimeTrialData> Read()
	{
		return FileSystem.Data.ReadJson<List<TimeTrialData>>( RACE_DATA );
	}
	public static List<TimeTrialData> ReadForTrack(string track)
	{
		return Read()?.Where(d => d.Track == track).ToList();
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
	public string Track { get; set; }
	[JsonIgnore] public float TotalTime => LapTimes.Sum();
	public List<float> LapTimes { get; set; }

	public TimeTrialData( string playerName, string track, List<float> lapTimes )
	{
		PlayerName = playerName;
		Track = track;
		LapTimes = lapTimes;
	}
}
