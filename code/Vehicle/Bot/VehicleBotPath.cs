using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class VehicleBotPath : Component, Component.ITriggerListener
{
	[Property] public List<VehicleBotPath> NextPaths { get; set; } = new();
	public Vector3 GetTargetPosition(Vector3 current)
	{
		return Transform.Position;
	}
	protected override void DrawGizmos()
	{
		Gizmo.Draw.Color = Color.Orange;

		if ( NextPaths == null ) return;

		foreach ( var path in NextPaths )
		{
			Gizmo.Draw.Line( Vector3.Zero, Transform.World.PointToLocal( path.Transform.Position ) );
		}
	}

	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if ( other.Components.TryGet<VehicleBot>( out var bot, FindMode.EverythingInSelfAndDescendants ) )
		{
			bot.currentBotPath = this;
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
	}
}
