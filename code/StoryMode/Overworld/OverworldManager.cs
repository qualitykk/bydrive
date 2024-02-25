using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class OverworldManager : Component
{
	[Property] public PrefabFile PlayerPrefab { get; set; }
	[Property] public MapInstance Map { get; set; }
	private GameObject playerObject;
	private bool initialised = false;
	protected override void OnUpdate()
	{
		if(Map == null)
		{
			return;
		}

		if(Map.IsLoaded && !initialised)
		{
			Initialise();
		}
	}
	private void Initialise()
	{
		if ( PlayerPrefab == null )
		{
			Log.Error( "Overworld without player???" );
			return;
		}

		playerObject?.DestroyImmediate();

		playerObject = new();
		playerObject.ApplyPrefab( PlayerPrefab );

		// TODO: Get story based spawn pos
		GameObject spawn = Scene.GetAllComponents<SpawnPoint>().FirstOrDefault().GameObject;
		playerObject.Transform.World = spawn.Transform.World;
		initialised = true;

		foreach (var info in GetUsables() )
		{
			GameObject obj = info.Item1;
			IInteractible usable = info.Item2;

			var collider = obj.Components.Create<BoxCollider>();
			collider.Center = usable.Bounds.Center;
			collider.Scale = usable.Bounds.Size;
			collider.IsTrigger = true;
		}
	}

	protected override void OnDestroy()
	{
		ResetGlobals();
	}

	private IEnumerable<Tuple<GameObject, IInteractible>> GetUsables()
	{
		var objects = Scene.GetAllObjects( true );
		
		return objects.Select( obj => obj.Components.TryGet<IInteractible>( out var component ) ? new Tuple<GameObject, IInteractible>(obj, component ) : null ).Where(entry => entry != null);
	}
}
