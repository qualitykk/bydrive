using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class TimeTrialPage
{
	
	TrackDefinition selectedTrack;
	private void OnTrackSelected( TrackDefinition def )
	{
		selectedTrack = def;
	}

	private void OnClickStart()
	{
		/*
		if( selectedTrack == null )
		{
			return;
		}

		VehicleDefinition playerCar = StartMenu.SelectedVehicle;

		StartRace.TimeTrial( selectedTrack, playerCar );
		*/
		Log.Warning( "TODO: Reimplement seperate time trial" );
	}

	private void OnClickBack()
	{
		this.Navigate( "/front" );
	}
}
