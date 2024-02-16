using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class OverworldManager : Component
{
	[Property] public PrefabFile PlayerPrefab { get; set; }
	protected override void OnAwake()
	{
		if(PlayerPrefab == null)
		{
			Log.Error( "Overworld without player???" );
			return;
		}

		GameObject player = new();
		player.ApplyPrefab( PlayerPrefab );

		// TODO: Get story based spawn pos
		GameObject spawn = Scene.GetAllComponents<SpawnPoint>().FirstOrDefault().GameObject;
		player.Transform.World = spawn.Transform.World;
	}
}
