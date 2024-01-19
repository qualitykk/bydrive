using Sandbox.UI;
using System.Collections.Generic;

namespace Bydrive;

public partial class MainPage
{
	void OnClickTimeTrial()
	{
		this.Navigate( "/singleplayer/timetrial" );
	}
	void OnClickRaceQuickplay()
	{
		// Load race testing env
		RaceDefinition testRace = ResourceLibrary.Get<RaceDefinition>( "data/devtest1.race" );
		VehicleDefinition devCar = ResourceLibrary.Get<VehicleDefinition>( "data/devcar1.vehicle" );

		StartRace.WithBots( testRace, 3, devCar, 4 );
	}

	void OnClickRaceOnline()
	{
		this.Navigate( "/multiplayer/browser" );
	}
	void OnClickRaceBots()
	{
		this.Navigate( "/multiplayer/race/bots" );
	}

}
