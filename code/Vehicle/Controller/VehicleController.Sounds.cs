using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleController
{
	const float DEFAULT_MIN_PITCH = 0.5f;
	const float DEFAULT_MAX_PITCH = 2f;
	[Category("Sound"), Property] public SoundEvent IdleSound { get; set; }
	[Category( "Sound" ), Property] public SoundEvent RevSound { get; set; }
	[Category( "Sound" ), Property] public SoundEvent SlowdownSound { get; set; }
	[Category( "Sound" ), Property] public float MinEnginePitch { get; set; } = DEFAULT_MIN_PITCH;
	[Category( "Sound" ), Property] public float MaxEnginePitch { get; set; } = DEFAULT_MAX_PITCH;

	private List<SoundHandle> activeSounds = new();
	SoundHandle engineSound;
	int engineMode = -1;
	float enginePitch = 1;
	float targetPitch = 1;
	public SoundHandle PlaySound( SoundEvent sound )
	{
		var handle = Sound.Play( sound, Transform.Position );
		activeSounds.Add( handle );
		return handle;
	}
	private void TickSounds()
	{
		TickEngineSound();

		foreach ( var sound in activeSounds )
		{
			sound.Position = Transform.Position;
		}
	}

	private void TickEngineSound()
	{ 
		float forwardSpeed = MathF.Abs( Transform.Local.VelocityToLocal( Body.Velocity ).x );

		float speedFraction = forwardSpeed / GetMaxSpeed();
		targetPitch = speedFraction.Remap( 0, 1, MinEnginePitch, MaxEnginePitch, false );
		enginePitch = enginePitch.LerpTo( targetPitch, MathF.Pow( 0.001f, Time.Delta) / 5 );

		if (engineMode > 0)
		{
			engineSound.Pitch = enginePitch;
		}

		if (MathF.Abs(forwardSpeed) < 50f)
		{
			if ( engineMode == 0 ) return;

			engineMode = 0;
			SetEngineSound( IdleSound );
		}
		else if(ThrottleInput != 0)
		{
			if ( engineMode == 1 ) return;

			engineMode = 1;
			SetEngineSound( RevSound );
		}
		else
		{
			if ( engineMode == 2 ) return;

			engineMode = 2;
			SetEngineSound( SlowdownSound );
		}
	}

	private void SetEngineSound(SoundEvent sound)
	{
		if ( engineSound != null && engineSound.Name == sound.ResourceName ) return;

		engineSound?.Stop();
		engineSound = PlaySound( sound );
	}
}
