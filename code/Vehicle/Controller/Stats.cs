using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleController
{
	private class TemporaryStatModifier : IEquatable<TemporaryStatModifier>
	{
		public string Id { get; }
		public string Stat { get; }
		public float Multiplier { get; }
		public float LifeTime { get; private set; }
		public float DeletionTime { get; set; }

		public TemporaryStatModifier( string id, string stat, float multiplier, float lifeTime = 1f )
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

		public bool Equals( TemporaryStatModifier other )
		{
			return Id.Equals( other.Id );
		}

		public override string ToString()
		{
			return $"{Id}/{DeletionTime}";
		}
	}
	private List<VehicleStatModifier> permamentStatModifiers = new();
	[ActionGraphIgnore]
	public void AddPermanentStatModifier(VehicleStatModifier modifier)
	{
		permamentStatModifiers.Add( modifier );
	}
	public void AddPermanentStatModifier( VehicleStats stats, VehicleStatChangeMode mode ) => AddPermanentStatModifier( new( stats, mode ) );
	public VehicleStats GetStats()
	{
		VehicleStats stats = Definition?.Stats ?? new();
		if(_attachments.Any())
		{
			foreach ( var attachment in _attachments )
			{
				stats = stats.WithChanges( attachment );
			}
		}
		if(permamentStatModifiers.Any())
		{
			foreach(var modifier in permamentStatModifiers)
			{
				stats = stats.WithChanges( modifier.Stats, modifier.Mode );
			}
		}

		return stats;
	}
	[Title("Reset Stat Modifiers"), Category( "Stats" )]
	public void ResetStats()
	{
		modifierIds.Clear();
		modifiers.Clear();
		modifiersPerStat.Clear();
	}
	private void TickStats()
	{
		TickTemporaryModifiers();
	}

	#region Modifiers
	private Dictionary<string, TemporaryStatModifier> modifierIds = new();
	private List<TemporaryStatModifier> modifiers = new();
	private Dictionary<string, List<TemporaryStatModifier>> modifiersPerStat = new();
	public bool AddTemporaryStatModifier(string id, string stat, float multiplier, float time = 1f) => AddTemporaryStatModifier( new( id, stat, multiplier, time ) );
	private bool AddTemporaryStatModifier(TemporaryStatModifier modifier)
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
	private void DeleteTemporaryStatModifier(TemporaryStatModifier modifier)
	{
		if(!modifiers.Contains(modifier))
		{
			return;
		}

		modifierIds.Remove( modifier.Id );
		modifiers.Remove(modifier );
		modifiersPerStat[modifier.Stat].Remove( modifier );
	}

	private bool ShouldDeleteTemporaryModifier(TemporaryStatModifier modifier)
	{
		return Time.Now >= modifier.DeletionTime;
	}

	private void ApplyTemporaryStatMultipliers(string id, ref float multiplier)
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

	private void TickTemporaryModifiers()
	{
		TemporaryStatModifier[] modifiersToDelete = modifiers.Where( ShouldDeleteTemporaryModifier ).ToArray();
		if (modifiersToDelete.Any())
		{
			foreach ( var mod in modifiersToDelete )
			{
				DeleteTemporaryStatModifier( mod );
			}
		}
	}
	#endregion
	[Category("Stats")]
	public float GetMaxSpeed()
	{
		const float NO_HEALTH_SPEED_MULTIPLIER = 0.875f;
		const float HALF_HEALTH_SPEED_MULTIPLIER = 0.95f;

		float maxSpeed = GetStats().MaxSpeed;
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
			multiplier *= GetStats().BoostSpeedMultiplier;
		}

		ApplyTemporaryStatMultipliers( VehicleStatModifiers.SPEED, ref multiplier );

		return maxSpeed * multiplier;
	}
	[Category( "Stats" )]
	public float GetAcceleration(float speed)
	{
		float speedFraction = speed / GetMaxSpeed();
		float accelerationFactor = GetStats().AccelerationCurve.Evaluate( speedFraction );
		float acceleration = GetStats().Acceleration * accelerationFactor;
		float multiplier = 1;
		if(UsingBoost)
		{
			multiplier *= GetStats().BoostAccelerationMultiplier;
		}

		ApplyTemporaryStatMultipliers( VehicleStatModifiers.ACCELERATION, ref multiplier );
		return acceleration * multiplier;
	}

	[Category( "Stats" )]
	public float GetBoostDuration()
	{
		float boost = GetStats().BoostDuration;
		return boost;
	}
	[Category( "Stats" )]
	public float GetBoostSpeedMultiplier()
	{
		float speedMultiplier = GetStats().BoostSpeedMultiplier;
		return speedMultiplier;
	}
	[Category( "Stats" )]
	public float GetBoostRechargeCooldown()
	{
		float rechargeCooldown = GetStats().BoostRechargeCooldown;
		return rechargeCooldown;
	}
	[Category( "Stats" )]
	public float GetBoostRechargeFactor()
	{
		float rechargeFactor = GetStats().BoostRechargeFactor;
		return rechargeFactor;
	}
	[Category( "Stats" )]
	public int GetMaxHealth()
	{
		int maxHealth = GetStats().MaxHealth;
		return maxHealth;
	}
	[Category( "Stats" )]
	public int GetHalfHealth()
	{
		return (int)MathF.Floor( GetMaxHealth() / 2f );
	}
	[Category( "Stats" )]
	public float GetTurnSpeed()
	{ 
		return GetStats().TurnSpeed; 
	}
	[Category( "Stats" )]
	public float GetTurnFactor(float speed)
	{
		float speedFraction = speed / GetMaxSpeed();
		return GetStats().TurnFactorCurve.Evaluate( speedFraction );
	}
	[Category("Stats")]
	public float GetSpringStrength()
	{
		return GetStats().SpringStrength;
	}
	[Category( "Stats" )]
	public float GetSpringDamping()
	{
		return GetStats().SpringDamping;
	}
	[Category("Stats")]
	public float GetGrip()
	{
		return GetStats().Grip;
	}
	[Category("Stats")]
	public float GetSlideGrip(float velocity, float maxVelocity = 512f)
	{
		float velocityFraction = MathF.Abs(velocity) / maxVelocity;
		return GetStats().SlidingGripCurve.Evaluate( velocityFraction );
	}
	[Category("Stats")]
	public float GetAngularDamping()
	{
		return GetStats().AngularDamping;
	}

	public Vector3 GetCameraPositionOffset()
	{
		return GetStats().CameraPositionOffset;
	}
	[Category( "Stats" )]
	public List<ItemDefinition> GetVehicleItems()
	{
		return GetStats().BonusItems;
	}
}

public class VehicleStatModifier
{
	public VehicleStats Stats { get; set; }
	public VehicleStatChangeMode Mode { get; set; }

	public VehicleStatModifier( VehicleStats stats, VehicleStatChangeMode mode )
	{
		Stats = stats;
		Mode = mode;
	}
}
