using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleController
{
	public float RemainingBoost { get; set; }
	public bool UsingBoost { get; set; }
	public TimeSince TimeSinceUseBoost { get; set; }
	public void InitialiseAbilities()
	{
		RemainingBoost = GetBoostDuration();
	}
	public void DoAbilities()
	{
		TickBoost();
	}

	public void TickBoost()
	{
		float dt = Time.Delta;
		float maxBoost = GetBoostDuration();
		bool wantsBoost = BoostInput > 0;

		if( wantsBoost && RemainingBoost > dt ) 
		{
			RemainingBoost -= dt;
			RemainingBoost = MathF.Max( RemainingBoost, 0 );
			TimeSinceUseBoost = 0;
			UsingBoost = true;
		}
		else
		{
			UsingBoost = false;
		}

		if(RemainingBoost < maxBoost && TimeSinceUseBoost > GetBoostRechargeCooldown())
		{
			RemainingBoost += dt * GetBoostRechargeFactor();
		}
	}
}
