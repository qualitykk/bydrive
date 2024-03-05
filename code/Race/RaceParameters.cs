using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class RaceParameters : IEquatable<RaceParameters>
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
		return Equals( obj as RaceParameters );
	}

	public bool Equals( RaceParameters other )
	{
		return other is not null &&
			   MaxLaps == other.MaxLaps;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine( MaxLaps );
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
