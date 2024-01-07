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

	public const float DEFAULT_TURN_SPEED = 90f;
	public const float DEFAULT_TURN_SPEED_IDEAL_DISTANCE = 500f;
	public const float DEFAULT_TURN_SPEED_VELOCITY_FACTOR = 0.6f;

	public float MaxSpeed { get; set; } = DEFAULT_MAX_SPEED;
	public float Acceleration { get; set; } = DEFAULT_ACCELERATION;
	[Category( "Turning" )] public float TurnSpeed { get; set; } = DEFAULT_TURN_SPEED;
	[Category( "Turning" )] public float TurnSpeedIdealDistance { get; set; } = DEFAULT_TURN_SPEED_IDEAL_DISTANCE;
	/// <summary>
	/// Decreaes turn speed by this factor at max speed
	/// </summary>
	[Category( "Turning" )] public float TurnSpeedVelocityFactor { get; set; } = DEFAULT_TURN_SPEED_VELOCITY_FACTOR;
	public VehicleStats()
	{
		MaxSpeed = DEFAULT_MAX_SPEED;
		Acceleration = DEFAULT_ACCELERATION;

		TurnSpeed = DEFAULT_TURN_SPEED;
		TurnSpeedIdealDistance = DEFAULT_TURN_SPEED_IDEAL_DISTANCE;
		TurnSpeedVelocityFactor = DEFAULT_TURN_SPEED_VELOCITY_FACTOR;
	}
}
