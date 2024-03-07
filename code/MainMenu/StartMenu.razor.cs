using Sandbox;
using Sandbox.Network;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class StartMenu : PanelComponent
{
	const string MENU_SCENE = "/scenes/startmenu.scene";
	const string RACE_LOBBY_URL = "/multiplayer/race/active";
	private float GetHorizontalScroll()
	{
		const float HORIZONTALL_SCROLL_TIME = 25f;
		float current = (RealTime.Now % HORIZONTALL_SCROLL_TIME) / HORIZONTALL_SCROLL_TIME;
		return current * 100;
	}
	private float GetVerticalScroll()
	{
		const float VERTICAL_SCROLL_TIME = 60f;
		float current = (RealTime.Now % VERTICAL_SCROLL_TIME) / VERTICAL_SCROLL_TIME;
		return current * 100;
	}
	public static StartMenu Current { get; set; }
	public static void Open()
	{
		var scene = ResourceLibrary.Get<SceneFile>( MENU_SCENE );
		Game.ActiveScene.Load( scene );

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

	public static void Back()
	{
		Current?.NavPanel?.GoBack();
	}
	private static VehicleDefinition _localSelectedVehicle;
	public static VehicleDefinition SelectedVehicle 
	{ 
		get 
		{
			VehicleDefinition selection = LobbyManager.MultiplayerActive ? LobbyManager.Instance.LocalPlayer.SelectedVehicle : _localSelectedVehicle;
			if( selection == null)
			{
				return VehicleDefinition.GetDefault();
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
		ResetGlobals();
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

	protected override int BuildHash()
	{
		return HashCode.Combine( RealTime.Now );
	}
}
