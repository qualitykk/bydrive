﻿using System;
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
	public TimeSince TimeSinceUseItem { get; set; }
	public void InitialiseAbilities()
	{
		RemainingBoost = GetBoostDuration();
		TimeSinceUseItem = 0;
	}
	public void DoAbilities()
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
		if(WantsItem && TimeSinceUseItem > ITEM_COOLDOWN)
		{
			UseItem();
		}
	}

	private void UseItem()
	{
		Vector3 spawnPos = Transform.Local.PointToWorld( ItemSpawnPosition );

		GameObject itemObject = new();
		itemObject.ApplyPrefab( "prefabs/item_bouncer.prefab" );

		itemObject.Transform.Position = spawnPos;
		itemObject.Transform.LocalRotation = Transform.LocalRotation;

		WantsItem = false;
		TimeSinceUseItem = 0;
	}
}