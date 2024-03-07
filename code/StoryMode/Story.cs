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
	public delegate bool UnlockCheck( SaveFile save );
	public delegate void CompletionProgress( SaveFile file );
	public static bool Active { get; private set; }
	public static SaveFile Progress { get; set; }
	public static string GetPlayerName()
	{
		return Progress?.CharacterName ?? GetLocalName() ?? "Max Mustermann";
	}
	public static string GetTeamName()
	{
		return "Team Muster";
	}
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

	[ConCmd("st_file_load")]
	private static void Command_LoadSave(string id)
	{
		const string SAVE_EXTENSION = ".save";
		if ( !id.EndsWith( SAVE_EXTENSION ) )
			id += SAVE_EXTENSION;

		var file = SaveFile.Load( id );
		if(file == null)
		{
			Log.Warning( $"No save file with id {id}" );
			return;
		}

		Load( file );
	}

	[ConCmd("st_file_save")]
	private static void Command_SaveCurrent()
	{
		if(!Active)
		{
			Log.Warning( "No save file loaded!" );
			return;
		}

		Save();
	}

	[ConCmd("st_file_dump")]
	private static void Command_DumpSaves()
	{
		var saves = SaveFile.GetAll();
		Log.Info( $"{saves?.Count()} saves:" );
		if(saves.Any())
		{
			foreach(var save in saves)
			{
				Log.Info( $"= {save}" );
			}
		}
	}
}
