using Sandbox.Audio;
using Sandbox.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class MusicManager : SingletonComponent<MusicManager>
{
	[Property] public MixerHandle Mixer { get; set; }
	Soundtrack currentTrack;
	MusicPlayer currentPlayer;
	Mixer currentMixer;

	[Rpc.Broadcast]
	public void Play(Soundtrack track, float volume = -1f)
	{
		if(track == null)
		{
			//Log.Info( "Played null soundtrack, stopping track instead..." );
			return;
		}

		string path = track.ResourcePath.Split('.').FirstOrDefault();
		Assert.NotNull( path, "Cant play soundtrack without sound name!" );

		path += ".mp3"; //FIXME: Try to find files of other file types!
		if ( !FileSystem.Mounted.FileExists( path ) ) return;

		Stop();

		currentTrack = track;
		currentMixer = Mixer.GetOrDefault();
		if(volume != -1f)
		{
			currentMixer.Volume = volume;
		}

		currentPlayer = MusicPlayer.Play( FileSystem.Mounted, path );
		currentPlayer.TargetMixer = currentMixer;
	}

	[Rpc.Broadcast]
	public void Stop()
	{
		if ( currentPlayer == null ) return;

		currentPlayer.Stop();
		currentPlayer = null;
		currentTrack = null;
		ClearProcessors();
	}

	[Rpc.Broadcast]
	public void AddProcessor(AudioProcessor processor )
	{
		currentMixer?.AddProcessor( processor );
	}
	[Rpc.Broadcast]
	public void RemoveProcessor(AudioProcessor processor )
	{
		currentMixer?.RemoveProcessor( processor );
	}
	[Rpc.Broadcast]
	public void ClearProcessors()
	{
		currentMixer?.ClearProcessors();
	}

	protected override void OnFixedUpdate()
	{
		if(currentTrack != null && currentPlayer != null)
		{
			if(currentPlayer.PlaybackTime >= currentTrack.LoopEnd)
			{
				float offset = currentPlayer.PlaybackTime - currentTrack.LoopEnd;
				currentPlayer.Seek( currentTrack.LoopStart + offset );
			}
		}
	}
	protected override void DrawGizmos()
	{
		if(currentPlayer == null ) return;
		Gizmo.Draw.ScreenText( $"Current OST: {currentTrack?.Sound?.ResourceName} ({currentPlayer.PlaybackTime:0.00}s)", new(100f, 50f) );
		Gizmo.Draw.ScreenText( $"- Loop: {currentTrack?.LoopStart:0.00}s-{currentTrack?.LoopEnd:0.00}s", new(100f, 65f) );
	}

	[ConCmd("music_play")]
	internal static void Command_PlayTrack(string path, float volume = -1f)
	{
		if ( Instance == null ) return;

		if(!ResourceLibrary.TryGet<Soundtrack>(path, out var ost))
		{
			return;
		}

		Instance?.Play( ost, volume );
	}

	[ConCmd("music_settime")]
	internal static void Command_SetTrackTime(float time)
	{
		if ( Instance == null || Instance.currentPlayer == null ) return;

		Instance.currentPlayer.Seek( time );
	}
}
