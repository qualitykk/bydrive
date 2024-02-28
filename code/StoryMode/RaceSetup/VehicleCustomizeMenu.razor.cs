using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleCustomizeMenu
{
	RaceSetupManager manager => RaceSetupManager.Current;
	VehicleDefinition currentVehicle => RaceSetupManager.Current?.SelectedVehicle;

	private void OnClickVehicle(VehicleDefinition vehicle)
	{
		manager.SetPreview( vehicle );
	}
	protected override int BuildHash()
	{
		return HashCode.Combine( currentVehicle );
	}
}
