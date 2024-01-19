using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public struct VehicleStats
{
	public const float DEFAULT_MAX_SPEED = 900f;
	public const float DEFAULT_ACCELERATION = 1200f;
	public const int DEFAULT_MAX_HEALTH = 4;

	public const float DEFAULT_BOOST_DURATION = 3f;
	public const float DEFAULT_BOOST_SPEED_FACTOR = 1.2f;
	public const float DEFAULT_BOOST_ACCELERATION_FACTOR = 2f;
	public const float DEFAULT_BOOST_RECHARGE_COOLDOWN = 2f;
	public const float DEFAULT_BOOST_RECHARGE_FACTOR = 0.8f;

	public const float DEFAULT_TURN_SPEED = 90f;
	public const float DEFAULT_TURN_SPEED_IDEAL_DISTANCE = 500f;
	public const float DEFAULT_TURN_SPEED_VELOCITY_FACTOR = 0.6f;

	public float MaxSpeed { get; set; } = DEFAULT_MAX_SPEED;
	public float Acceleration { get; set; } = DEFAULT_ACCELERATION;
	public int MaxHealth { get; set; } = DEFAULT_MAX_HEALTH;

	/// <summary>
	/// Maximum boost gauge in seconds
	/// </summary>
	[Category( "Boost" )] public float BoostDuration { get; set; } = DEFAULT_BOOST_DURATION;
	[Category( "Boost" )] public float BoostSpeedMultiplier { get; set; } = DEFAULT_BOOST_SPEED_FACTOR;
	[Category( "Boost" )] public float BoostAccelerationMultiplier { get; set; } = DEFAULT_BOOST_ACCELERATION_FACTOR;
	[Category( "Boost" )] public float BoostRechargeCooldown { get; set; } = DEFAULT_BOOST_RECHARGE_COOLDOWN;
	[Category( "Boost" )] public float BoostRechargeFactor { get; set; } = DEFAULT_BOOST_RECHARGE_FACTOR;
	[Category( "Turning" )] public float TurnSpeed { get; set; } = DEFAULT_TURN_SPEED;
	[Category( "Turning" )] public float TurnSpeedIdealDistance { get; set; } = DEFAULT_TURN_SPEED_IDEAL_DISTANCE;
	/// <summary>
	/// Decreaes turn speed by this factor at max speed
	/// </summary>
	[Category( "Turning" )] public float TurnSpeedVelocityFactor { get; set; } = DEFAULT_TURN_SPEED_VELOCITY_FACTOR;
	[Category( "Visual" )] public float CameraPositionOffset { get; set; }
	public VehicleStats()
	{
		MaxSpeed = DEFAULT_MAX_SPEED;
		Acceleration = DEFAULT_ACCELERATION;
		MaxHealth = DEFAULT_MAX_HEALTH;

		BoostDuration = DEFAULT_BOOST_DURATION;
		BoostSpeedMultiplier = DEFAULT_BOOST_SPEED_FACTOR;
		BoostAccelerationMultiplier = DEFAULT_BOOST_ACCELERATION_FACTOR;
		BoostRechargeCooldown = DEFAULT_BOOST_RECHARGE_COOLDOWN;
		BoostRechargeFactor = DEFAULT_BOOST_RECHARGE_FACTOR;

		TurnSpeed = DEFAULT_TURN_SPEED;
		TurnSpeedIdealDistance = DEFAULT_TURN_SPEED_IDEAL_DISTANCE;
		TurnSpeedVelocityFactor = DEFAULT_TURN_SPEED_VELOCITY_FACTOR;
	}
}
