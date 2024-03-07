using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[GameResource( "Story Upgrade", "upgrade", "Upgrade for story mode." )]
public class UpgradeDefinition : GameResource, IEconomyItem
{
	public delegate void ApplyUpgrade( VehicleController vehicle );
	public string Title { get; set; } = "Untitled Upgrade";
	[TextArea] public string Description { get; set; } = "No Description set";
	public UpgradeTarget Target { get; set; }
	[ShowIf("Target", UpgradeTarget.Vehicle)]
	public VehicleDefinition TargetVehicle { get; set; }
	public ApplyUpgrade OnRaceStart { get; set; }
	[Category("Unlock")] public float MoneyCost { get;set; }
	[Category( "Unlock" )] public int TokenCost { get;set; }
	[Category("Unlock")] public Story.UnlockCheck CanUnlock { get; set; }
	[Category( "Unlock" ), TextArea] public string UnlockCriteriaDisplay { get; set; }
	[Hide] Story.UnlockCheck IEconomyItem.CanPurchase => CanUnlock;
}

public enum UpgradeTarget
{
	Player,
	Team,
	Vehicle
}
