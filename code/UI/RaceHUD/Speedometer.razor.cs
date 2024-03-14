using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class Speedometer
{
	const string UNIT_KMH = "km/h";
	const string UNIT_MPH = "mph";
	const string UNIT_MS = "m/s";
	const string UNIT_US = "u/s";

	// Not the "real" conversion value, set so the number on the speedometer makes sense compared to IRL vehicles.
	const float METERS_PER_UNIT = 0.048f;
	const float MAX_SPEEDOMETER_SPEED = 9999.9f;

	private static VehicleSpeedUnit Units => Settings?.SpeedometerUnit ?? VehicleSpeedUnit.UnitsPerSecond;
	private static VehicleController Vehicle => GetLocalVehicle();

	private float GetSpeedAmount()
	{
		float speed = Vehicle?.Speed.Clamp(0f, MAX_SPEEDOMETER_SPEED ) ?? 0f;

		return Units switch
		{
			VehicleSpeedUnit.MilesPerHour => UnitsToMph( speed ),
			VehicleSpeedUnit.MetersPerSecond => UnitsToMs( speed ),
			VehicleSpeedUnit.UnitsPerSecond => speed,
			_ => UnitsToKmh( speed ),
		};
	}

	private string GetSpeedUnit()
	{
		return Units switch
		{
			VehicleSpeedUnit.MilesPerHour => UNIT_MPH,
			VehicleSpeedUnit.MetersPerSecond => UNIT_MS,
			VehicleSpeedUnit.UnitsPerSecond => UNIT_US,
			_ => UNIT_KMH,
		};
	}

	public float GetBoostRemaining()
	{
		float remaining = Vehicle?.RemainingBoost ?? VehicleStats.DEFAULT_BOOST_DURATION;
		return remaining;
	}

	public float GetBoostMax()
	{
		float max = Vehicle?.GetBoostDuration() ?? VehicleStats.DEFAULT_BOOST_DURATION;
		return max;
	}

	private float UnitsToMeters( float units ) => units * METERS_PER_UNIT;

	private float UnitsToMs( float speed )
	{
		return UnitsToMeters( speed );
	}
	private float UnitsToKmh( float speed )
	{
		const float MS_TO_KMH = 3.6f;
		return UnitsToMs( speed ) * MS_TO_KMH;
	}

	private float UnitsToMph( float speed )
	{
		const float MS_TO_MPH = 2.23694f;
		return UnitsToMs(speed) * MS_TO_MPH;
	}

	/// <summary>
	/// the hash determines if the system should be rebuilt. If it changes, it will be rebuilt
	/// </summary>
	protected override int BuildHash() => HashCode.Combine( GetLocalVehicle()?.Speed, GetBoostRemaining() );
}
