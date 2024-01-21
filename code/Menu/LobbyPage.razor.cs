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
	int playerCount = RaceMatchInformation.MAX_PLAYERCOUNT;
	RaceDefinition selectedTrack;
	IEnumerable<Player> players => LobbyManager.Instance?.Players;
	public override void Tick()
	{
		if ( StartMenu.Current?.NavPanel?.CurrentPanel != this )
			return;

		if(!GameNetworkSystem.IsActive && !GameNetworkSystem.IsConnecting)
		{
			this.Navigate( "/front" );
		}
	}
	protected override void OnParametersSet()
	{
		Refresh();
		selectedTrack = ResourceLibrary.GetAll<RaceDefinition>().First();
	}
	private void Refresh()
	{

	}
	private void OnClickStart()
	{
		List<RaceMatchInformation.Participant> racers = new();
		int i = 1;
		foreach(var ply in players)
		{
			VehicleDefinition playerVehicle = ply.SelectedVehicle ?? StartMenu.GetDefaultVehicle();
			racers.Add( new( playerVehicle, ply, i) );
			//Log.Info( $"Start {ply} {playerVehicle}" );
			i++;
		}

		StartRace.Online( selectedTrack, racers );
	}
	private void OnClickRefresh()
	{
		Refresh();
	}
	private void OnClickBack()
	{
		GameNetworkSystem.Disconnect();
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
