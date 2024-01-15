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
	IEnumerable<Player> players;
	public override void Tick()
	{
		if(!GameNetworkSystem.IsActive && !GameNetworkSystem.IsConnecting)
		{
			this.Navigate( "/front" );
		}
	}
	private void OnClickStart()
	{
		Log.Info( "Start" );
	}
	private void OnClickRefresh()
	{
		players = LobbyManager.Instance?.Players;
	}
	private void OnClickBack()
	{
		GameNetworkSystem.Disconnect();
		StartMenu.Current.NavPanel?.GoBack();
	}
	
	/*
	protected override int BuildHash()
	{
		return HashCode.Combine(  );
	}
	*/
}
