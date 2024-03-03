using Sandbox.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class OverworldCharacterInstance : Component, IInteractible
{
	[Property] public CharacterDefinition Character { get; set; }
	[Property] public DialogTree Dialog { get; set; }
	[Property] public SkinnedModelRenderer CharacterRenderer { get; set; }
	[Property] public BBox InteractionBounds { get; set; } = BBox.FromPositionAndSize(Vector3.Zero, 64f);
	protected override void OnEnabled()
	{
		base.OnStart();
		Assert.NotNull( Character );
		Assert.NotNull( Character.WorldModel );
		Assert.NotNull( CharacterRenderer );

		CharacterRenderer.Model = Character.WorldModel;
		CharacterRenderer.Set( "idle", true );
	}
	public bool OnInteract()
	{
		DialogBox.Show( Dialog );
		return true;
	}
	protected override void DrawGizmos()
	{
		const string DEFAULT_MODEL = "models/editor/spawnpoint.vmdl";
		base.DrawGizmos();

		Gizmo.Draw.Color = Color.Green;
		Gizmo.Draw.LineBBox(InteractionBounds);

		Model model = Character?.WorldModel ?? Model.Load( DEFAULT_MODEL );
		Gizmo.Draw.Color = Gizmo.Draw.Color.WithAlpha( (Gizmo.IsHovered || Gizmo.IsSelected) ? 0.7f : 0.5f );
		Gizmo.Hitbox.Model( model );
		Gizmo.Draw.Model( model );
	}
	BBox IInteractible.Bounds => InteractionBounds;
	string IInteractible.Title => "Talk";
	string IInteractible.Subtitle => Character?.Name;
	Vector3 IInteractible.Position => Transform.Position;
}
