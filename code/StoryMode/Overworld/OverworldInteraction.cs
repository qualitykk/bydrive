using Sandbox.ActionGraphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class OverworldInteraction : Component, IInteractible
{
	[Property] public string InteractionTitle { get; set; } = "Inspect";
	[Property] public BBox InteractionBounds { get; set; } = BBox.FromPositionAndSize( Vector3.Zero, 64f );
	[Property] public Action Interaction { get; set; }

	Vector3 IInteractible.Position => Transform.Position;

	BBox IInteractible.Bounds => InteractionBounds;

	string IInteractible.Title => InteractionTitle;

	bool IInteractible.OnInteract()
	{
		Interaction?.Invoke();
		return true;
	}

	protected override void DrawGizmos()
	{
		base.DrawGizmos();
		Gizmo.Draw.Color = Color.Green;
		Gizmo.Draw.LineBBox( InteractionBounds );
	}
}
