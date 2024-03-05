using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class TrackMusicParameters : IEqualityOperators<TrackMusicParameters, TrackMusicParameters, bool>
{
	public const float RACE_START_WAIT = 4f;
	public static readonly TrackMusicParameters Default = new();
	public SoundEvent RaceMusic { get; set; }
	public float RaceMusicVolume { get; set; } = -1f;
	public float RaceStartWait { get; set; } = RACE_START_WAIT;
	public TrackMusicParameters()
	{
		RaceMusicVolume = -1;
		RaceStartWait = RACE_START_WAIT;
	}

	public override bool Equals( object obj )
	{
		return obj is TrackMusicParameters parameters &&
			   EqualityComparer<SoundEvent>.Default.Equals( RaceMusic, parameters.RaceMusic ) &&
			   RaceMusicVolume == parameters.RaceMusicVolume &&
			   RaceStartWait == parameters.RaceStartWait;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine( RaceMusic, RaceMusicVolume, RaceStartWait );
	}

	public static bool operator ==( TrackMusicParameters left, TrackMusicParameters right )
	{
		return left.Equals( right );
	}

	public static bool operator !=( TrackMusicParameters left, TrackMusicParameters right )
	{
		return !left.Equals( right );
	}
}
