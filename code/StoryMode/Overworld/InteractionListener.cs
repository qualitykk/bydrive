using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class InteractionListener : Component, Component.ITriggerListener
{
	public static IEnumerable<IInteractible> GetBestInteractions()
	{
		var interactions = Current.availableInteractions;
		return interactions.OrderBy(interaction => Current.Transform.Position.DistanceSquared(interaction.Position));
	}
	public static bool Exists => Current != null;
	internal static InteractionListener Current { get; set; }
	protected override void OnEnabled()
	{
		Current = this;
	}
	protected override void OnDisabled()
	{
		if(Current == this)
		{
			Current = null;
		}
	}
	List<IInteractible> availableInteractions = new();
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		var usableComponents = other.Components.GetAll<IInteractible>();
		if ( !usableComponents.Any() )
			return;
		usableComponents = usableComponents.Where( c => !availableInteractions.Contains( c ) );

		availableInteractions.AddRange(usableComponents);
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		var usableComponents = other.Components.GetAll<IInteractible>();
		if ( !usableComponents.Any() )
			return;

		foreach(var component in usableComponents)
		{
			availableInteractions.Remove( component );
		}
	}
}
