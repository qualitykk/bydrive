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
		Assert.True( LobbyManager.MultiplayerActive, "Cant create other player object in single player mode!" );

		GameObject playerObject = new();
		playerObject.NetworkMode = NetworkMode.Object;
		playerObject.Name = $"Player | {connection.Name}";

		var ply = playerObject.Components.Create<Player>();
		playerObject.NetworkSpawn( connection );

		return ply;
	}

	public static Player CreateLocal()
	{
		Assert.False( Networking.IsActive || Networking.IsConnecting, "Cant create local dummy player in multiplayer games!" );

		GameObject playerObject = new();
		playerObject.Name = $"Player | LOCAL";

		var ply = playerObject.Components.Create<Player>();
		ply.DisplayName = GetLocalName();
		return ply;
	}

	public static Player CreateBot(string name = "")
	{
		GameObject playerObject = new();
		if(LobbyManager.MultiplayerActive)
		{
			playerObject.NetworkMode = NetworkMode.Object;
		}
		var ply = playerObject.Components.Create<Player>();
		ply.IsBot = true;

		if(string.IsNullOrEmpty(name))
		{
			playerObject.Name = $"Player | BOT";
		}
		else
		{
			playerObject.Name = $"Player | {name} (BOT)";
			ply.DisplayName = name;
		}

		if(Networking.IsActive)
		{
			playerObject.NetworkSpawn();
		}

		return ply;
	}
	public static Player Local => LobbyManager.Instance?.LocalPlayer ?? CreateLocal();
	public Connection Connection => Network.Owner;
	public string Name => DisplayName ?? Connection?.DisplayName ?? "Player";
	public ulong SteamId => Connection?.SteamId ?? (ulong)0;
	[Property, Sync] public string DisplayName { get; set; }
	[Property, Sync(SyncFlags.FromHost)] public bool IsBot { get; set; }
	public bool IsHost => IsLocal || Connection?.IsHost == true;
	public bool IsLocal => Connection == null || Game.SteamId == (long)Connection.SteamId;
	public VehicleDefinition SelectedVehicle { get; set; }
	public TrackDefinition SelectedTrack { get; set; }
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
