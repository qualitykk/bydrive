using Sandbox.Diagnostics;
using Sandbox.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class Player : Component
{
	public static Player Create( Connection connection )
	{
		GameObject playerObject = new();
		//playerObject.Networked = true;
		playerObject.Name = $"Player | {connection.DisplayName}";
		playerObject.Networked = true;

		var ply = playerObject.Components.Create<Player>();
		playerObject.NetworkSpawn( connection );

		return ply;
	}

	public static Player CreateLocal()
	{
		Assert.False( GameNetworkSystem.IsActive || GameNetworkSystem.IsConnecting, "Cant create local dummy player in multiplayer games!" );

		GameObject playerObject = new();
		playerObject.Name = $"Player | LOCAL";

		var ply = playerObject.Components.Create<Player>();
		ply.DisplayName = GetLocalName();
		return ply;
	}

	public static Player CreateBot()
	{
		GameObject playerObject = new();
		playerObject.Name = $"Player | BOT";
		playerObject.Networked = true;
		var ply = playerObject.Components.Create<Player>();
		ply.IsBot = true;
		playerObject.NetworkSpawn();

		return ply;
	}
	public static Player Local => LobbyManager.Instance?.LocalPlayer ?? CreateLocal();
	public Connection Connection => Network.OwnerConnection;
	public string Name => DisplayName ?? Connection?.DisplayName ?? Connection?.Name ?? "Player";
	public ulong SteamId => Connection?.SteamId ?? 0;
	[Property, Sync] public string DisplayName { get; set; }
	[Property, Sync] public bool IsBot { get; set; }
	public bool IsHost => IsLocal || Connection?.IsHost == true;
	public bool IsLocal => Connection == null || Game.SteamId == (long)Connection.SteamId;
	public VehicleDefinition SelectedVehicle { get; set; }
	public RaceDefinition SelectedTrack { get; set; }
	protected override void OnAwake()
	{
		GameObject.Flags |= GameObjectFlags.DontDestroyOnLoad;
	}
	public string GetAvatar()
	{
		return $"avatar:{SteamId}";
	}
	public override string ToString() => Name;
}
