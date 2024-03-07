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

	string selectedGroup;
	UpgradeDefinition selectedUpgrade;
	private IEnumerable<VehicleDefinition> GetUpgradableVehicles()
	{
		return ResourceLibrary.GetAll<VehicleDefinition>();
	}

	private IEnumerable<UpgradeDefinition> GetUpgrades()
	{
		return ResourceLibrary.GetAll<UpgradeDefinition>();
	}

	private string GetClasses(UpgradeDefinition upgrade)
	{
		string classes = "";
		if ( CanBuy( upgrade ) )
			classes += "buyable ";

		return classes;
	}

	private bool CanBuy(UpgradeDefinition upgrade)
	{
		return CurrentSave != null && CurrentSave.CanBuy( upgrade );
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

	void OnClickBack()
	{
		this.Navigate( "/" );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( selectedGroup, selectedUpgrade, GetUpgrades(), CurrentSave );
	}
}
