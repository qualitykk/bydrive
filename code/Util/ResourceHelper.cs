using Sandbox.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public static class ResourceHelper
{
	public static GameObject CreateObjectFromResource<T>(T instance) where T: GameResource, IPrefabProvider
	{
		Assert.NotNull( instance, "Tried to instanciate prefab from null resource!" );

		GameObject obj = new();
		obj.ApplyPrefab( instance.Prefab.ResourcePath );
		obj.Name = instance.ResourceName;

		PopulateReferences( obj, instance );

		return obj;
	}

	private static void PopulateReferences<T>(GameObject obj, T resource) where T : GameResource, IPrefabProvider
	{
		var components = obj.Components.GetAll();
		foreach(Component component in components)
		{
			TypeDescription type = TypeLibrary.GetType( component.GetType() );
			foreach( PropertyDescription prop in type.Properties )
			{
				var attrib = prop.GetCustomAttribute<AutoReferenceAttribute>();
				if(attrib == null)
				{
					continue;
				}

				if(attrib.UnsetOnly && prop.GetValue(component) != default)
				{
					continue;
				}

				if(prop.CanWrite && prop.PropertyType == typeof(T))
				{
					prop.SetValue( component, resource );
				}
			}
		}
	}
}
