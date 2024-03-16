using Sandbox.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public static class GameSound
{
	private class SoundInstance
	{
		public SoundHandle Handle { get; set; }
		public GameSoundChannel Channel { get; set; }
		public float Volume { get; set;}

		public SoundInstance( SoundHandle handle, GameSoundChannel channel, float volume )
		{
			Handle = handle;
			Channel = channel;
			Volume = volume;
		}
	}
	private static readonly List<SoundInstance> sounds = new();
	public static SoundHandle Play( string name, GameSoundChannel channel = GameSoundChannel.None, float volume = -1f ) => Play( name, Vector3.Zero, channel, volume );
	public static SoundHandle Play( string name, Vector3 position, GameSoundChannel channel = GameSoundChannel.None, float volume = -1f )
	{
		return Play( ResourceLibrary.Get<SoundEvent>( name ), position, channel, volume );
	}
	[ActionGraphNode( "gamesound.play_noposition" )]
	[Group("Sound"), Title("Play Sound (Channel")]
	public static SoundHandle Play(SoundEvent sound, GameSoundChannel channel = GameSoundChannel.None, float volume = -1f ) => Play(sound, Vector3.Zero, channel, volume );
	[Group( "Sound" ), Title( "Play Sound (Channel + Position)" )]
	[ActionGraphNode("gamesound.play_full")]
	public static SoundHandle Play( SoundEvent sound, Vector3 position, GameSoundChannel channel = GameSoundChannel.None, float volume = -1f )
	{
		float instanceVolume = sound.Volume.GetValue();
		if ( volume >= 0 )
			instanceVolume *= volume;

		var soundInstance = Sound.Play( sound, position );
		sounds.Add( new(soundInstance, channel, instanceVolume ));

		Update();

		return soundInstance;
	}

	public static void Update()
	{
		if ( !sounds.Any() ) return;
		foreach(SoundInstance sound in sounds)
		{
			float volume = GetSoundChannelVolume( sound.Channel );
			if ( sound.Volume >= 0 )
				volume *= sound.Volume;

			sound.Handle.Volume = volume;
		}
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
