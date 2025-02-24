﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleController
{
	const float ITEM_COOLDOWN = 0.5f;
	[Property] public Vector3 ItemSpawnPosition { get; set; }
	public ItemDefinition CurrentItem { get; private set; }
	public TimeSince TimeSinceUseItem { get; set; }

	private void InitialiseItems()
	{
		TimeSinceUseItem = 0;
	}
	private void TickItems()
	{
		if ( WantsItem && CanUseCurrentItem() )
		{
			UseItem();
		}
	}
	public bool EquipItem( ItemDefinition def )
	{
		if ( !CanEquipItem() ) return false;
		CurrentItem = def;
		return true;
	}
	public bool CanEquipItem()
	{
		return CurrentItem == default;
	}
	public bool CanUseCurrentItem()
	{
		return TimeSinceUseItem > ITEM_COOLDOWN && CurrentItem != default;
	}
	public void UseItem()
	{
		if(CurrentItem == null)
		{
			return;
		}

		Vector3 spawnPos = Transform.Local.PointToWorld( ItemSpawnPosition );

		GameObject itemObject = ResourceHelper.CreateObjectFromResource( CurrentItem );
		var itemHooks = itemObject.Components.GetAll<VehicleItemEvents>();
		foreach ( var itemHook in itemHooks )
		{
			itemHook.OnItemUsed?.Invoke( this );
		}

		itemObject.WorldPosition = spawnPos;
		// Only use the vehicle yaw, we dont wanna mess with item pitch & roll
		itemObject.WorldRotation = Rotation.FromYaw( WorldRotation.Yaw() );

		CurrentItem = default;
		WantsItem = false;
		TimeSinceUseItem = 0;
	}

	[ConCmd("rc_give_item")]
	private static void Command_GiveItem(string name)
	{
		ItemDefinition item = ResourceLibrary.GetAll<ItemDefinition>().FirstOrDefault( i => i.ResourceName == name );
		if(item == null)
		{
			Log.Warning( $"No item with name {name} exists!" );
			return;
		}

		var localVehicle = GetLocalVehicle();
		localVehicle?.EquipItem( item );
	}
}
