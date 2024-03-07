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
	public static void Challenge(ChallengeDefinition challenge, VehicleBuilder vehicle)
	{
		Assert.NotNull(challenge);
		Assert.NotNull( vehicle );

		List<RaceInformation.Participant> racers = new();
		int currentPosition = RaceStartingPosition.FIRST_PLACE;

		foreach(var p in challenge.Participants.OrderBy(p => p.StartPosition))
		{
			racers.Add( new( VehicleBuilder.ForDefinition(p.Vehicle), Player.CreateBot(p.Name), currentPosition ) );
			currentPosition++;
		}
		racers.Add( new( vehicle, Player.Local, currentPosition ) );
		var race = new RaceInformation( challenge.Races, racers );
		race.OnAllRacesFinished += ( RaceInformation info, List<RaceInformation.Participant> participants ) => ChallengeRaceAllFinished(challenge, info, participants);
		race.Start();
	}
	private static void ChallengeRaceAllFinished(ChallengeDefinition challenge, RaceInformation info, List<RaceInformation.Participant> participants )
	{
		Player firstPlace = participants.FirstOrDefault()?.Player;
		bool win = firstPlace.IsBot == false;
		if(!Story.Active)
		{
			Log.Warning( "Completed challenge with no save file? Returning to main menu..." );
			StartMenu.Open();
			return;
		}

		Story.EnterOverworld();
		if(win)
		{
			Log.Info( "Win!" );
			if(CurrentSave.GetChallengeState(challenge) != ChallengeState.Complete)
			{
				CurrentSave.SetChallengeState( challenge, ChallengeState.Complete );
				challenge.OnComplete?.Invoke( CurrentSave );

				CurrentSave.AutoSave();
			}
			
		}
		else
		{
			Log.Info( "Lose..." );
		}
	}
	public static void TimeTrial(TrackDefinition track, Dictionary<string, string> variables, VehicleDefinition vehicle)
	{
		List<RaceInformation.Participant> racers = new()
		{
			new( VehicleBuilder.ForDefinition(vehicle), Player.Local, RaceStartingPosition.FIRST_PLACE )
		};

		RaceParameters timeTrialParameters = new()
		{
			MaxLaps = track.Parameters.MaxLaps,
			Mode = RaceMode.TimeTrial
		};
			
		TrackMusicParameters timeTrialMusic = new()
		{
			RaceMusic = ResourceLibrary.Get<SoundEvent>( TIME_TRIAL_MUSIC_TRACK ),
			RaceMusicVolume = TIME_TRIAL_MUSIC_VOLUME,
			RaceStartWait = TIME_TRIAL_WAIT
		};

		var race = new RaceInformation( new RaceSetup( track, timeTrialParameters, variables, timeTrialMusic ), racers );
		race.Start();
	}
	public static void LocalWithBots(TrackDefinition race, int amount, VehicleDefinition playerVehicle, int playerStartPos)
	{
		List<RaceInformation.Participant> racers = new() 
		{ 
			new( VehicleBuilder.ForDefinition(playerVehicle), Player.Local, playerStartPos ) 
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
			racers.Add( new(VehicleBuilder.ForDefinition(GetBotVehicle()), botPlayer, i) );
		}

		new RaceInformation( race, racers ).Start();
	}

	private static VehicleDefinition GetBotVehicle()
	{
		VehicleDefinition devCar = VehicleDefinition.GetDefault();
		return devCar;
	}

	private static RaceInformation.Participant CreateBot(VehicleDefinition def, int startPosition)
	{
		return new( VehicleBuilder.ForDefinition(def), Player.CreateBot(), startPosition );
	}
}
