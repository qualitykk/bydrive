using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

internal static class TransformExtensions
{
	public static Vector3 VelocityToLocal( this Transform transform, Vector3 velocity )
	{
		return transform.PointToLocal( transform.Position + velocity );
	}
}

internal static class NumberExtensions
{
	public static string FormatAsRaceTime(this float time)
	{
		var timeSpan = TimeSpan.FromSeconds( time );
		return timeSpan.ToString( @"mm\:ss\:ff" );
	}

	public static string FormatAsRaceTime( this TimeSince time ) => FormatAsRaceTime( time.Relative );
}
