using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class UserSettings
{
	[Property] public float SoundEffectVolume { get; set; }
	[Property] public float MusicVolume { get;set; }
	[Property, Category("UI")] public VehicleSpeedUnit SpeedometerUnit { get;set; }
}
