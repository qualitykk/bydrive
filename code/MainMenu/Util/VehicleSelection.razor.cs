using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleSelection
{
	private static readonly Vector3 PreviewCameraPosition = new( -150, 12, 90 );
	private static readonly Angles PreviewCameraAngles = new( 32, 0, 0 );

	public Action<VehicleDefinition> OnVehicleSelected { get; set; }

	VehicleDefinition selectedVehicle;
	IEnumerable<VehicleDefinition> vehicles;
	protected override void OnParametersSet()
	{
		vehicles = ResourceLibrary.GetAll<VehicleDefinition>().Where(def => !def.Hidden);
	}
	private static Vector3 VehiclePreviewPosition(VehicleDefinition def)
	{
		if ( def == null ) return PreviewCameraPosition;
		return def?.PreviewPosition != Vector3.Zero ? def.PreviewPosition : PreviewCameraPosition;
	}
	private void OnClickVehicle(VehicleDefinition def)
	{
		selectedVehicle = def;
		OnVehicleSelected?.Invoke( def );
	}
}
