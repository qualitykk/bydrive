using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

internal static class GameObjectExtensions
{
	public static void ApplyPrefab(this GameObject obj, string prefab)
	{
		obj.SetPrefabSource(prefab);
		obj.UpdateFromPrefab();
		obj.BreakFromPrefab();
	}

	public static void ApplyPrefab( this GameObject obj, PrefabFile prefab ) => ApplyPrefab( obj, prefab.ResourcePath );
}

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

internal static class ResourceExtensions
{
	public static GameObject CreateObject<T>(this T instance ) where T : GameResource, IPrefabProvider
	{
		return ResourceHelper.CreateObjectFromResource( instance );
	}
}
