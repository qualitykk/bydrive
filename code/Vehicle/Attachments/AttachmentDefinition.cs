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
	[TextArea] public string Description { get; set; }
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
	/// Increase by percentage relative to base stats (1200 vs 900 base = 30% increase, etc)
	/// </summary>
	PercentRelative,
	/// <summary>
	/// Percentage increase with 0 = 100%, 0.5 = 150%, -0.5 = 50% etc.
	/// </summary>
	PercentRaw,
	Absolute,
	Override
}
public static class VehicleStatExtensions
{
	public static VehicleStats WithChanges(this VehicleStats stats, VehicleStats changes, VehicleStatChangeMode mode = VehicleStatChangeMode.PercentRaw)
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
			stats.MaxHealth += defaultStats.MaxHealth - changes.MaxHealth;

			stats.TurnSpeed += defaultStats.TurnSpeed - changes.TurnSpeed;

			stats.BoostRechargeCooldown += defaultStats.BoostRechargeCooldown - changes.BoostRechargeCooldown;
			stats.BoostRechargeFactor += defaultStats.BoostRechargeFactor - changes.BoostRechargeFactor;
			stats.BoostSpeedMultiplier += defaultStats.BoostSpeedMultiplier - changes.BoostSpeedMultiplier;
			stats.BoostAccelerationMultiplier += defaultStats.BoostAccelerationMultiplier - changes.BoostAccelerationMultiplier;

		}
		else if(mode == VehicleStatChangeMode.PercentRaw)
		{
			stats.MaxSpeed *= 1 + changes.MaxSpeed;
			stats.MaxHealth += changes.MaxHealth;
			stats.TurnSpeed *= 1 + changes.TurnSpeed;
			stats.BoostRechargeFactor *= 1 + changes.BoostRechargeFactor;
			stats.BoostSpeedMultiplier *= 1 + changes.BoostSpeedMultiplier;
			stats.BoostAccelerationMultiplier *= 1 + changes.BoostAccelerationMultiplier;
		}
		else
		{
			stats.MaxSpeed *= changes.MaxSpeed / defaultStats.MaxSpeed;
			stats.MaxHealth *= changes.MaxHealth / defaultStats.MaxHealth;

			stats.TurnSpeed *= changes.TurnSpeed / defaultStats.TurnSpeed;

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
