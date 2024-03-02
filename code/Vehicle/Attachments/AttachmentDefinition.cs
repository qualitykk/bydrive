using Sandbox.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[GameResource( "Vehicle Attachment", "vhatt", "Attachment for a Vehicle" )]
public class AttachmentDefinition : GameResource
{
	public string Title { get; set; }
	public string Description { get; set; }
	public AttachmentSlot Slot { get; set; }
	public Model Model { get; set; }
	public VehicleStats StatChanges { get; set; }
	public VehicleStatChangeMode StatChangeMode { get; set; }
	public Action Attach { get; set; }
	public Action Tick { get; set; }
}
public enum VehicleStatChangeMode
{
	/// <summary>
	/// Percentage increase with 0 = 100%, 0.5 = 150%, -0.5 = 50% etc.
	/// </summary>
	Percent,
	Absolute,
	Override
}
public static class VehicleStatExtensions
{
	public static VehicleStats WithChanges(this VehicleStats stats, VehicleStats changes, VehicleStatChangeMode mode = VehicleStatChangeMode.Percent)
	{
		if ( stats.Equals(default) ) return changes;
		if ( changes.Equals(default) ) return stats;
		VehicleStats defaultStats = new();

		if(mode == VehicleStatChangeMode.Override)
		{
			return changes;
		}
		else if(mode == VehicleStatChangeMode.Absolute)
		{
			stats.MaxSpeed += defaultStats.MaxSpeed - changes.MaxSpeed;
			stats.Acceleration += defaultStats.Acceleration - changes.Acceleration;
			stats.MaxHealth += defaultStats.MaxHealth - changes.MaxHealth;

			stats.TurnSpeed += defaultStats.TurnSpeed - changes.TurnSpeed;
			stats.TurnSpeedIdealDistance += defaultStats.TurnSpeedIdealDistance - changes.TurnSpeedIdealDistance;
			stats.TurnSpeedVelocityFactor += defaultStats.TurnSpeedVelocityFactor - changes.TurnSpeedVelocityFactor;

			stats.BoostRechargeCooldown += defaultStats.BoostRechargeCooldown - changes.BoostRechargeCooldown;
			stats.BoostRechargeFactor += defaultStats.BoostRechargeFactor - changes.BoostRechargeFactor;
			stats.BoostSpeedMultiplier += defaultStats.BoostSpeedMultiplier - changes.BoostSpeedMultiplier;
			stats.BoostAccelerationMultiplier += defaultStats.BoostAccelerationMultiplier - changes.BoostAccelerationMultiplier;

		}
		else
		{
			stats.MaxSpeed *= changes.MaxSpeed / defaultStats.MaxSpeed;
			stats.Acceleration *= changes.Acceleration / defaultStats.Acceleration;
			stats.MaxHealth *= changes.MaxHealth / defaultStats.MaxHealth;

			stats.TurnSpeed *= changes.TurnSpeed / defaultStats.TurnSpeed;
			stats.TurnSpeedIdealDistance *= changes.TurnSpeedIdealDistance / defaultStats.TurnSpeedIdealDistance;
			stats.TurnSpeedVelocityFactor *= changes.TurnSpeedVelocityFactor / defaultStats.TurnSpeedVelocityFactor;

			stats.BoostRechargeCooldown *= changes.BoostRechargeCooldown / defaultStats.BoostRechargeCooldown;
			stats.BoostRechargeFactor *= changes.BoostRechargeFactor / defaultStats.BoostRechargeFactor;
			stats.BoostSpeedMultiplier *= changes.BoostSpeedMultiplier / defaultStats.BoostSpeedMultiplier;
			stats.BoostAccelerationMultiplier *= changes.BoostAccelerationMultiplier / defaultStats.BoostAccelerationMultiplier;
		}

		if(stats.BonusItems != null)
		{
			stats.BonusItems.AddRange( changes.BonusItems );
		}
		else
		{
			stats.BonusItems = changes.BonusItems;
		}

		return stats;
	}

	public static VehicleStats WithChanges( this VehicleStats stats, AttachmentDefinition attachment ) => WithChanges( stats, attachment.StatChanges, attachment.StatChangeMode );
}
