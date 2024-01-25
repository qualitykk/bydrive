using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static Sandbox.GameObject;

namespace Bydrive;

internal static class GameObjectExtensions
{
	public static void ApplyPrefab(this GameObject obj, string prefab)
	{
		obj.SetPrefabSource(prefab);
		obj.UpdateFromPrefab();
		obj.BreakFromPrefab();
	}

	public static void ApplyPrefab( this GameObject obj, PrefabFile prefab ) => ApplyPrefab( obj, prefab?.ResourcePath );
}

internal static class TransformExtensions
{
	public static Vector3 VelocityToLocal( this Transform transform, Vector3 velocity )
	{
		return transform.PointToLocal( transform.Position + velocity );
	}

	public static Vector3 VelocityToWorld(this Transform transform, Vector3 velocity)
	{
		return velocity * transform.Rotation.Normal;
	}

	public static Transform EnsureNotNaN(this Transform transform)
	{
		if ( transform.Position.IsNaN )
			transform.Position = Vector3.Zero;

		var rot = transform.Rotation;
		if ( float.IsNaN( rot.x ) || float.IsNaN( rot.y ) || float.IsNaN( rot.z ) || float.IsNaN( rot.w ) )
			transform.Rotation = Rotation.Identity;

		return transform;
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
