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
	public Dictionary<string, Texture> ExpressionImages { get; set; } = new();
	public List<VehicleDefinition> PreferredVehicles { get; set; } = new();
	public Texture GetDialogImage(string expression = "")
	{
		if ( string.IsNullOrEmpty( expression ) )
			expression = "base";

		if ( ExpressionImages == null )
			return Texture.White;

		if ( ExpressionImages.TryGetValue( expression, out var image ) )
			return image;

		return Texture.White;
	}
	public VehicleDefinition GetVehicle()
	{
		return PreferredVehicles.FirstOrDefault() ?? VehicleDefinition.GetDefault();
	}
}
