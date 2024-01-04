global using System;
global using Sandbox;
global using static Bydrive.Globals;
using System.Linq;

namespace Bydrive;
internal static class Globals
{
	private static Scene lastScene;
	private static VehicleController lastLocalVehicle;
	public static RaceManager Race => RaceManager.Current;
	public static RaceInformation RaceContext => RaceInformation.Current;
	public static VehicleController GetLocalVehicle()
	{
		Scene currentScene = GameManager.ActiveScene;
		if(currentScene == lastScene && lastLocalVehicle != null)
		{
			return lastLocalVehicle;
		}
		VehicleController vehicle = currentScene.GetAllComponents<VehicleController>().FirstOrDefault( p => IsLocallyOwned(p) && p.PlayerControlled );

		lastScene = currentScene;
		lastLocalVehicle = vehicle;

		return vehicle;
	}

	private static bool IsLocallyOwned(VehicleController controller)
	{
		return !controller.Network.Active || controller.Network.IsOwner;
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

	public const string PITCH_DOWN = "Crouch";
}

internal static class TraceTags
{
	public const string SOLID = "Solid";
}
