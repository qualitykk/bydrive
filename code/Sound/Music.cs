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
	static float currentVolume;
	public static void Play( string name, float volume = -1f)
	{
		Play(ResourceLibrary.Get<SoundEvent>(name), volume);
	}

	public static void Play(SoundEvent sound, float volume = -1f)
	{
		Stop();
		if(sound == null)
		{
			Log.Warning( "Tried to play null sound for music!" );
			return;
		}

		float trackVolume = volume > 0 ? volume : 1;
		trackVolume *= Settings.MusicVolume;
		if ( trackVolume > 0)
		{
			currentTrack = Sound.Play( sound );
			if ( currentTrack.IsValid() )
			{
				currentTrack.Volume = trackVolume;
				currentTrack.ListenLocal = true;
			}
		}

		currentVolume = volume;
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
