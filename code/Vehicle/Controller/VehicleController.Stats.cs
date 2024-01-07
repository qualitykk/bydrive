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
		return maxSpeed;
	}

	public float GetAcceleration()
	{
		float acceleration = Stats.Acceleration;
		return acceleration;
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
