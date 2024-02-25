using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public struct RaceParameters
{
	public const int DEFAULT_MAX_LAPS = 3;
	public const float RACE_START_WAIT = 4f;
	public int MaxLaps { get; set; } = DEFAULT_MAX_LAPS;
	public SoundEvent RaceMusic { get; set; }
	public float RaceMusicVolume { get; set; } = -1f;
	public float RaceStartWait { get; set; } = RACE_START_WAIT;
	public RaceMode Mode { get; set; }
	public RaceParameters()
	{
		MaxLaps = DEFAULT_MAX_LAPS;
		RaceMusicVolume = -1;
		RaceStartWait = RACE_START_WAIT;
	}
}
