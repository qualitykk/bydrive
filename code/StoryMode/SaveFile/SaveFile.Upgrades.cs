using Sandbox.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class SaveFile
{
	private Dictionary<string, bool> unlockedUpgrades { get; set; } = new();
	public IEnumerable<UpgradeDefinition> GetUnlockedUpgrades()
	{
		return unlockedUpgrades.Where(u => u.Value).Select(kv => ResourceLibrary.Get<UpgradeDefinition>(kv.Key));
	}
	public bool UpgradeUnlocked(UpgradeDefinition upgrade)
	{
		string key = upgrade.ResourcePath;
		return unlockedUpgrades.ContainsKey( key ) && unlockedUpgrades[key];
	}
	public void UpgradeUnlock(UpgradeDefinition upgrade)
	{
		if ( UpgradeUnlocked( upgrade ) )
			return;

		string key = upgrade.ResourcePath;
		if(unlockedUpgrades.ContainsKey( key ) )
		{
			unlockedUpgrades[key] = true;
		}
		else
		{
			unlockedUpgrades.Add(key, true);
		}

		OnUpgradeUnlocked( upgrade );
	}

	public void UpgradeBuy(UpgradeDefinition upgrade)
	{
		if ( !Buy( upgrade ) )
			return;

		UpgradeUnlock( upgrade );
	}
	private void OnUpgradeUnlocked(UpgradeDefinition upgrade)
	{
		Popup.Add( new PopupPage( "Upgrade Unlocked!", $"Unlocked the upgrade {upgrade.Title}!", UI.Colors.Popup.Positive ) );
	}
}
