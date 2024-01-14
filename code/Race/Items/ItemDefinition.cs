using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[GameResource("Item", "item", "An item to be used in ByDrive.")]
public class ItemDefinition : GameResource, IPrefabProvider
{
	public string Title { get; set; }
	[ResourceType(".png")] public string Icon { get; set; }
	public PrefabFile Prefab { get; set; }
}
