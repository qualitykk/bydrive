using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public static class StoryMode
{
	const string OVERWORLD_SCENE = "/scenes/story_overworld.scene";
	public static bool Active { get; private set; }
	public static SaveFile Progress { get; set; }
	public static void Load(SaveFile save)
	{
		Active = true;
		Progress = save;

		LoadOverworld();
	}

	public static void Save()
	{
		if(!Active)
		{
			return;
		}

		Progress.Save();
	}
	
	public static void Exit()
	{
		Active = false;

		StartMenu.Open();
		Progress = null;
	}

	private static void LoadOverworld()
	{
		GameManager.ActiveScene.LoadFromFile( OVERWORLD_SCENE );
	}
}
