using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Icon( "fact_check" )]
public class DefinitionVehicleStats : VehicleStatsProvider
{
	[Property, ResourceReference] public VehicleDefinition Definition { get; set; }
	public override VehicleStats GetStats()
	{
		return Definition?.Stats ?? new();
	}
}
