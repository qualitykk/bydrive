using Sandbox.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

internal static class StartRace
{
	const float TIME_TRIAL_WAIT = 3.5f;
	const string TIME_TRIAL_MUSIC_TRACK = "/sounds/music/race_timetrial.sound";
	const float TIME_TRIAL_MUSIC_VOLUME = 0.8f;
	public static void Challenge(ChallengeDefinition challenge, VehicleDefinition vehicle)
	{
		Assert.NotNull(challenge);
		Assert.NotNull( vehicle );

		List<RaceMatchInformation.Participant> racers = new();
		int currentPosition = RaceStartingPosition.FIRST_PLACE;

		foreach(var p in challenge.Participants.OrderBy(p => p.StartPosition))
		{
			racers.Add( new( p.Vehicle, Player.CreateBot(p.Name), currentPosition ) );
			currentPosition++;
		}
		racers.Add( new( vehicle, Player.Local, currentPosition ) );
		new RaceMatchInformation( challenge.Track, racers, challenge.Parameters );
	}
	public static void TimeTrial(RaceDefinition race, VehicleDefinition vehicle)
	{
		List<RaceMatchInformation.Participant> racers = new()
		{
			new( vehicle, Player.Local, RaceStartingPosition.FIRST_PLACE )
		};

		RaceParameters timeTrialParameters = new()
		{
			MaxLaps = race.Parameters.MaxLaps,
			Mode = RaceMode.TimeTrial,
			RaceMusic = ResourceLibrary.Get<SoundEvent>( TIME_TRIAL_MUSIC_TRACK ),
			RaceMusicVolume = TIME_TRIAL_MUSIC_VOLUME,
			RaceStartWait = TIME_TRIAL_WAIT
		};

		new RaceMatchInformation( race, racers, timeTrialParameters);
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

			Player botPlayer = Player.CreateBot();
			botPlayer.DisplayName = $"Bot {i}";
			racers.Add( new(GetBotVehicle(), botPlayer, i) );
		}

		new RaceMatchInformation( race, racers );
	}

	private static VehicleDefinition GetBotVehicle()
	{
		VehicleDefinition devCar = VehicleDefinition.GetDefault();
		return devCar;
	}

	private static RaceMatchInformation.Participant CreateBot(VehicleDefinition def, int startPosition)
	{
		return new( def, Player.CreateBot(), startPosition );
	}
}
