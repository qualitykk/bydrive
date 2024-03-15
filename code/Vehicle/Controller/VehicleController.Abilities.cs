using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleController
{
	const string BOOST_SOUND = "sounds/vehicle/boost.sound";
	const float BOOST_MIN_PITCH = 1f;
	const float BOOST_MAX_PITCH = 6f;
	public float RemainingBoost { get; set; }
	public bool UsingBoost { get; set; }
	public TimeSince TimeSinceUseBoost { get; set; }
	SoundHandle boostSoundInstance;
	private void InitialiseAbilities()
	{
		RemainingBoost = GetBoostDuration();
	}
	private void TickAbilities()
	{
		TickBoost();
	}

	private void TickBoost()
	{
		float dt = Time.Delta;
		float maxBoost = GetBoostDuration();

		if( WantsBoost && RemainingBoost > dt ) 
		{
			if(!UsingBoost)
			{
				//boostSoundInstance = PlaySound( BOOST_SOUND );
			}

			RemainingBoost -= dt;
			RemainingBoost = MathF.Max( RemainingBoost, 0 );

			//boostSoundInstance.Pitch = RemainingBoost.Remap( 0, maxBoost, BOOST_MAX_PITCH, BOOST_MIN_PITCH );
			TimeSinceUseBoost = 0;
			UsingBoost = true;
		}
		else
		{
			UsingBoost = false;
			//boostSoundInstance?.Stop(1f);
		}

		if(RemainingBoost < maxBoost && TimeSinceUseBoost > GetBoostRechargeCooldown())
		{
			RemainingBoost += dt * GetBoostRechargeFactor();
		}
	}
}
