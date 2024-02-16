﻿using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class BotRacePage
{
	int botCount { get; set; } = 3;
	RaceDefinition selectedTrack;
	protected override void OnParametersSet()
	{
		selectedTrack = ResourceLibrary.GetAll<RaceDefinition>().First();
	}
	private void OnTrackSelected( RaceDefinition def )
	{
		selectedTrack = def;
	}

	private void OnClickStart()
	{
		if( selectedTrack == null )
		{
			return;
		}

		VehicleDefinition playerCar = StartMenu.SelectedVehicle ?? ResourceLibrary.Get<VehicleDefinition>( "data/devcar1.vehicle" );

		StartRace.LocalWithBots( selectedTrack, botCount, playerCar, botCount + 1 );
	}

	private void OnClickBack()
	{
		this.Navigate( "/front" );
	}
}
