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
		RaceDefinition randomTrack = Game.Random.FromArray(RaceDefinition.GetAllVisible());

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
}
