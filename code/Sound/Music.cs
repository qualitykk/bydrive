using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public static class Music
{
	static SoundHandle currentTrack;
	static string currentTrackName;
	public static void Play(string name)
	{
		Music.Play(ResourceLibrary.Get<SoundEvent>(name));
	}

	public static void Play(SoundEvent sound)
	{
		Stop();
		currentTrack = Sound.Play( sound );
		currentTrack.ListenLocal = true;
		currentTrackName = sound.ResourcePath;
	}

	/// <summary>
	/// Only plays track if it isnt playing already
	/// </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public static bool TryPlay(string name)
	{
		if ( IsPlaying( name ) ) return false;
		Play( name );
		return true;
	}
	/// <summary>
	/// Only plays track if it isnt playing already
	/// </summary>
	/// <param name="sound"></param>
	/// <returns></returns>
	public static bool TryPlay(SoundEvent sound)
	{
		if(IsPlaying( sound ) ) return false;
		Play( sound );
		return true;
	}
	public static bool IsPlaying(string name)
	{
		return currentTrack != null && currentTrackName == name;
	}

	public static bool IsPlaying( SoundEvent sound ) => IsPlaying( sound.ResourcePath );

	public static void Stop()
	{
		currentTrack?.Stop();
		currentTrackName = "";
	}
	public static bool TryStop( string name )
	{
		if ( !IsPlaying( name ) ) return false;
		Stop();
		return true;
	}
	public static bool TryStop(SoundEvent sound)
	{
		if ( !IsPlaying( sound ) ) return false;
		Stop();
		return true;
	}
}
