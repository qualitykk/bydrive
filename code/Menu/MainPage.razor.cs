using Sandbox.UI;
using System.Collections.Generic;

namespace Bydrive;

public partial class MainPage
{
	void OnClickSingleplayer()
	{
		this.Navigate( "/bots" );
	}

	void OnClickMultiplayer()
	{

	}

	void OnClickDevStart()
	{
		// Load race testing env
		RaceDefinition testRace = ResourceLibrary.Get<RaceDefinition>( "data/devtest1.race" );
		VehicleDefinition devCar = ResourceLibrary.Get<VehicleDefinition>( "data/devcar1.vehicle" );

		StartRace.WithBots( testRace, 3, devCar, 4 );
	}
}
