using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class SaveFile
{
	public bool CanUseVehicle(VehicleDefinition vehicle)
	{
		bool canUse = vehicle != null;
		canUse = canUse && vehicle == VehicleDefinition.GetDefault();
		return canUse;
	}
}
