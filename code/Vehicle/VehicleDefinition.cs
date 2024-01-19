using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[GameResource( "Vehicle Definition", "vehicle", "A Bydrive Vehicle" )]
[Icon( "electric_car" )]
public class VehicleDefinition : GameResource, IPrefabProvider
{
	public string Title { get; set; }
	public string Description { get; set; }
	public bool Hidden { get; set; }
	[Category("Preview")] public Model PreviewModel { get; set; }
	[Category("Preview")] public Vector3 PreviewPosition { get; set; }
	[Category("Preview")] public Angles PreviewAngles { get; set; }
	public VehicleStats Stats { get;set; }
	public PrefabFile Prefab { get; set; }
}
