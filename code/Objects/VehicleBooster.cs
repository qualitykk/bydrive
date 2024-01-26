using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Icon( "fast_forward" )]
public class VehicleBooster : Component, Component.ITriggerListener
{
	const string BOOSTER_SPEED_MODIFIER = "BoosterComponent_Speed";
	const float BOOSTER_SPEED_MULTIPLIER = 1.4f;
	const string BOOSTER_ACCELERATION_MODIFIER = "BoosterComponent_Accel";
	const float BOOSTER_ACCELERATION_MULTIPLIER = 2.0f;
	[Property] public float Time { get; set; } = 0.5f;
	[Property, Title("Sound")] public SoundEvent SoundResource { get; set; }
	public static bool ApplyBoost(VehicleController vehicle, float time)
	{
		bool added = true;
		added = added & vehicle.AddStatModifier( BOOSTER_SPEED_MODIFIER, VehicleStatModifiers.SPEED, BOOSTER_SPEED_MULTIPLIER, time );
		added = added & vehicle.AddStatModifier( BOOSTER_ACCELERATION_MODIFIER, VehicleStatModifiers.ACCELERATION, BOOSTER_ACCELERATION_MULTIPLIER, time );
		return added;
	}
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if( !other.GameObject.Components.TryGet<VehicleController>(out var vehicle, FindMode.EnabledInSelfAndDescendants))
		{
			return;
		}

		bool playSound = ApplyBoost( vehicle, Time );
		if(playSound && SoundResource != default)
		{
			vehicle.PlaySound( SoundResource );
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		if ( !other.GameObject.Components.TryGet<VehicleController>( out var vehicle, FindMode.EnabledInSelfAndDescendants ) )
		{
			return;
		}

		ApplyBoost( vehicle, Time );
	}
}
