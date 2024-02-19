using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class TimeTrialPage
{
	
	RaceDefinition selectedTrack;
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

		VehicleDefinition playerCar = StartMenu.SelectedVehicle;

		StartRace.TimeTrial( selectedTrack, playerCar );
	}

	private void OnClickBack()
	{
		this.Navigate( "/front" );
	}
}
