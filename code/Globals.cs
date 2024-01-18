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

		public static string ActiveIf(bool active)
		{
			if ( active )
				return "active ";

			return "disabled ";
		}

		public static string ActiveIf( bool? active ) => ActiveIf( active == true );
	}
	private static VehiclePlayerInput lastLocalInput;
	public static RaceManager Race => RaceManager.Current;
	public static RaceInformation RaceContext => RaceInformation.Current;
	public static UserSettings Settings => new();
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

	public const string BOOST = "Run";
	public const string ITEM = "Use";
	public const string BREAK = "Jump";
	public const string RESPAWN = "Reload";

	public const string PITCH_UP = "Run";
	public const string PITCH_DOWN = "Crouch";
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
	}
}
