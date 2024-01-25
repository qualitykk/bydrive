using Sandbox;
using Sandbox.Network;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class StartMenu
{
	const string MENU_SCENE = "/scenes/startmenu.scene";
	const string RACE_LOBBY_URL = "/multiplayer/race/active";
	public static StartMenu Current { get; set; }
	public static void Open()
	{
		if(RaceContext?.Definition.UseScene() == true)
		{
			var scene = ResourceLibrary.Get<SceneFile>( MENU_SCENE );
			GameManager.ActiveScene.Load( scene );

			return;
		}

		if ( Current == null )
			return;
		Current.Enabled = true;
	}
	public static void Close()
	{
		if ( Current == null )
			return;

		if(RaceContext == null)
		{
			Game.Close();
		}

		Current.Enabled = false;
	}
	private static VehicleDefinition _localSelectedVehicle;
	public static VehicleDefinition GetDefaultVehicle()
	{
		return ResourceLibrary.GetAll<VehicleDefinition>().FirstOrDefault();
	}
	public static VehicleDefinition SelectedVehicle 
	{ 
		get 
		{
			VehicleDefinition selection = LobbyManager.MultiplayerActive ? LobbyManager.Instance.LocalPlayer.SelectedVehicle : _localSelectedVehicle;
			if( selection == null)
			{
				return GetDefaultVehicle();
			}

			return selection;
		}
		set
		{
			if(LobbyManager.MultiplayerActive)
			{
				LobbyManager.Instance.LocalPlayer.SelectedVehicle = value;
			}
			_localSelectedVehicle = value;
		}
	}
	[Property] public SoundEvent BackgroundMusic { get; set; }
	[Property] public float BackgroundMusicVolume { get; set; } = -1f;
	public NavHostPanel NavPanel { get; set; }
	private string[] GetLobbyUrls() => new string[] { "/vehicle", RACE_LOBBY_URL };
	protected override void OnUpdate()
	{
		if(GameNetworkSystem.IsConnecting || GameNetworkSystem.IsActive)
		{
			if(!GetLobbyUrls().Any( NavPanel.CurrentUrl.Contains ) )
			{
				NavPanel.Navigate( RACE_LOBBY_URL );
			}
		}
	}
	protected override void OnEnabled()
	{
		base.OnEnabled();
		Current = this;
		PlayMusic();
		NavPanel?.Navigate( "/front" );
		SelectedVehicle = default;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Music.Stop();
	}

	private void PlayMusic( )
	{
		Music.Play( BackgroundMusic, BackgroundMusicVolume );
	}
}
