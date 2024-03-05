using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public static class Story
{
	const string OVERWORLD_SCENE = "/scenes/story_overworld.scene";
	const string RACE_SETUP_SCENE = "/scenes/story_race_setup.scene";
	public static bool Active { get; private set; }
	public static SaveFile Progress { get; set; }
	public static void Load(SaveFile save)
	{
		Active = true;
		Progress = save;

		EnterOverworld();
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

	[ActionGraphNode( "story.enter.overworld" )]
	[Title( "Enter Overworld" ), Group( "Story" )]
	public static void EnterOverworld()
	{
		Game.ActiveScene.LoadFromFile( OVERWORLD_SCENE );
	}
	[ActionGraphNode( "story.enter.race_setup" )]
	[Title( "Enter Race Setup" ), Group( "Story" )]
	public static void EnterRaceSetup()
	{
		Game.ActiveScene.LoadFromFile( RACE_SETUP_SCENE );
	}
}
