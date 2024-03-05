using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class ChallengesPage : Panel
{
	RaceSetupManager manager => RaceSetupManager.Current;

	private enum SetupStep
	{
		SelectRace,
		SelectVehicle,
		ViewInfo
	}

	SetupStep currentStep;
	private void OnClickBack()
	{
		if ( currentStep > SetupStep.SelectRace )
		{
			currentStep--;
		}
		else
		{
			//Story.EnterOverworld();
			this.Navigate( "/" );
		}
	}
	private void OnClickNext()
	{
		if ( currentStep < SetupStep.ViewInfo )
		{
			currentStep++;
		}
		else
		{
			manager.Start();
		}
	}
	private bool CanClickBack()
	{
		return true;
	}
	private bool CanClickNext()
	{
		switch(currentStep)
		{
			case SetupStep.SelectRace:
				return manager.SelectedChallenge != null;
			case SetupStep.SelectVehicle:
				return manager.SelectedVehicle != null;
		}

		return true;
	}
	private string GetHeader()
	{
		switch(currentStep)
		{
			case SetupStep.SelectRace:
				return "Select Challenge";
			case SetupStep.SelectVehicle:
				return "Select Vehicle";
		}
		return "Start Race";
	}
	private string GetBackLabel()
	{
		return "Back";

		/*
		if ( currentStep == SetupStep.SelectRace )
			return "Quit";

		return "Back";
		*/
	}

	private string GetNextLabel()
	{
		if ( currentStep == SetupStep.ViewInfo )
			return "Start";

		return "Next";
	}
}
