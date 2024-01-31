using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

internal static class StartRace
{

	public static void TimeTrial(RaceDefinition race, VehicleDefinition vehicle)
	{
		List<RaceMatchInformation.Participant> racers = new()
		{
			new( vehicle, Player.Local, RaceStartingPosition.FIRST_PLACE )
		};

		new RaceMatchInformation( race, racers, mode: RaceMode.TimeTrial );
	}
	public static void Online(RaceDefinition race, List<RaceMatchInformation.Participant> racers)
	{
		new RaceMatchInformation( race, racers );
	}
	public static void LocalWithBots(RaceDefinition race, int amount, VehicleDefinition playerVehicle, int playerStartPos)
	{
		List<RaceMatchInformation.Participant> racers = new() 
		{ 
			new( playerVehicle, Player.Local, playerStartPos ) 
		};
		int racerAmount = amount + 1;

		for ( int i = 1; i < racerAmount + 1; i++ )
		{
			if(i == playerStartPos)
			{
				continue;
			}

			racers.Add( CreateBot(GetBotVehicle(), i) );
		}

		new RaceMatchInformation( race, racers );
	}

	private static VehicleDefinition GetBotVehicle()
	{
		VehicleDefinition devCar = ResourceLibrary.Get<VehicleDefinition>( "data/devcar1.vehicle" );
		return devCar;
	}

	private static RaceMatchInformation.Participant CreateBot(VehicleDefinition def, int startPosition)
	{
		RaceMatchInformation.Participant bot = new( def, Player.CreateBot(), startPosition );
		bot.Player.DisplayName = $"Bot {startPosition}";

		return bot;
	}
}
