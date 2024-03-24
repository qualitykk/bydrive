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
	public const float DEFAULT_TURN_SPEED_LOW_VELOCITY_FACTOR = 0.3f;
	public const float DEFAULT_TURN_SPEED_VELOCITY_FACTOR = 0.6f;

	public const float DEFAULT_ANGULAR_DAMPING = 5f;

	public float MaxSpeed { get; set; } = DEFAULT_MAX_SPEED;
	public float Acceleration { get; set; } = DEFAULT_ACCELERATION;
	/// <summary>
	/// Acceleration relative to speed
	/// </summary>
	public Curve AccelerationCurve { get; set; } = 1f;

	public int MaxHealth { get; set; } = DEFAULT_MAX_HEALTH;

	/// <summary>
	/// Maximum boost gauge in seconds
	/// </summary>
	[Category( "Boost" )] public float BoostDuration { get; set; } = DEFAULT_BOOST_DURATION;
	[Category( "Boost" )] public float BoostSpeedMultiplier { get; set; } = DEFAULT_BOOST_SPEED_FACTOR;
	[Category( "Boost" )] public float BoostAccelerationMultiplier { get; set; } = DEFAULT_BOOST_ACCELERATION_FACTOR;
	[Category( "Boost" )] public float BoostRechargeCooldown { get; set; } = DEFAULT_BOOST_RECHARGE_COOLDOWN;
	[Category( "Boost" )] public float BoostRechargeFactor { get; set; } = DEFAULT_BOOST_RECHARGE_FACTOR;
	[Category( "Handling" )] public float TurnSpeed { get; set; } = DEFAULT_TURN_SPEED;
	/// <summary>
	/// Turn factor relative to speed
	/// </summary>
	[Category( "Handling" )] public Curve TurnFactorCurve { get; set; } = 1f;
	[Category( "Suspension" )] public float SpringStrength { get; set; } = 100f;
	[Category( "Suspension" )] public float SpringDamping { get; set; } = 50f;
	[Category( "Suspension" )] public float Grip { get; set; } = 1f;
	/// <summary>
	/// Grip relative to speed
	/// </summary>
	[Category( "Suspension" )] public Curve SlidingGripCurve { get; set; } = 1f;
	
	[Category( "Suspension" )] public float AngularDamping { get; set; } = DEFAULT_ANGULAR_DAMPING;
	[Category( "Visual" )] public Vector3 CameraPositionOffset { get; set; } = new();
	[Category( "Items" )] public List<ItemDefinition> BonusItems { get; set; } = new();
	public VehicleStats()
	{
		
	}
}
