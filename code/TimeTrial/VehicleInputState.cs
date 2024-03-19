using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Bydrive;

public class VehicleInputState
{
	public float ThrottleInput { get; set; }
	public float TurnInput { get; set; }
	public float BreakInput { get; set; }
	public float TiltInput { get; set; }

	public bool WantsBoost { get; set; }
	public bool WantsItem { get; set; }

	[JsonConstructor]
	public VehicleInputState( float throttleInput, float turnInput, float breakInput, float tiltInput, bool wantsBoost, bool wantsItem )
	{
		ThrottleInput = throttleInput;
		TurnInput = turnInput;
		BreakInput = breakInput;
		TiltInput = tiltInput;
		WantsBoost = wantsBoost;
		WantsItem = wantsItem;
	}

	public VehicleInputState(VehicleController vehicle)
	{
		ThrottleInput = vehicle.ThrottleInput; 
		TurnInput = vehicle.TurnInput; 
		BreakInput = vehicle.BreakInput; 
		TiltInput = vehicle.TiltInput; 
		WantsBoost = vehicle.WantsBoost; 
		WantsItem = vehicle.WantsItem;
	}
}
