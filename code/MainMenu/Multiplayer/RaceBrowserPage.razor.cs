using Sandbox.Network;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class RaceBrowserPage
{
	List<LobbyInformation> lobbies;

	protected override async Task OnParametersSetAsync()
	{
		_ = FetchLobbies();

		await base.OnParametersSetAsync();
	}
	private async Task FetchLobbies()
	{
		lobbies = await Networking.QueryLobbies();
	}

	private void OnClickLobby( LobbyInformation lobby )
	{
		GameNetworkSystem.Connect( lobby.LobbyId );
	}
	private void OnClickCreate()
	{
		GameNetworkSystem.CreateLobby();
	}
	private void OnClickRefresh()
	{
		lobbies = null;
		GameTask.RunInThreadAsync( () => { 
			lobbies = Networking.QueryLobbies().Result; 
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
