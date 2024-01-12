using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Category("Race")]
[Icon("speed")]
public class BoostPickup : RacerPickup
{
	public override bool OnPickup( VehicleController vehicle )
	{
		float maxBoost = vehicle.GetBoostDuration();

		if(vehicle.RemainingBoost < maxBoost )
		{
			vehicle.RemainingBoost = maxBoost;
			Notifications.Add( vehicle, new("Picked up boost", UIColors.Notification.Bonus) );
			return true;
		}

		return false;
	}
}
