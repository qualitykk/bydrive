using Sandbox.UI;
using System.Collections.Generic;

namespace Bydrive;

public partial class MainPage
{
	void OnClickCareerNew()
	{
		this.Navigate( "/career/new" );

	}
	void OnClickCareerLoad()
	{
		this.Navigate( "/career/load" );
	}
	void OnClickTimeTrial()
	{
		this.Navigate( "/singleplayer/timetrial" );
	}
	void OnClickRaceQuickplay()
	{
		TrackDefinition randomTrack = Game.Random.FromArray(TrackDefinition.GetAllVisible());

		StartRace.LocalWithBots( randomTrack, 3, StartMenu.SelectedVehicle, 4 );
	}
	void OnClickRaceOnline()
	{
		this.Navigate( "/multiplayer/race/browser" );
	}
	void OnClickRaceBots()
	{
		this.Navigate( "/multiplayer/race/bots" );
	}
	void OnClickControls()
	{
		this.Navigate( "/controls" );
	}
	void OnClickSettings()
	{
		this.Navigate( "/settings" );
	}

	void OnClickCredits()
	{
		this.Navigate( "/credits" );
	}
}
