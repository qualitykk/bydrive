using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bydrive;

/// <summary>
/// Information about the current race
/// </summary>
[GameResource("Race Track Definition", "track", "Stores information about a specific track")]
[Icon("timer")]
public class TrackDefinition : GameResource
{
	public static TrackDefinition[] GetAllVisible()
	{
		return ResourceLibrary.GetAll<TrackDefinition>().Where( r => r.Visible ).ToArray();
	}
	public string Name { get; set; }
	public string Group { get; set; }
	public bool Visible { get; set; }
	public SceneFile Scene { get; set; }
	public List<TrackVariable> Variables { get; set; }
	public RaceParameters Parameters { get; set; }
	public TrackMusicParameters Music { get; set; }

}
public class TrackVariable
{
	public string Key { get; set; }
	[Title( "Possible Values" )] public List<string> Values { get; set; } = new();
	public bool Required { get; set; } = true;
	public string Title { get; set; }
}
