using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[GameResource("Race Definition", "race", "Stores information about a specific race")]
[Icon("timer")]
/// <summary>
/// Information about the current race
/// </summary>
public class RaceDefinition : GameResource
{
	public string Name { get; set; }
	public SceneFile Scene { get; set; }
}
