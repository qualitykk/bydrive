using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public static class NetworkFilter
{
	public static IDisposable Players(IEnumerable<Player> players)
	{
		if ( !Networking.IsActive ) return default;
		return Rpc.FilterInclude( players.Select( ply => ply?.Connection ).Distinct() );
	}

	public static IDisposable Player( Player player )
	{
		if ( player?.Connection == null || !Networking.IsActive ) return default;
		return Rpc.FilterInclude( player.Connection );
	}

	public static IDisposable Local()
	{
		if ( Networking.IsActive ) return default;

		return Player( Bydrive.Player.Local );
	}
}
