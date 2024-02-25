using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleController
{
	private class StatModifier : IEquatable<StatModifier>
	{
		public string Id { get; }
		public string Stat { get; }
		public float Multiplier { get; }
		public float LifeTime { get; private set; }
		public float DeletionTime { get; set; }

		public StatModifier( string id, string stat, float multiplier, float lifeTime = 1f )
		{
			Id = id;
			Stat = stat;
			Multiplier = multiplier;
			LifeTime = lifeTime;
			DeletionTime = Time.Now + lifeTime;
		}

		public void AddLifetime(float time)
		{
			LifeTime += time;
			DeletionTime += time;
		}

		public bool Equals( StatModifier other )
		{
			return Id.Equals( other.Id );
		}

		public override string ToString()
		{
			return $"{Id}/{DeletionTime}";
		}
	}
	[Property] public VehicleStatsProvider StatProvider { get; set; }
	public VehicleStats Stats => StatProvider?.GetStats() ?? new();

	public void ResetStats()
	{
		modifierIds.Clear();
		modifiers.Clear();
		modifiersPerStat.Clear();
	}
	public void TickStats()
	{
		TickModifiers();
	}

	#region Modifiers
	private Dictionary<string, StatModifier> modifierIds = new();
	private List<StatModifier> modifiers = new();
	private Dictionary<string, List<StatModifier>> modifiersPerStat = new();
	public bool AddStatModifier(string id, string stat, float multiplier, float time = 1f) => AddStatModifier( new( id, stat, multiplier, time ) );
	private bool AddStatModifier(StatModifier modifier)
	{
		if(modifierIds.TryGetValue(modifier.Id, out var existing))
		{
			existing.AddLifetime( modifier.LifeTime );
			return false;
		}

		modifierIds[modifier.Id] = modifier;
		modifiers.Add( modifier );

		if(!modifiersPerStat.ContainsKey(modifier.Stat))
		{
			modifiersPerStat.Add( modifier.Stat, new() { modifier } );
		}
		else
		{
			modifiersPerStat[modifier.Stat].Add( modifier );
		}
		return true;
	}
	private void DeleteModifier(StatModifier modifier)
	{
		if(!modifiers.Contains(modifier))
		{
			return;
		}

		modifierIds.Remove( modifier.Id );
		modifiers.Remove(modifier );
		modifiersPerStat[modifier.Stat].Remove( modifier );
	}

	private bool ShouldDelete(StatModifier modifier)
	{
		return Time.Now >= modifier.DeletionTime;
	}

	private void ApplyStatMultipliers(string id, ref float multiplier)
	{
		if(!modifiersPerStat.TryGetValue(id, out var modifiers) || !modifiers.Any())
		{
			return;
		}

		foreach(var mod in modifiers)
		{
			multiplier += mod.Multiplier - 1;
		}
	}

	private void TickModifiers()
	{
		StatModifier[] modifiersToDelete = modifiers.Where( ShouldDelete ).ToArray();
		if (modifiersToDelete.Any())
		{
			foreach ( var mod in modifiersToDelete )
			{
				DeleteModifier( mod );
			}
		}
	}
	#endregion
	public float GetMaxSpeed()
	{
		const float NO_HEALTH_SPEED_MULTIPLIER = 0.875f;
		const float HALF_HEALTH_SPEED_MULTIPLIER = 0.95f;

		float maxSpeed = Stats.MaxSpeed;
		if(Health <= 0)
		{
			maxSpeed *= NO_HEALTH_SPEED_MULTIPLIER;
		}
		else if(Health <= GetHalfHealth())
		{
			maxSpeed *= HALF_HEALTH_SPEED_MULTIPLIER;
		}

		float multiplier = 1f;
		if ( UsingBoost )
		{
			multiplier *= Stats.BoostSpeedMultiplier;
		}

		ApplyStatMultipliers( VehicleStatModifiers.SPEED, ref multiplier );

		return maxSpeed * multiplier;
	}

	public float GetAcceleration()
	{
		float acceleration = Stats.Acceleration;
		float multiplier = 1;
		if(UsingBoost)
		{
			multiplier *= Stats.BoostAccelerationMultiplier;
		}

		ApplyStatMultipliers( VehicleStatModifiers.ACCELERATION, ref multiplier );
		return acceleration * multiplier;
	}

	public float GetBoostDuration()
	{
		float boost = Stats.BoostDuration;
		return boost;
	}

	public float GetBoostSpeedMultiplier()
	{
		float speedMultiplier = Stats.BoostSpeedMultiplier;
		return speedMultiplier;
	}

	public float GetBoostRechargeCooldown()
	{
		float rechargeCooldown = Stats.BoostRechargeCooldown;
		return rechargeCooldown;
	}
	public float GetBoostRechargeFactor()
	{
		float rechargeFactor = Stats.BoostRechargeFactor;
		return rechargeFactor;
	}

	public int GetMaxHealth()
	{
		int maxHealth = Stats.MaxHealth;
		return maxHealth;
	}
	public int GetHalfHealth()
	{
		return (int)MathF.Floor( GetMaxHealth() / 2f );
	}
	public float GetTurnSpeed()
	{ 
		return Stats.TurnSpeed; 
	}

	public float GetTurnSpeedIdealDistance()
	{
		return Stats.TurnSpeedIdealDistance;
	}

	public float GetTurnSpeedVelocityFactor()
	{
		return Stats.TurnSpeedVelocityFactor;
	}

	public Vector3 GetCameraPositionOffset()
	{
		return Stats.CameraPositionOffset;
	}
}
