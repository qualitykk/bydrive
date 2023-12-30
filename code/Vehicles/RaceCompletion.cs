using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redrome;

public class RaceCompletion : Component
{
	public float TotalCompletion { get; set; }
	public RaceCheckpoint LastCheckpoint { get; set; }
	public List<RaceCheckpoint> NextCheckpoints { get; set; }
	public void UpdateCompletion()
	{

	}
}
