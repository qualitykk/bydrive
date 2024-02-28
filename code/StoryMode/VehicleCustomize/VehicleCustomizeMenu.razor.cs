using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleCustomizeMenu : PanelComponent
{
	CustomizationManager manager => CustomizationManager.Current;
	VehicleDefinition currentVehicle => CustomizationManager.Current?.SelectedVehicle;

	private void OnClickVehicle(VehicleDefinition vehicle)
	{
		manager.SetPreview( vehicle );
	}
	protected override int BuildHash()
	{
		return HashCode.Combine( currentVehicle );
	}
}
