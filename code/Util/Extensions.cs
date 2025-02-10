namespace Bydrive;
public static class CollectionExtensions
{
	public static T Largest<T>( this IEnumerable<T> collection ) where T : IComparable<T>
	{
		if ( collection == default || !collection.Any() ) return default;

		T largest = collection.First();
		foreach ( T item in collection )
		{
			if ( item.CompareTo( largest ) > 0 )
			{
				largest = item;
			}
		}
		return largest;
	}

	public static T LargestBy<T, TKey>( this IEnumerable<T> collection, Func<T, TKey> keySelector ) where TKey : IComparable<TKey>
	{
		if ( collection == default || !collection.Any() ) return default;

		T largestItem = collection.First();
		TKey largestValue = keySelector( largestItem );
		foreach ( T item in collection )
		{
			TKey value = keySelector( item );
			if ( value.CompareTo( largestValue ) > 0 )
			{
				largestValue = value;
				largestItem = item;
			}
		}
		return largestItem;
	}

	public static T Smallest<T>( this IEnumerable<T> collection ) where T : IComparable<T>
	{
		if ( collection == default || !collection.Any() ) return default;

		T largest = collection.First();
		foreach ( T item in collection )
		{
			if ( item.CompareTo( largest ) < 0 )
			{
				largest = item;
			}
		}
		return largest;
	}

	public static T SmallestBy<T, TKey>( this IEnumerable<T> collection, Func<T, TKey> keySelector ) where TKey : IComparable<TKey>
	{
		if ( collection == default || !collection.Any() ) return default;

		T largestItem = collection.First();
		TKey largestValue = keySelector( largestItem );
		foreach ( T item in collection )
		{
			TKey value = keySelector( item );
			if ( value.CompareTo( largestValue ) < 0 )
			{
				largestValue = value;
				largestItem = item;
			}
		}
		return largestItem;
	}

	public static TValue GetValueOrDefault<TKey, TValue>( this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default )
	{
		return dictionary.TryGetValue( key, out var value ) ? value : defaultValue;
	}
}
internal static class GameObjectExtensions
{
	public static void ApplyPrefab(this GameObject obj, string prefab)
	{
		obj.SetPrefabSource(prefab);
		obj.UpdateFromPrefab();
		obj.BreakFromPrefab();
	}

	public static void ApplyPrefab( this GameObject obj, PrefabFile prefab ) => ApplyPrefab( obj, prefab?.ResourcePath );
	public static void TakeDamage( this GameObject obj, DamageInfo info )
	{
		if ( obj == null ) return;

		foreach ( var damagable in obj.GetComponentsInChildren<Component.IDamageable>() )
		{
			damagable.OnDamage( info );
		}
	}

	public static void TakeDamage( this Component comp, DamageInfo info ) => TakeDamage( comp?.GameObject, info );
}

internal static class SceneExtensions
{
	public static GameObject[] GetObjectsInRadius( this Scene scene, Vector3 origin, float radius )
	{
		var objects = scene.GetAllObjects( true );
		return objects?.Where( obj => obj.WorldPosition.Distance( origin ) <= radius ).ToArray();
	}
	public static IEnumerable<GameObject> FindInPhysics( this Scene scene, Vector3 position, float radius ) => scene?.FindInPhysics( new Sphere( position, radius ) ) ?? Enumerable.Empty<GameObject>();
	public static IEnumerable<GameObject> FindInPhysics( this Scene scene, Vector3 position ) => FindInPhysics( scene, position, 1f );
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
		if(time == float.MaxValue)
		{
			return "DNF";
		}

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
