using Sandbox.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sandbox.VertexLayout;

namespace Bydrive;

public class SoundManager : SingletonComponent<SoundManager>
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
	private readonly List<SoundInstance> sounds = new();
	[Property] public Dictionary<GameSoundChannel, MixerHandle> Mixers { get; set; } = new();
	public SoundHandle Play( string name, GameSoundChannel channel = GameSoundChannel.None, Vector3 position = default, float volume = -1f ) => Play( ResourceLibrary.Get<SoundEvent>( name ), channel, position, volume );
	[Group( "Sound" ), Title( "Play Sound" )]
	[ActionGraphNode("gamesound.play")]
	public SoundHandle Play( SoundEvent sound, GameSoundChannel channel = GameSoundChannel.None, Vector3 position = default, float volume = -1f )
	{
		float instanceVolume = sound.Volume.GetValue();
		if ( volume >= 0 )
			instanceVolume *= volume;

		var soundInstance = Sound.Play( sound, position );
		sounds.Add( new(soundInstance, channel, instanceVolume ));

		Update();

		return soundInstance;
	}

	public void Update()
	{
		if ( !sounds.Any() ) return;
		if(Mixers != null && Mixers.Count > 0)
		{
			foreach ( (GameSoundChannel channel, MixerHandle handle) in Mixers )
			{
				var mixer = handle.GetOrDefault();
				mixer.Volume = GetSoundChannelVolume( channel );
			}
		}
		
		foreach(SoundInstance sound in sounds)
		{
			float volume = GetSoundChannelVolume( sound.Channel );
			if ( sound.Volume >= 0 )
				volume *= sound.Volume;

			sound.Handle.Volume = volume;
		}
	}

	private float GetSoundChannelVolume(GameSoundChannel channel)
	{
		switch( channel )
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
