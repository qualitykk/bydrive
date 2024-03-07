using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;
public partial class SaveFile
{
	/// <summary>
	/// Money in €, gained mainly from side content.
	/// </summary>
	public float Money { get; set; }
	/// <summary>
	/// League Tokens, used for upgrades. Gained from main story.
	/// </summary>
	public int Tokens { get; set; }
	/// <summary>
	/// Vehicle Licenses. Used to unlock new vehicles.
	/// </summary>
	public int Licenses { get; set; }

	public bool CanBuy(UpgradeDefinition upgrade)
	{
		bool canBuy = Money >= upgrade.MoneyCost && Tokens >= upgrade.TokenCost;
		if(upgrade.CanUnlock != null)
		{
			canBuy &= upgrade.CanUnlock.Invoke( this );
		}

		return canBuy;
	}

	[ConCmd("st_econ_setmoney")]
	private static void Command_SetMoney(float amount)
	{
		if(!Story.Active)
		{
			Log.Warning( "No save file loaded!" );
			return;
		}

		CurrentSave.Money = amount;
	}

	[ConCmd( "st_econ_settokens" )]
	private static void Command_SetTokens(int amount)
	{
		if ( !Story.Active )
		{
			Log.Warning( "No save file loaded!" );
			return;
		}

		CurrentSave.Tokens = amount;
	}

	[ConCmd( "st_econ_setlicenses" )]
	private static void Command_SetLicenses( int amount )
	{
		if ( !Story.Active )
		{
			Log.Warning( "No save file loaded!" );
			return;
		}

		CurrentSave.Licenses = amount;
	}
}
