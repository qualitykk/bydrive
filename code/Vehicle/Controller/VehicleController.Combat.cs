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

	private int health;
	public int Health => health;
	public void InitialiseCombat()
	{
		health = GetMaxHealth();
	}
	public void AddHealth(int amount )
	{
		health += amount;
		health.Clamp( 0, GetMaxHealth() );
	}
	public void TakeDamage(int amount)
	{
		health -= amount;
		health = health.Clamp( 0, GetMaxHealth() );
	}
}
