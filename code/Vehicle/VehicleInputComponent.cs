using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public abstract class VehicleInputComponent : Component
{
	[Property] public RaceParticipant ParticipantInstance { get; set; }
	[Property] public VehicleController VehicleController { get; set; }

	protected abstract void BuildInput();
	protected override void OnUpdate()
	{
		if ( ParticipantInstance == null || VehicleController == null ) return;

		if(CanInput())
		{
			BuildInput();
		}
		else
		{
			ResetVehicleInputs();
		}
	}

	protected virtual void ResetVehicleInputs()
	{
		VehicleController.ResetInput();
	}

	public virtual bool CanInput()
	{
		return Race == null || (Race.HasStarted && !Race.HasParticipantFinished( ParticipantInstance ));
	}
}
