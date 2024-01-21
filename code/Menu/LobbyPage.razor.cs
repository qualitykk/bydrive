using Sandbox.Network;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class LobbyPage : Panel
{
	int playerCount = RaceInformation.MAX_PLAYERCOUNT;
	RaceDefinition selectedTrack;
	IEnumerable<Player> players => LobbyManager.Instance?.Players;
	public override void Tick()
	{
		if ( StartMenu.Current.NavPanel.CurrentPanel != this )
			return;

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
	private void OnTrackSelected( RaceDefinition def )
	{
		selectedTrack = def;
	}
	protected override int BuildHash()
	{
		return HashCode.Combine( LobbyManager.Instance );
	}
}
