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
[GameResource("Race Definition", "race", "Stores information about a specific race")]
[Icon("timer")]
public class RaceDefinition : GameResource
{
	public static RaceDefinition[] GetAllVisible()
	{
		return ResourceLibrary.GetAll<RaceDefinition>().Where( r => r.Visible ).ToArray();
	}
	public string Name { get; set; }
	public string Group { get; set; }
	public bool Visible { get; set; }
	[Obsolete, Hide, JsonIgnore] public bool Hidden => !Visible;
	public SceneFile Scene { get; set; }
	public PrefabFile Prefab { get; set; }
	[HideIf("Prefab", null)] public string MapName { get;set; }
	public RaceParameters Parameters { get; set; }
	
	public bool UseScene()
	{
		return Prefab == default;
	}
}
