global using System;
global using Sandbox;
global using static Bydrive.Globals;
using System.Linq;

namespace Bydrive;
internal static class Globals
{
	private static Scene lastScene;
	private static VehiclePlayerInput lastLocalInput;
	public static RaceManager Race => RaceManager.Current;
	public static RaceInformation RaceContext => RaceInformation.Current;
	public static UserSettings Settings => new();
	public static VehiclePlayerInput GetLocalInput()
	{
		Scene currentScene = GameManager.ActiveScene;
		if ( currentScene == lastScene && lastLocalInput != null )
		{
			return lastLocalInput;
		}
		VehiclePlayerInput input = currentScene.GetAllComponents<VehiclePlayerInput>().FirstOrDefault( input => input.IsLocalInput() );

		lastScene = currentScene;
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
}

internal static class InputActions
{
	public const string FORWARD = "Forward";
	public const string BACK = "Backward";
	public const string LEFT = "Left";
	public const string RIGHT = "Right";

	public const string BOOST = "Run";
	public const string BREAK = "Jump";
	public const string RESPAWN = "Reload";

	public const string PITCH_UP = "Run";
	public const string PITCH_DOWN = "Crouch";
}

internal static class TraceTags
{
	public const string SOLID = "Solid";
}
