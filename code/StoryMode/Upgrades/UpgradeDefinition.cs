using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[GameResource( "Story Upgrade", "upgrade", "Upgrade for story mode." )]
public class UpgradeDefinition : GameResource
{
	public delegate void ApplyUpgrade( VehicleController vehicle );
	public string Title { get; set; } = "Untitled Upgrade";
	[TextArea] public string Description { get; set; } = "No Description set";
	public ApplyUpgrade OnRaceStart { get; set; }
	[Category("Unlock")] public float MoneyCost { get;set; }
	[Category( "Unlock" )] public float TokenCost { get;set; }
	[Category("Unlock")] public Story.UnlockCheck CanUnlock { get; set; }
	[Category( "Unlock" ), TextArea] public string UnlockCriteriaDisplay { get; set; }
}
