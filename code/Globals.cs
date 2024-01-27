global using System;
global using Sandbox;
global using System.Collections.Generic;
global using System.Linq;
global using static Bydrive.Globals;

namespace Bydrive;
internal static class Globals
{
	public static class UI
	{
		public static string ActiveIf( Func<bool> check ) => ActiveIf( check?.Invoke() ?? false );

		public static string ActiveIf( bool active )
		{
			if ( active )
				return "active ";

			return "disabled ";
		}

		public static string ActiveIf( bool? active ) => ActiveIf( active == true );
	}
	public static void ResetGlobals()
	{
		lastLocalInput = null;
		localSettings = null;
	}
	private static VehiclePlayerInput lastLocalInput;
	private static UserSettings localSettings;
	public static RaceManager Race => RaceManager.Current;
	public static RaceMatchInformation RaceContext => RaceMatchInformation.Current;
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

		Scene scene = GameManager.ActiveScene;
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
	public const string FORWARD = "Forward";
	public const string BACK = "Backward";
	public const string LEFT = "Left";
	public const string RIGHT = "Right";

	public const string BOOST = "Boost";
	public const string ITEM = "Item";
	public const string BREAK = "Break";
	public const string RESPAWN = "Respawn";

	public const string PITCH_UP = "Backward";
	public const string PITCH_DOWN = "Forward";
}

internal static class TraceTags
{
	public const string SOLID = "Solid";
	public const string WORLD = "world";
	public const string VEHICLE = "vehicle";
}

internal static class UIColors
{
	internal static class Notification
	{
		public static Color Success => new Color( 0x6080DF20 );
		public static Color Critical => new Color( 0x601B48FF );
		public static Color Bonus => new Color( 0x60EE5F13 );
		public static Color Danger => new Color( 0.6f, 0.1f, 0.1f, 0.5f);
	}
}

internal static class VehicleStatModifiers
{
	public const string SPEED = "Speed";
	public const string ACCELERATION = "Acceleration";
}
