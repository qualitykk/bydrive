using Sandbox.Diagnostics;
using Sandbox.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bydrive;

public partial class TimeTrialRecording : ITimeTrialData
{
	public Guid Id { get; set; }
	public long SteamId { get; set; }
	public string Track { get; set; }
	public string Vehicle { get; set; }
	public Dictionary<string, string> TrackVariables { get; set; }
	public List<TimestampedVehicleInput> Inputs { get; set; }
	public List<float> LapTimes { get; set; }
	[JsonIgnore] public float TotalTime => LapTimes.Sum();

	public TimeTrialRecording( long steamId, TrackDefinition track, VehicleDefinition vehicle, Dictionary<string, string> trackVariables, List<TimestampedVehicleInput> inputs, List<float> lapTimes )
	{
		Id = Guid.NewGuid();
		SteamId = steamId;
		Track = track.ResourcePath;
		Vehicle = vehicle.ResourcePath;
		TrackVariables = trackVariables;
		Inputs = inputs;
		LapTimes = lapTimes;
	}
	public TimeTrialRecording()
	{
	}
}
