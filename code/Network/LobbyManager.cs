
using System.Collections.Generic;
using System.Linq;

namespace Bydrive;
[Icon( "hub" )]
public class LobbyManager : Component, Component.INetworkListener
{
	public static bool LobbyActive => Instance != null && Instance.Enabled;
	public static LobbyManager Instance { get; private set; }
	public IReadOnlyList<Player> Players => connectedPlayers;
	private List<Player> connectedPlayers = new();
	protected override void OnEnabled()
	{
		if(Instance != null && Instance != this)
		{
			foreach ( var user in Instance.Players )
				connectedPlayers.Add( user );

			Instance.Destroy();
		}
		Instance = this;
	}

	void INetworkListener.OnActive(Connection connection)
	{
		Player ply = new( connection );
		connectedPlayers.Add( ply );
	}

	void INetworkListener.OnDisconnected(Connection connection)
	{
		Player ply = connectedPlayers.FirstOrDefault(p => p.Connection == connection);
		if(ply != null)
		{
			connectedPlayers.Remove(ply);
		}
	}
}
