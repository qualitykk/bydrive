using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public interface ITimeTrialData
{
	public long SteamId { get; }
	public string PlayerName { get; }
	public string Track { get; }
	public Dictionary<string, string> TrackVariables { get;  }
	public float TotalTime { get; }
}
