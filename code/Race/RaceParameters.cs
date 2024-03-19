using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class RaceParameters
{
	public const int DEFAULT_MAX_LAPS = 3;
	public int MaxLaps { get; set; } = DEFAULT_MAX_LAPS;
	public RaceMode Mode { get; set; }
	public RaceParameters()
	{
		MaxLaps = DEFAULT_MAX_LAPS;
	}

	public override bool Equals( object obj )
	{
		return obj is RaceParameters parameters &&
			   MaxLaps == parameters.MaxLaps &&
			   Mode == parameters.Mode;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine( MaxLaps, Mode );
	}

	public static bool operator ==( RaceParameters left, RaceParameters right )
	{
		return EqualityComparer<RaceParameters>.Default.Equals( left, right );
	}

	public static bool operator !=( RaceParameters left, RaceParameters right )
	{
		return !(left == right);
	}
}
