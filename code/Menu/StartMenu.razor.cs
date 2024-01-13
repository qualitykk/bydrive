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
	NavHostPanel NavPanel { get; set; }
	public static void Open()
	{
		GameManager.ActiveScene.LoadFromFile( "scenes/startmenu.scene" );
	}

	protected override void OnAwake()
	{
		base.OnAwake();
		PlayMusic();
	}

	protected override void OnEnabled()
	{
		base.OnEnabled();
		NavPanel?.Navigate( "/front" );
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
