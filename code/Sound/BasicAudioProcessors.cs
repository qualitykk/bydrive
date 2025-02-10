using Sandbox.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;
public static class AudioProcessorExtensions
{
	public static T Get<T>( this PerChannel<T> sample, AudioChannel channel ) => sample.Value[channel.Get()];
	public static void Set<T>(this PerChannel<T> sample, AudioChannel channel, T value) => sample.Value[channel.Get()] = value;
}

[Title("Custom Low Pass")]
public class LowPassProcessor : AudioProcessor
{
	[Hide]
	private PerChannel<float> previousSample = (PerChannel<float>)0.0f;

	/// <summary>
	/// Controls the strength of the filtering. Range is 0.0 to 1.0.
	/// Lower values mean more smoothing (stronger low-pass effect).
	/// </summary>
	[Range( 0.0f, 1f, 0.01f, true, true )]
	public float Amount { get; set; } = 0.5f;

	protected override void ProcessSingleChannel( AudioChannel c, Span<float> input )
	{
		float passFactor = Amount.Remap( 0.0f, 1f, 1f, 0.0f, true );
		for ( int index = 0; index < input.Length; ++index )
		{
			input[index] = (float)((passFactor * input[index]) + (1.0 - Amount) * previousSample.Get(c));
			previousSample.Set(c, input[index]);
		}
	}
}
[Title("Custom High Pass")]
public class HighPassProcessor : AudioProcessor
{
	[Hide]
	private PerChannel<float> previousInput = (PerChannel<float>)0.0f;
	[Hide]
	private PerChannel<float> previousOutput = (PerChannel<float>)0.0f;

	/// <summary>
	/// Controls the strength of the filtering. Range is 0.0 to 1.0.
	/// Higher values mean more of the high frequencies are allowed through.
	/// </summary>
	[Range( 0.0f, 1f, 0.01f, true, true )]
	public float Amount { get; set; } = 0.5f;

	protected override void ProcessSingleChannel( AudioChannel c, Span<float> input )
	{
		float passFactor = (1f - (float)Math.Pow( (double)Amount, 2.0 )).Clamp( 0.0f, 1f );
		for ( int index = 0; index < input.Length; ++index )
		{
			float currentInput = input[index];
			input[index] = passFactor * (previousOutput.Get( c ) + currentInput - previousInput.Get( c ));
			previousInput.Set( c, currentInput );
			previousOutput.Set( c, input[index] );
		}
	}
}
