using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

internal static class StartRace
{
	public static void WithBots(RaceDefinition race, int amount, VehicleDefinition playerVehicle, int playerStartPos)
	{
		Player localPlayer = new( null )
		{
			DisplayName = GetLocalName()
		};

		List<RaceInformation.Participant> racers = new() 
		{ 
			new( playerVehicle, localPlayer, playerStartPos ) 
		};
		int racerAmount = amount + 1;

		for ( int i = 1; i < racerAmount + 1; i++ )
		{
			if(i == playerStartPos)
			{
				continue;
			}

			Player botPlayer = Player.CreateBot();
			botPlayer.DisplayName = $"Bot {i}";
			racers.Add( new(GetBotVehicle(), botPlayer, i) );
		}

		new RaceInformation( race, racers );
	}

	private static VehicleDefinition GetBotVehicle()
	{
		VehicleDefinition devCar = ResourceLibrary.Get<VehicleDefinition>( "data/devcar1.vehicle" );
		return devCar;
	}

	private static RaceInformation.Participant CreateBot(VehicleDefinition def, int startPosition)
	{
		return new( def, Player.CreateBot(), startPosition );
	}
}
