using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class InputRecorder
{
	public VehicleController Vehicle { get; private set; }
	public bool Started { get; private set; }
	public bool Finished { get; private set; }
	public List<TimestampedVehicleInput> Timestamps { get; set; } = new List<TimestampedVehicleInput>();
	TimeSince timeSinceRecordingStart;
	IDisposable sceneTickHook;
	public InputRecorder(VehicleController vehicle)
	{
		Vehicle = vehicle;
	}
	public void Start()
	{
		Timestamps.Clear();
		timeSinceRecordingStart = 0;
		Started = true;

		sceneTickHook = Game.ActiveScene.AddHook( GameObjectSystem.Stage.FinishUpdate, 0, Tick, "InputRecorder", "Records input for race replays." );
	}

	public void Stop()
	{
		Started = false;
		Finished = true;

		sceneTickHook?.Dispose();
	}

	private void Tick()
	{
		Timestamps.Add( new()
		{
			Time = timeSinceRecordingStart,
			Input = new( Vehicle )
		} );
	}
}

public struct TimestampedVehicleInput
{
	public float Time { get; set; }
	public VehicleInputState Input { get; set; }
}
