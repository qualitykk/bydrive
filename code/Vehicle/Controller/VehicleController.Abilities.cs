using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleController
{
	const float ITEM_COOLDOWN = 0.5f;
	public float RemainingBoost { get; set; }
	public bool UsingBoost { get; set; }
	public TimeSince TimeSinceUseBoost { get; set; }
	[Property] public Vector3 ItemSpawnPosition { get; set; }
	public ItemDefinition CurrentItem { get; private set; }
	public TimeSince TimeSinceUseItem { get; set; }
	public void InitialiseAbilities()
	{
		RemainingBoost = GetBoostDuration();
		TimeSinceUseItem = 0;
	}
	public void TickAbilities()
	{
		TickBoost();
		TickItems();
	}

	public void TickBoost()
	{
		float dt = Time.Delta;
		float maxBoost = GetBoostDuration();

		if( WantsBoost && RemainingBoost > dt ) 
		{
			RemainingBoost -= dt;
			RemainingBoost = MathF.Max( RemainingBoost, 0 );
			TimeSinceUseBoost = 0;
			UsingBoost = true;
		}
		else
		{
			UsingBoost = false;
		}

		if(RemainingBoost < maxBoost && TimeSinceUseBoost > GetBoostRechargeCooldown())
		{
			RemainingBoost += dt * GetBoostRechargeFactor();
		}
	}

	public void TickItems()
	{
		if(WantsItem && CanUseCurrentItem() )
		{
			UseItem();
		}
	}
	public bool EquipItem(ItemDefinition def )
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
	private void UseItem()
	{
		Vector3 spawnPos = Transform.Local.PointToWorld( ItemSpawnPosition );

		GameObject itemObject = ResourceHelper.CreateObjectFromResource( CurrentItem );
		var itemHooks = itemObject.Components.GetAll<VehicleItemEvents>();
		foreach ( var itemHook in itemHooks )
		{
			itemHook.OnItemUsed?.Invoke( this );
		}

		itemObject.Transform.Position = spawnPos;
		// Only use the vehicle yaw, we dont wanna mess with item pitch & roll
		itemObject.Transform.Rotation = Rotation.FromYaw( Transform.Rotation.Yaw() );

		CurrentItem = default;
		WantsItem = false;
		TimeSinceUseItem = 0;
	}
}
