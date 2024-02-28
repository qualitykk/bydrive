using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class ItemPickup : RacerPickup
{
	[Property] public ItemPool Items { get; set; }
	public override bool OnPickup( VehicleController vehicle )
	{
		const int MAX_RETRIES = 5;
		if(Items == null) return false;
		if ( !vehicle.CanEquipItem() ) return false;

		ItemDefinition item = Items.GetRandom();
		int retries = 0;
		while(!CanEquip(vehicle, item) && retries < MAX_RETRIES)
		{
			item = Items.GetRandom();
			retries++;
		}

		if ( retries >= MAX_RETRIES )
		{
			return false;
		}

		if ( vehicle.EquipItem( item ) )
		{
			RaceNotifications.Add( vehicle, new( "Picked up item", UI.Colors.Notification.Critical ) );
			return true;
		}

		return false;
	}

	public virtual ItemDefinition GetItems(VehicleController vehicle)
	{
		List<ItemDefinition> itemChoices = Items.GetItemsWeighted();
		itemChoices.AddRange( vehicle.GetVehicleItems() );

		return Game.Random.FromList( itemChoices );
	}
	public virtual bool CanEquip(VehicleController vehicle, ItemDefinition item)
	{
		return true;
	}
}
