using Sandbox.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public static class GameSound
{
	public static SoundHandle Play( string name, GameSoundChannel channel = GameSoundChannel.None, float volume = -1f ) => Play( name, Vector3.Zero, channel, volume );
	public static SoundHandle Play( string name, Vector3 position, GameSoundChannel channel = GameSoundChannel.None, float volume = -1f )
	{
		return Play( ResourceLibrary.Get<SoundEvent>( name ), position, channel, volume );
	}
	public static SoundHandle Play(SoundEvent sound, GameSoundChannel channel = GameSoundChannel.None, float volume = -1f ) => Play(sound, Vector3.Zero, channel, volume );
	public static SoundHandle Play( SoundEvent sound, Vector3 position, GameSoundChannel channel = GameSoundChannel.None, float volume = -1f )
	{
		float trackVolume = volume > 0 ? volume : 1;
		trackVolume *= GetSoundChannelVolume(channel);
		var soundInstance = Sound.Play( sound, position );
		soundInstance.Volume = trackVolume;

		return soundInstance;
	}

	private static float GetSoundChannelVolume(GameSoundChannel channel)
	{
		switch(channel )
		{
			case GameSoundChannel.Music:
				return Settings.MusicVolume;
			case GameSoundChannel.Effect:
				return Settings.SoundEffectVolume;
			case GameSoundChannel.Vehicle:
				return Settings.VehicleVolume;
		}

		return 1f;
	}
}

public enum GameSoundChannel
{
	None,
	Vehicle,
	Effect,
	Music
}
