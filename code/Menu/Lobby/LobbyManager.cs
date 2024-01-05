
using System.Collections.Generic;

namespace Bydrive;
public class LobbyManager : Component, Component.INetworkListener
{
	public IReadOnlyList<Connection> Users => connectedUsers;
	private List<Connection> connectedUsers = new();
	void INetworkListener.OnActive(Connection connection)
	{
		connectedUsers.Add(connection);
	}

	void INetworkListener.OnDisconnected(Connection connection)
	{
		connectedUsers.Remove(connection);
	}
}
