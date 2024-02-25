using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[GameResource("Story Character", "prsn", "A character for the story mode.")]
public class CharacterDefinition : GameResource
{
	public string Name { get; set; }
	public Model WorldModel { get; set; }
	public List<VehicleDefinition> PreferredVehicles { get; set; } = new();
	public VehicleDefinition GetVehicle()
	{
		return PreferredVehicles.FirstOrDefault() ?? VehicleDefinition.GetDefault();
	}
}
