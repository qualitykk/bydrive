using Sandbox.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[Alias("CustomizationManager")]
public class RaceSetupManager : Component
{
	static readonly Type[] previewAllowedComponents = new[] { typeof( SkinnedModelRenderer ) };
	public static RaceSetupManager Current { get; private set; }
	[Property] public GameObject VehiclePreview { get; set; }
	public ChallengeDefinition SelectedChallenge { get; set; }
	public VehicleDefinition SelectedVehicle { get; private set; }
	public Dictionary<AttachmentSlot, List<AttachmentDefinition>> SelectedAttachments { get; private set; } = new();
	public void Start()
	{
		Assert.NotNull( SelectedChallenge );
		Assert.NotNull( SelectedVehicle );

		var slotDefinitions = SelectedVehicle.AttachmentSlots.ToList();
		VehicleBuilder vehicle = VehicleBuilder.ForDefinition( SelectedVehicle );
		if(SelectedAttachments != null && SelectedAttachments.Any())
		{
			Dictionary<Guid, AttachmentDefinition> slotDefinitionAttachments = new();
			foreach(var attachmentGroups in SelectedAttachments)
			{
				foreach(var attachment in attachmentGroups.Value )
				{
					AttachmentSlotPosition slot = slotDefinitions.FirstOrDefault( s => s.Slot == attachmentGroups.Key );
					if ( slot == default ) break;
					slotDefinitionAttachments.Add(slot.Id, attachment);
					slotDefinitions.Remove( slot );
				}
			}
			vehicle = vehicle.WithAttachments( slotDefinitionAttachments );
		}

		StartRace.Challenge( SelectedChallenge, vehicle );
	}
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

		SelectedAttachments.Clear();
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
