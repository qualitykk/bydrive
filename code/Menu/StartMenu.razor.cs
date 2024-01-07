using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class StartMenu
{
	[Property] public SoundEvent BackgroundMusic { get; set; }

	void OnClickSingleplayer()
	{
		OnClickDevStart();
	}

	void OnClickMultiplayer()
	{

	}

	void OnClickDevStart()
	{
		// Load race testing env
		RaceDefinition testDefinition = ResourceLibrary.Get<RaceDefinition>( "data/devtest1.race" );
		VehicleDefinition devCar = ResourceLibrary.Get<VehicleDefinition>( "data/devcar1.vehicle" );

		List<RaceInformation.Participant> participants = new()
		{
			new(devCar, Player.CreateBot(), 1),
			new(devCar, Player.CreateBot(), 2),
			new(devCar, Player.CreateBot(), 3),
			new(devCar, new Player(null), 4)
		};

		Music.Stop();
		RaceInformation info = new( testDefinition, participants, true );
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
		IntroPlayer.OnIntroComplete += PlayMusic;
	}

	private void PlayMusic( IntroPlayer player )
	{
		Music.Play( BackgroundMusic );
	}
}
