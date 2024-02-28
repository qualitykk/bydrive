using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

/// <summary>
/// Deals damage to a vehicle when colliding with it.
/// </summary>
[Icon("car_crash")]
public class HitVehicles : Component, Component.ICollisionListener
{
	public delegate void VehicleHit(VehicleController victim);
	[Title("Damage Amount"), Property] public int Amount { get; set; } = 1;
	[Property] public bool DestroyOnHit { get; set; } = true;
	[Property] public VehicleHit OnHit { get; set; }
	void ICollisionListener.OnCollisionStart( Collision args )
	{
		var target = args.Other;
		var vehicle = target.GameObject.Components.GetInDescendantsOrSelf<VehicleController>();
		if ( vehicle == null )
			return;

		vehicle.TakeDamage( Amount );
		OnHit?.Invoke( vehicle );
		if(DestroyOnHit)
		{
			GameObject.Destroy();
		}
	}

	void ICollisionListener.OnCollisionStop( CollisionStop args )
	{
	}

	void ICollisionListener.OnCollisionUpdate( Collision args )
	{
	}
}
