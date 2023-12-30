using Sandbox;
using System.Runtime.InteropServices;

namespace Redrome;
public sealed class FinishLine : Component, Component.ITriggerListener
{
	const float DEFAULT_BOUND_SIZE = 10f;
	[Property] public BBox Bounds { get; set; } = new( new Vector3(-DEFAULT_BOUND_SIZE), new Vector3( DEFAULT_BOUND_SIZE ) );
	private PhysicsBody linePhysics;

	protected override void OnAwake()
	{
		linePhysics = new( Scene.PhysicsWorld );
		linePhysics.AddBoxShape( Bounds, Transform.Rotation );
		linePhysics.SetComponentSource( this );
	}
	protected override void DrawGizmos()
	{
		const float GIZMO_ALPHA = 0.25f;

		//if ( !Gizmo.IsSelected ) return;

		Color drawColor = Color.Blue;

		Gizmo.Draw.Color = drawColor.WithAlpha( GIZMO_ALPHA );
		Gizmo.Draw.SolidBox( Bounds );

		Gizmo.Draw.Color = drawColor;
		Gizmo.Draw.LineBBox( Bounds );

		Gizmo.Control.BoundingBox( "Bounds", Bounds, out var boundControl );
		Bounds = boundControl;
	}

	public void OnTriggerEnter( Collider other )
	{
		if(!other.Components.TryGet<VehicleController>(out var vehicle))
		{
			return;
		}

		Log.Info( "Vehicle Entered" );
	}

	public void OnTriggerExit( Collider other )
	{
		
	}
}
