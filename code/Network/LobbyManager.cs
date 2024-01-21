
using Sandbox.Network;
using System.Collections.Generic;
using System.Linq;

namespace Bydrive;
[Icon( "hub" )]
public class LobbyManager : Component, Component.INetworkListener
{
	public static bool MultiplayerEnabled => Instance != null && Instance.Enabled;
	public static bool MultiplayerActive => Instance != null && Instance.Enabled && GameNetworkSystem.IsActive;
	public static bool IsHost => Instance == null || Instance.LocalPlayer?.IsHost == true;
	public static int PlayerCount => Instance?.Players.Count() ?? 0;
	public static int MaxPlayerCount => Instance?.MaxPlayers ?? 1;
	public static LobbyManager Instance { get; private set; }
	[Property] public int MaxPlayers { get; set; } = RaceMatchInformation.MAX_PLAYERCOUNT;
	public IEnumerable<Player> Players => Scene.GetAllComponents<Player>();
	public Player LocalPlayer => Players.Where(p => p.IsLocal).FirstOrDefault();
	protected override void OnAwake()
	{
		GameObject.Flags |= GameObjectFlags.DontDestroyOnLoad;
	}
	protected override void OnEnabled()
	{
		if(Instance != null && Instance != this)
		{
			Instance.Destroy();
		}
		Instance = this;
	}
	protected override void OnUpdate()
	{
		if(!GameNetworkSystem.IsActive && Players.Any())
		{
			DestroyPlayers();
		}
	}
	protected override void OnDestroy()
	{
		if ( Instance == this )
		{
			Instance = null;
			DestroyPlayers();
		}
	}
	private void DestroyPlayers()
	{
		foreach ( var ply in Players.ToArray() )
		{
			ply.GameObject.Destroy();
		}
	}

	void INetworkListener.OnActive(Connection connection)
	{
		if(Players.Count() >= MaxPlayers)
		{
			// TODO: DISCONNECT
			return;
		}

		Player ply = Player.Create(connection);
		ply.GameObject.SetParent(GameObject, false);
	}

	void INetworkListener.OnDisconnected(Connection connection)
	{
		Player ply = Players.FirstOrDefault(p => p.Connection == connection);
		ply?.GameObject.Destroy();
	}
}
