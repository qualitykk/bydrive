using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class UserSettings
{
	public float SoundEffectVolume { get; set; }
	public float MusicVolume { get;set; }
	[Category("UI")] public VehicleSpeedUnit SpeedometerUnit { get;set; }
}
