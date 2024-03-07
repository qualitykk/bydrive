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
	private Transform lastPlayerPosition;
	private bool initialised = false;
	protected override void OnUpdate()
	{
		if(Map == null)
		{
			return;
		}

		lastPlayerPosition = playerObject?.Transform.World ?? default;

		if(Map.IsLoaded && !initialised)
		{
			Initialise();
		}
	}
	private void Initialise()
	{
		ResetGlobals();

		if ( PlayerPrefab == null )
		{
			Log.Error( "Overworld without player???" );
			return;
		}

		playerObject?.DestroyImmediate();

		playerObject = new();
		playerObject.ApplyPrefab( PlayerPrefab );

		// Spawn via last location, then via spawn point, then via this object
		Transform spawnTransform = CurrentSave?.LastTransform ?? Scene.GetAllComponents<SpawnPoint>()?.FirstOrDefault()?.Transform?.World ?? Transform.World;

		// Scale = bad
		playerObject.Transform.Position = spawnTransform.Position;
		playerObject.Transform.Rotation = spawnTransform.Rotation;

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
		if(Story.Active)
		{
			CurrentSave.LastTransform = lastPlayerPosition;
			Story.Save();
		}
	}

	private IEnumerable<Tuple<GameObject, IInteractible>> GetUsables()
	{
		var objects = Scene.GetAllObjects( true );
		
		return objects.Select( obj => obj.Components.TryGet<IInteractible>( out var component ) ? new Tuple<GameObject, IInteractible>(obj, component ) : null ).Where(entry => entry != null);
	}
}
