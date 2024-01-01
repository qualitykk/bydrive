using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redrome;

public static class Music
{
	static SoundHandle currentTrack;
	public static void Play(string name)
	{
		Stop();
		Sound.Play(name);
	}

	public static void Play(SoundEvent sound)
	{
		Stop();
		currentTrack = Sound.Play( sound );
		currentTrack.ListenLocal = true;
	}

	public static void Stop()
	{
		currentTrack?.Stop();
	}
}
