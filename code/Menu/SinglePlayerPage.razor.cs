using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class SinglePlayerPage
{
	int botCount { get; set; } = 3;
	RaceDefinition selectedTrack;
	private void OnTrackSelected( RaceDefinition def )
	{
		selectedTrack = def;
	}

	private string GetStartClasses()
	{
		if ( selectedTrack == null )
			return "disabled";
		return "";
	}

	private void OnClickStart()
	{
		if( selectedTrack == null )
		{
			return;
		}

		// TODO: Vehicle Selection
		VehicleDefinition devCar = ResourceLibrary.Get<VehicleDefinition>( "data/devcar1.vehicle" );

		StartRace.WithBots( selectedTrack, botCount, devCar, botCount + 1 );
	}

	private void OnClickBack()
	{
		this.Navigate( "/front" );
	}
}
