using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleCustomizeMenu
{
	RaceSetupManager manager => RaceSetupManager.Current;
	VehicleDefinition currentVehicle => manager?.SelectedVehicle ?? default;
	Dictionary<AttachmentSlot, List<AttachmentDefinition>> currentAttachments => manager?.SelectedAttachments;
	private void OnClickVehicle(VehicleDefinition vehicle)
	{
		manager.SetPreview( vehicle );
	}
	private bool AttachmentActive(AttachmentDefinition definition)
	{
		if ( !currentAttachments.TryGetValue( definition.Slot, out var attachments ) )
			return false;

		return attachments.Contains( definition );
	}
	private int SlotAttachmentCount(AttachmentSlot slot)
	{
		if ( currentVehicle == null ) return 0;
		if ( !currentAttachments.TryGetValue( slot, out var attachments ) ) return 0;

		return attachments.Count;
	}
	private int SlotAttachmentMax(AttachmentSlot slot)
	{
		if ( currentVehicle == null ) return 0;

		return currentVehicle.AttachmentSlots.Where( attach => attach.Slot == slot ).Count();
	}
	private void OnClickAttachment(AttachmentDefinition definition)
	{
		if ( currentVehicle == null ) return;

		AttachmentSlot slot = definition.Slot;

		if ( !currentAttachments.TryGetValue( slot, out var attachments ) )
		{
			attachments = new();
			currentAttachments.Add( slot, attachments );
		}

		if(attachments.Contains(definition))
		{
			attachments.Remove( definition ); 
		}
		else
		{
			attachments.Add( definition );
		}
	}
	protected override int BuildHash()
	{
		return HashCode.Combine( currentVehicle, currentAttachments );
	}

	private IEnumerable<IGrouping<AttachmentSlot, AttachmentDefinition>> GetAttachments()
	{
		var attachments = ResourceLibrary.GetAll<AttachmentDefinition>();
		return attachments.GroupBy( a => a.Slot ).OrderBy(group => group.Key);
	}
}
