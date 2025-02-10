using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[GameResource("Soundtrack", "ost", "Soundtrack with loop/transition support.")]
public class Soundtrack : GameResource
{
	public SoundFile Sound { get; set; }
	/// <summary>
	/// Loop starting point in seconds
	/// </summary>
	public float LoopStart { get; set; }
	/// <summary>
	/// Loop end point in seconds
	/// </summary>
	public float LoopEnd { get; set; }
}
