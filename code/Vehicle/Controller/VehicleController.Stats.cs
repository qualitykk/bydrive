using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleController
{
	[Property] public VehicleStatsProvider StatProvider { get; set; }
	public VehicleStats Stats => StatProvider?.GetStats() ?? new();

	public float GetMaxSpeed()
	{
		float maxSpeed = Stats.MaxSpeed;
		if ( UsingBoost )
		{
			maxSpeed *= Stats.BoostSpeedMultiplier;
		}

		return maxSpeed;
	}

	public float GetAcceleration()
	{
		float acceleration = Stats.Acceleration;
		if(UsingBoost)
		{
			acceleration *= Stats.BoostAccelerationMultiplier;
		}

		return acceleration;
	}

	public float GetBoostDuration()
	{
		float boost = Stats.BoostDuration;
		return boost;
	}

	public float GetBoostSpeedMultiplier()
	{
		float speedMultiplier = Stats.BoostSpeedMultiplier;
		return speedMultiplier;
	}

	public float GetBoostRechargeCooldown()
	{
		float rechargeCooldown = Stats.BoostRechargeCooldown;
		return rechargeCooldown;
	}
	public float GetBoostRechargeFactor()
	{
		float rechargeFactor = Stats.BoostRechargeFactor;
		return rechargeFactor;
	}

	public float GetTurnSpeed()
	{ 
		return Stats.TurnSpeed; 
	}

	public float GetTurnSpeedIdealDistance()
	{
		return Stats.TurnSpeedIdealDistance;
	}

	public float GetTurnSpeedVelocityFactor()
	{
		return Stats.TurnSpeedVelocityFactor;
	}
}
