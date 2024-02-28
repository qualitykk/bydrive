using Sandbox.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class CustomizationManager : Component
{
	public static CustomizationManager Current { get; private set; }
	static readonly Type[] previewAllowedComponents = new[] { typeof( SkinnedModelRenderer ) };
	[Property] public GameObject VehiclePreview { get; set; }
	public VehicleDefinition SelectedVehicle { get; private set; }
	public Dictionary<AttachmentSlotPosition, AttachmentDefinition> SelectedAttachments { get; private set; }
	protected override void OnStart()
	{
		Current = this;
	}
	protected override void OnDisabled()
	{
		if ( Current == this )
			Current = null;
	}
	public void SetPreview(VehicleDefinition vehicle)
	{
		Assert.NotNull( VehiclePreview );
		Assert.NotNull( vehicle );

		VehiclePreview.Children.ForEach( ( GameObject child ) => child.Destroy() );

		GameObject instance = vehicle.CreateObject();
		var vehicleComponents = instance.Components.GetAll(FindMode.EverythingInSelfAndDescendants).ToArray();
		foreach(var component in vehicleComponents )
		{
			if(previewAllowedComponents.Contains(component.GetType()))
			{
				continue;
			}
			component.Destroy();
		}

		instance.Parent = VehiclePreview;
		SelectedVehicle = vehicle;
	}

	[ConCmd("st_customize_preview")]
	private static void Command_ForcePreview(string vehiclePath)
	{
		if(!ResourceLibrary.TryGet<VehicleDefinition>(vehiclePath, out var definition))
		{
			Log.Error( $"No vehicle with path {vehiclePath} exists!" );
			return;
		}

		Current?.SetPreview(definition);
	}
}
