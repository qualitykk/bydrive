global using System;
global using Sandbox;
global using System.Collections.Generic;
global using System.Linq;
global using static Bydrive.Globals;
using Sandbox.UI;

namespace Bydrive;
internal static class Globals
{
	public const string VERSION = "CONTEST 1.0.2";
	public static class Soundtrack
	{
		public const string RACE_WIN = "sounds/music/menu_race_win.sound";
	}
	public static void ResetGlobals()
	{
		lastLocalInput = null;
		localSettings = null;
		ActiveMenu = null;
	}
	private static VehiclePlayerInput lastLocalInput;
	private static UserSettings localSettings;
	public static Panel ActiveMenu { get; internal set; }
	public static RaceManager Race => RaceManager.Current;
	public static RaceInformation RaceContext => RaceInformation.Current;
	public static TrackMusicParameters RaceMusic => RaceContext?.CurrentMusic ?? new();
	public static SaveFile CurrentSave => Story.Progress;
	public static UserSettings Settings 
	{
		get
		{
			if ( localSettings == null )
				localSettings = UserSettings.Load();

			return localSettings;
		}	
	}
	public static VehiclePlayerInput GetLocalInput()
	{
		if ( lastLocalInput.IsValid() )
		{
			return lastLocalInput;
		}

		Scene scene = Game.ActiveScene;
		VehiclePlayerInput input = scene?.GetAllComponents<VehiclePlayerInput>().FirstOrDefault( input => input.IsLocalInput() );

		lastLocalInput = input;

		return input;
	}
	public static VehicleController GetLocalVehicle()
	{
		return GetLocalInput()?.VehicleController;
	}
	public static RaceParticipant GetLocalParticipantInstance()
	{
		return GetLocalInput()?.ParticipantInstance;
	}
	public static string GetLocalName()
	{
		return new Friend( Game.SteamId ).Name;
	}
}

internal static class InputActions
{
	// Race
	public const string BOOST = "Boost";
	public const string ITEM = "Item";
	public const string BREAK = "Break";
	public const string RESPAWN = "Respawn";

	public const string PITCH_UP = "Backward";
	public const string PITCH_DOWN = "Forward";

	// Story
	public const string USE = "Item";
	public const string DIALOG_SKIP = "Break";
	public const string DIALOG_UP = "Forward";
	public const string DIALOG_DOWN = "Backward";
}

internal static class TraceTags
{
	public const string SOLID = "Solid";
	public const string WORLD = "world";
	public const string VEHICLE = "vehicle";
}
internal static class UI
{
	internal static class Colors
	{
		internal static class Notification
		{
			public static readonly Color Success = new Color( 0x6080DF20 );
			public static readonly Color Critical = new Color( 0x601B48FF );
			public static readonly Color Bonus = new Color( 0x60EE5F13 );
			public static readonly Color Danger = new Color( 0.6f, 0.1f, 0.1f, 0.5f );
		}

		internal static class Popup
		{
			public static readonly Color Info = (Color)Color.Parse( "#5589d9" );
			public static readonly Color Positive = (Color)Color.Parse( "#5bd955" );
			public static readonly Color Negative = (Color)Color.Parse( "#d96b55" );
			public static readonly Color Major = (Color)Color.Parse( "#e6932e" );
		}

		public static Color GetStatColor(float fraction)
		{
			Color badColor = (Color)Color.Parse("#ff7733");
			Color goodColor = (Color)Color.Parse( "#77ff33" );
			return Color.Lerp(badColor, goodColor, fraction);
		}
	}
	public static void MakeMenu(Panel panel)
	{
		if ( panel == ActiveMenu ) return;

		ActiveMenu?.SetClass( "menu", false );
		ActiveMenu = panel;
		panel?.SetClass( "menu", true );
	}
	public static void MakeMenuInactive(Panel panel = null)
	{
		if ( panel != null && ActiveMenu != panel ) return;

		ActiveMenu?.SetClass( "menu", false );
		ActiveMenu = null;
	}

	public static string TagIf(string tag, bool active, string inactiveTag = "")
	{
		if ( active )
			return tag;

		return inactiveTag;
	}
	public static string ActiveIf( bool active ) => TagIf( "active", active, "disabled " );
	public static string ActiveIf( Func<bool> check ) => ActiveIf( check?.Invoke() ?? false );
	public static string ActiveIf( bool? active ) => ActiveIf( active == true );
	public static string ActiveIfMenu( Panel panel ) => ActiveIf( ActiveMenu == panel );
	public static IEnumerable<PanelComponent> GetRaceHudElements()
	{
		return Game.ActiveScene.Components.GetAll<PanelComponent>( FindMode.EverythingInSelfAndDescendants).Where( p => p is IRaceHudPanel );
	}
	public static void ShowRaceHUD()
	{
		foreach(var panel in GetRaceHudElements() )
		{
			panel.Enabled = true;
		}
	}
	public static void HideRaceHud()
	{
		foreach ( var panel in GetRaceHudElements() )
		{
			panel.Enabled = false;
		}
	}
	public static string FormatRoute(string value)
	{
		string[] parts = value.Split( '_' );
		if ( parts.Length > 1 )
		{
			return $"{parts[0].ToTitleCase()} ({string.Join( ' ', parts.Skip( 1 ).Select( p => p.ToTitleCase() ) )})";
		}

		return value.ToTitleCase();
	}
}

internal static class VehicleStatModifiers
{
	public const string SPEED = "Speed";
	public const string ACCELERATION = "Acceleration";
}
