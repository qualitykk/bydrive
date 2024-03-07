using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class UpgradePage
{
	const string GROUP_PLAYER = "__PLAYER";
	const string GROUP_TEAM = "__TEAM";

	string selectedGroup = GROUP_PLAYER;
	UpgradeDefinition selectedUpgrade;
	private IEnumerable<VehicleDefinition> GetUpgradableVehicles()
	{
		return ResourceLibrary.GetAll<VehicleDefinition>();
	}

	private IEnumerable<UpgradeDefinition> GetUpgrades()
	{
		if(string.IsNullOrWhiteSpace(selectedGroup))
		{
			return null;
		}

		var upgrades = ResourceLibrary.GetAll<UpgradeDefinition>();

		if(selectedGroup == GROUP_PLAYER)
		{
			return upgrades.Where( u => u.Target == UpgradeTarget.Player );
		}
		else if(selectedGroup == GROUP_TEAM)
		{
			return upgrades.Where( u => u.Target == UpgradeTarget.Team );
		}
		else
		{
			return upgrades.Where(u => u.Target == UpgradeTarget.Vehicle && u.TargetVehicle?.ResourcePath == selectedGroup);
		}
	}

	private string GetClasses(UpgradeDefinition upgrade)
	{
		string classes = "";
		if ( Unlocked( upgrade ) )
			classes += "unlocked ";
		else if ( CanBuy( upgrade ) )
			classes += "buyable ";

		return classes;
	}
	private bool Unlocked(UpgradeDefinition upgrade)
	{
		return CurrentSave != null && CurrentSave.UpgradeUnlocked( upgrade );
	}
	private bool CanBuy(UpgradeDefinition upgrade)
	{
		return CurrentSave != null && CurrentSave.CanBuy( upgrade );
	}
	private void TryBuySelected()
	{
		if ( CurrentSave == null )
			return;

		CurrentSave.UpgradeBuy( selectedUpgrade );
	}

	private int GetMoney()
	{
		return CurrentSave?.Money.FloorToInt() ?? 0;
	}

	private int GetTokens()
	{
		return CurrentSave?.Tokens ?? 0;
	}

	void OnClickUpgrade(UpgradeDefinition definition)
	{
		selectedUpgrade = definition;
	}

	void OnClickGroup(string id)
	{
		selectedGroup = id;
		selectedUpgrade = null;
	}

	void OnClickBack()
	{
		this.Navigate( "/" );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( selectedGroup, selectedUpgrade, GetUpgrades(), CurrentSave );
	}
}
