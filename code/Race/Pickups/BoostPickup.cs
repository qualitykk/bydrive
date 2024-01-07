using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Category("Race")]
public class BoostPickup : RacerPickup
{
	public override bool OnPickup( VehicleController vehicle )
	{
		float maxBoost = vehicle.GetBoostDuration();

		if(vehicle.RemainingBoost < maxBoost )
		{
			vehicle.RemainingBoost = maxBoost;
			return true;
		}

		return false;
	}
}
