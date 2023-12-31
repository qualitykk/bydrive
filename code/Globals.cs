global using System;
global using Sandbox;
global using static Redrome.Globals;

namespace Redrome;
internal static class Globals
{
	public static RaceManager Race => RaceManager.Current;
	public static RaceInformation RaceContext => RaceInformation.Current;
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
