using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Category( "Vehicle" )]
[Icon("analytics")]
public class StaticVehicleStats : VehicleStatsProvider
{
	[Property] public float Speed { get; set; }
	[Property] public float Acceleration { get; set; }
	[Property] public float TurnSpeed { get; set; }
	[Property] public float TurnSpeedIdealDistance { get; set; }
	[Property] public float TurnSpeedVelocityFactor { get; set; }
	public override VehicleStats GetStats()
	{
		return new()
		{
			MaxSpeed = Speed,
			Acceleration = Acceleration,
			TurnSpeed = TurnSpeed,
			TurnSpeedIdealDistance = TurnSpeedIdealDistance,
			TurnSpeedVelocityFactor = TurnSpeedVelocityFactor,
		};
	}
}
