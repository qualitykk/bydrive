using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public struct VehicleBuilder
{
	public static VehicleBuilder ForDefinition(VehicleDefinition definition)
	{
		return new VehicleBuilder()
		{
			Vehicle = definition
		};
	}
	public VehicleDefinition Vehicle { get; set; }
	public Dictionary<Guid, AttachmentDefinition> Attachments { get; set; }
	public VehicleBuilder WithVehicle(VehicleDefinition vehicle)
	{ 
		Vehicle = vehicle;
		return this;
	}
	public VehicleBuilder WithAttachments(Dictionary<Guid, AttachmentDefinition> attachments)
	{
		Attachments = attachments;
		return this;
	}
	// TODO: Upgrades
	public GameObject Build()
	{
		GameObject instance = Vehicle.CreateObject();
		var controller = instance.Components.GetInDescendantsOrSelf<VehicleController>();
		if(controller == null)
		{
			instance.DestroyImmediate();
			throw new InvalidOperationException( "Tried to spawn vehicle without controller? WTF?" );
		}

		if(Attachments != null && Attachments.Any())
		{
			foreach ( (Guid id, AttachmentDefinition attachment) in Attachments )
			{
				var slotInfo = Vehicle.AttachmentSlots.FirstOrDefault( slot => slot.Id == id );
				if ( slotInfo == null ) continue;

				controller.AddAttachment( new( slotInfo.LocalPosition, slotInfo.LocalRotation ), attachment );
			}
		}

		return instance;
	}
}
