using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class UserSettings
{
	public const string SETTINGS_KEY = "bydrive_settings_user";
	public static UserSettings Load()
	{
		return Cookie.Get<UserSettings>( SETTINGS_KEY, new() );
	}
	[Category("Sound"), MinMax( 0f, 2f )] public float VehicleVolume { get; set; } = 1.0f;
	[Category("Sound"), MinMax( 0f, 2f )] public float SoundEffectVolume { get; set; } = 1.0f;
	[Category("Sound"), MinMax(0f, 2f)] public float MusicVolume { get; set; } = 1.0f;
	[Category("UI")] public VehicleSpeedUnit SpeedometerUnit { get;set; }

	public void Save()
	{
		Cookie.Set( SETTINGS_KEY, this );
	}
}
