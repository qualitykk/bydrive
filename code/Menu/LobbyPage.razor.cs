using Sandbox.Network;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class LobbyPage
{
	int playerCount = RaceInformation.MAX_PLAYERCOUNT;
	RaceDefinition selectedTrack;
	IEnumerable<Player> players;
	public override void Tick()
	{
		if(!GameNetworkSystem.IsActive && !GameNetworkSystem.IsConnecting)
		{
			this.Navigate( "/front" );
		}
	}
	protected override void OnParametersSet()
	{
		Refresh();
	}
	private void Refresh()
	{
		players = LobbyManager.Instance?.Players;
	}
	private void OnClickStart()
	{
		Log.Info( "Start" );
	}
	private void OnClickRefresh()
	{
		Refresh();
	}
	private void OnClickBack()
	{
		GameNetworkSystem.Disconnect();
		StartMenu.Current.NavPanel?.GoBack();
	}
	protected override int BuildHash()
	{
		return HashCode.Combine( players );
	}
}
