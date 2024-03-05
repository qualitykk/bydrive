using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public struct RaceSetup
{
	public TrackDefinition Track { get; set; }
	public RaceParameters Parameters { get; set; }
	public Dictionary<string, string> Variables { get; set; }
	public TrackMusicParameters Music { get; set; }

	public RaceSetup( TrackDefinition track, RaceParameters parameters = null, Dictionary<string, string> variables = null, TrackMusicParameters music = null )
	{
		Track = track;
		Parameters = parameters;
		Variables = variables;
		Music = music;
	}

	public override string ToString()
	{
		string variablesString = Variables != null ? string.Join( ',', Variables.Select( kv => $"{kv.Key}={kv.Value}" ) ) : "NONE";
		return $"{Track.Name} ({variablesString})";
	}
}
