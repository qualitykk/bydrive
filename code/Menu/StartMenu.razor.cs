using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class StartMenu
{
	[Property] public SoundEvent BackgroundMusic { get; set; }
	public static void Open()
	{
		GameManager.ActiveScene.LoadFromFile( "scenes/startmenu.scene" );
	}
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

		Player localPlayer = new( null )
		{
			DisplayName = GetLocalName()
		};

		List<RaceInformation.Participant> participants = new()
		{
			new(devCar, Player.CreateBot(), 1),
			new(devCar, Player.CreateBot(), 2),
			new(devCar, Player.CreateBot(), 3),
			new(devCar, localPlayer, 4)
		};

		RaceInformation info = new( testDefinition, participants, true );
	}

	protected override void OnAwake()
	{
		base.OnAwake();
		PlayMusic();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Music.Stop();
	}

	private void PlayMusic( )
	{
		Music.Play( BackgroundMusic );
	}
}
