using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleController
{
	/*
	Health for vehicles is much simpler than your regular FPS game
	Each hit = -1 health
	0 health = slowdown
	Heal with pickups 
	*/

	[Property] public int Health { get; set; }
	public void InitialiseCombat()
	{
		Health = GetMaxHealth();
	}

	[ActionGraphInclude]
	public void TakeDamage(int amount)
	{
		Health -= amount;
		Health = Health.Clamp( 0, GetMaxHealth() );
	}
}
