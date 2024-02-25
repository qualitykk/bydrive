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
	/// <summary>
	/// Boost percentage gained on pickup
	/// </summary>
	[Property] public float Amount { get; set; } = 1.0f;
	public override bool OnPickup( VehicleController vehicle )
	{
		float maxBoost = vehicle.GetBoostDuration();
		float boostAmount = maxBoost * Amount;
		if(vehicle.RemainingBoost < maxBoost )
		{
			vehicle.RemainingBoost += boostAmount;
			vehicle.RemainingBoost = vehicle.RemainingBoost.Clamp( 0, maxBoost );
			Notifications.Add( vehicle, new("Picked up boost", UI.Colors.Notification.Bonus) );
			return true;
		}

		return false;
	}
}
