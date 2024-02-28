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

		if(mode == VehicleStatChangeMode.Override)
		{
			return changes;
		}
		else if(mode == VehicleStatChangeMode.Absolute)
		{
			stats.MaxSpeed += changes.MaxSpeed;
			stats.Acceleration += changes.Acceleration;
			stats.MaxHealth += changes.MaxHealth;

			stats.TurnSpeed += changes.TurnSpeed;
			stats.TurnSpeedIdealDistance += changes.TurnSpeedIdealDistance;
			stats.TurnSpeedVelocityFactor += changes.TurnSpeedVelocityFactor;

			stats.BoostRechargeCooldown += changes.BoostRechargeCooldown;
			stats.BoostRechargeFactor += changes.BoostRechargeFactor;
			stats.BoostSpeedMultiplier += changes.BoostSpeedMultiplier;
			stats.BoostAccelerationMultiplier += changes.BoostAccelerationMultiplier;

		}
		else
		{
			stats.MaxSpeed *= 1 + changes.MaxSpeed;
			stats.Acceleration *= 1 + changes.Acceleration;
			stats.MaxHealth *= 1 + changes.MaxHealth;

			stats.TurnSpeed *= 1 + changes.TurnSpeed;
			stats.TurnSpeedIdealDistance *= 1 + changes.TurnSpeedIdealDistance;
			stats.TurnSpeedVelocityFactor *= 1 + changes.TurnSpeedVelocityFactor;

			stats.BoostRechargeCooldown *= 1 + changes.BoostRechargeCooldown;
			stats.BoostRechargeFactor *= 1 + changes.BoostRechargeFactor;
			stats.BoostSpeedMultiplier *= 1 + changes.BoostSpeedMultiplier;
			stats.BoostAccelerationMultiplier *= 1 + changes.BoostAccelerationMultiplier;
		}

		stats.BonusItems.AddRange( changes.BonusItems );

		return stats;
	}

	public static VehicleStats WithChanges( this VehicleStats stats, AttachmentDefinition attachment ) => WithChanges( stats, attachment.StatChanges, attachment.StatChangeMode );
}
