using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class Player
{
	public const string UNSET_NAME_PLACEHOLDER = "Unknown Player";
	public string Name => DisplayName ?? Connection?.DisplayName ?? UNSET_NAME_PLACEHOLDER;
	public ulong SteamId => Connection?.SteamId ?? 0;
	public string DisplayName { get; set; }
	public bool IsBot { get; private set; }
	public bool IsHost => IsLocal || Connection.IsHost;
	public bool IsLocal => Connection == null || Game.SteamId == (long)Connection.SteamId;
	public Connection Connection { get; private set; }
	public Player(Connection connection)
	{
		Connection = connection;
	}
	public static Player CreateBot()
	{
		return new( null )
		{
			IsBot = true
		};
	}
	public string GetAvatar()
	{
		return $"avatar:{SteamId}";
	}
}
