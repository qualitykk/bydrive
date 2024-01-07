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
	[Property] public float BoostDuration { get; set; }
	[Property] public float BoostSpeedMultiplier { get; set; }
	[Property] public float BoostAccelerationMultiplier { get; set; }
	[Property] public float BoostRechargeCooldown { get; set; }
	[Property] public float BoostRechargeFactor { get; set; }
	[Property] public float TurnSpeed { get; set; }
	[Property] public float TurnSpeedIdealDistance { get; set; }
	[Property] public float TurnSpeedVelocityFactor { get; set; }
	public override VehicleStats GetStats()
	{
		return new()
		{
			MaxSpeed = Speed,
			Acceleration = Acceleration,
			BoostDuration = BoostDuration,
			BoostSpeedMultiplier = BoostSpeedMultiplier,
			BoostAccelerationMultiplier = BoostAccelerationMultiplier,
			BoostRechargeCooldown = BoostRechargeCooldown,
			BoostRechargeFactor = BoostRechargeFactor,
			TurnSpeed = TurnSpeed,
			TurnSpeedIdealDistance = TurnSpeedIdealDistance,
			TurnSpeedVelocityFactor = TurnSpeedVelocityFactor,
		};
	}
}
