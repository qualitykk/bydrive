﻿using System;
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
	}
}