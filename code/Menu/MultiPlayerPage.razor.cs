using Sandbox.Network;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class MultiPlayerPage
{
	List<LobbyInformation> lobbies;

	protected override async Task OnParametersSetAsync()
	{
		lobbies = await GameNetworkSystem.QueryLobbies();

		await base.OnParametersSetAsync();
	}
	private void OnClickLobby( LobbyInformation lobby )
	{
		GameNetworkSystem.Connect( lobby.LobbyId );
		this.Navigate( "active" );
	}
	private void OnClickCreate()
	{
		GameNetworkSystem.CreateLobby();
		this.Navigate( "active" );
	}
	private void OnClickRefresh()
	{
		lobbies = null;
		GameTask.RunInThreadAsync( () => { 
			lobbies = GameNetworkSystem.QueryLobbies().Result; 
		} );
	}
	private void OnClickBack()
	{
		StartMenu.Current.NavPanel?.GoBack();
	}

	
	protected override int BuildHash()
	{
		return HashCode.Combine( lobbies );
	}
}
