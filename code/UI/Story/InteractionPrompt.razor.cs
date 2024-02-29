using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class InteractionPrompt : PanelComponent
{
	IEnumerable<IInteractible> possibleInteractions;
	public bool HasInteractions()
	{
		bool canInteract = possibleInteractions?.Any() == true;
		return canInteract;
	}

	protected override void OnUpdate()
	{
		if(InteractionListener.Exists)
		{
			possibleInteractions = InteractionListener.GetBestInteractions();
		}
		else
		{
			possibleInteractions = null;
			UI.MakeMenuInactive( Panel );
		}

		if ( !HasInteractions() ) return;

		foreach(var action in possibleInteractions)
		{
			TryPress( action );
		}
	}
	private void TryPress( IInteractible action )
	{
		if ( ActiveMenu != null ) return;

		bool pressed = Input.Pressed( action.Input );
		if ( pressed )
		{
			action.OnInteract();
		}
	}
	protected override int BuildHash()
	{
		return HashCode.Combine( possibleInteractions );
	}
}
