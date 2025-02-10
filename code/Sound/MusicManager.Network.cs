using Sandbox.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class MusicManager
{
	#region Play
	public static void PlayAll( Soundtrack track, float volume = -1f )
	{
		Instance?.Play( track, volume );
	}
	public static void PlayLocal( Soundtrack track, float volume = -1f )
	{
		using var _ = NetworkFilter.Local();

		Instance?.Play( track, volume );
	}
	public static void PlaySingle( Player ply, Soundtrack track, float volume = -1f )
	{
		using var _ = NetworkFilter.Player( ply );

		Instance?.Play( track, volume );
	}
	public static void PlayMulti( IEnumerable<Player> plys, Soundtrack track, float volume = -1f )
	{
		using var _ = NetworkFilter.Players( plys );

		Instance?.Play( track, volume );
	}
	#endregion Play

	#region Stop
	public static void StopAll()
	{
		Instance?.Stop();
	}
	public static void StopLocal()
	{
		using var _ = NetworkFilter.Local();

		Instance?.Stop();
	}
	public static void StopSingle( Player ply )
	{
		using var _ = NetworkFilter.Player( ply );

		Instance?.Stop();
	}

	public static void StopMulti( IEnumerable<Player> plys )
	{
		using var _ = NetworkFilter.Players( plys );

		Instance?.Stop();
	}

	#endregion Stop

	#region Processors
	public static void AddProcessorAll(AudioProcessor processor)
	{
		Instance?.AddProcessor( processor );
	}
	public static void AddProcessorLocal( AudioProcessor processor )
	{
		using var _ = NetworkFilter.Local();

		Instance?.AddProcessor( processor );
	}
	public static void AddProcessorSingle(Player ply, AudioProcessor processor )
	{
		using var _ = NetworkFilter.Player( ply );

		Instance?.AddProcessor( processor );
	}
	public static void AddProcessorMulti( IEnumerable<Player> plys, AudioProcessor processor )
	{
		using var _ = NetworkFilter.Players( plys );

		Instance?.AddProcessor( processor );
	}

	public static void RemoveProcessorAll( AudioProcessor processor )
	{
		Instance?.RemoveProcessor( processor );
	}
	public static void RemoveProcessorLocal( AudioProcessor processor )
	{
		using var _ = NetworkFilter.Local();

		Instance?.RemoveProcessor( processor );
	}

	public static void RemoveProcessorSingle( Player ply, AudioProcessor processor )
	{
		using var _ = NetworkFilter.Player( ply );

		Instance?.RemoveProcessor( processor );
	}
	public static void RemoveProcessorMulti( IEnumerable<Player> plys, AudioProcessor processor )
	{
		using var _ = NetworkFilter.Players( plys );

		Instance?.RemoveProcessor( processor );
	}

	public static void ClearProcessorsAll()
	{
		Instance?.ClearProcessors();
	}
	public static void ClearProcessorsLocal()
	{
		using var _ = NetworkFilter.Local();

		Instance?.ClearProcessors();
	}

	public static void ClearProcessorsSingle( Player ply )
	{
		using var _ = NetworkFilter.Player( ply );

		Instance?.ClearProcessors();

	}
	public static void ClearProcessorsMulti( IEnumerable<Player> plys )
	{
		using var _ = NetworkFilter.Players( plys );

		Instance?.ClearProcessors();
	}
	#endregion Processors
}
