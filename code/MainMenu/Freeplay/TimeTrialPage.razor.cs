using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class TimeTrialPage : Panel, INavigatorPage
{
	int stage = 0;
	TrackDefinition selectedTrack;
	Dictionary<string, string> trackVariableValues = new();
	TimeTrialData selectedTimeTrial;
	VehicleDefinition selectedVehicle;
	private IEnumerable<TimeTrialData> GetTimeTrials()
	{
		return TimeTrialData.ReadForTrack( selectedTrack.ResourcePath, trackVariableValues );
	}
	private bool AllVariablesSelected()
	{
		foreach(var variable in selectedTrack.Variables)
		{
			if(!trackVariableValues.ContainsKey(variable.Key))
			{
				return false;
			}
		}

		return true;
	}
	private void OnTrackSelected( TrackDefinition def )
	{
		selectedTrack = def;
		stage = 1;
	}
	private void OnTrackVariableSelected(string key, string value)
	{
		trackVariableValues[key] = value;
	}

	private void OnTimeTrialEntrySelect(TimeTrialData entry)
	{
		selectedTimeTrial = entry;
	}

	private void OnVehicleSelect(VehicleDefinition vehicle)
	{
		selectedVehicle = vehicle;
	}

	private string GetVariableClasses( string key, string value ) 
	{
		string classes = "";
		if ( trackVariableValues.ContainsKey( key ) && trackVariableValues[key] == value )
			classes += " selected";

		return classes;
	}
	private string GetTimeTrialEntryClasses(TimeTrialData entry)
	{
		string classes = "";
		if ( selectedTimeTrial == entry )
			classes += " selected";

		return classes;
	}
	private string GetPlacementClass(int placement)
	{
		if ( placement == 1 )
			return "first";
		else if ( placement == 2 )
			return "second";
		else if ( placement == 3 )
			return "third";
		return "";
	}

	private void OnClickStart()
	{
		StartRace.TimeTrial( selectedTrack, trackVariableValues, selectedVehicle );
	}
	private void OnClickSelectVehicle()
	{
		stage = 2;
	}
	private void OnClickBack()
	{
		if(stage == 0)
		{
			this.Navigate( "/front" );
			return;
		}

		stage--;
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( selectedTrack, selectedTimeTrial, trackVariableValues );
	}

	void INavigatorPage.OnNavigationClose()
	{
		selectedTrack = null;
		trackVariableValues.Clear();
		selectedTimeTrial = null;
		selectedVehicle = null;
	}
}
