using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public enum VehicleSpeedUnit
{
	[Title( "Kilometers/Hour" )] KilometersPerHour,
	[Title( "Miles/Hour" )] MilesPerHour,
	[Title( "Meters/Second" )] MetersPerSecond,
	[Title( "Units/Second" )] UnitsPerSecond
}
