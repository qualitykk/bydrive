using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class StartMenu
{
	[Property] public SoundEvent BackgroundMusic { get; set; }
	[Property] public float BackgroundMusicVolume { get; set; } = -1f;
	public static VehicleDefinition SelectedVehicle { get; set; }
	public static StartMenu Current { get; set; }
	public static void Open()
	{
		GameManager.ActiveScene.LoadFromFile( "scenes/startmenu.scene" );
	}
	public NavHostPanel NavPanel { get; set; }

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
