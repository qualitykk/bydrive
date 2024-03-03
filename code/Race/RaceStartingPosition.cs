﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[EditorHandle( "materials/gizmo/login.png" )]
[Icon( "login" )]
public class RaceStartingPosition : Component
{
	public const int FIRST_PLACE = 1;
	[Property] public int Placement { get; set; } = FIRST_PLACE;

	protected override void DrawGizmos()
	{
		const float TEXT_VERTICAL_OFFSET = 24f;
		const float TEXT_SIZE = 16f;
		Color textColor = Color.Blue;
		Color lineColor = Color.Yellow;

		Gizmo.Draw.Color = textColor;
		int displayPlacement = Placement - FIRST_PLACE + 1;
		Gizmo.Draw.Text( $"<<{displayPlacement}>>", new(Vector3.Up * TEXT_VERTICAL_OFFSET), size: TEXT_SIZE );

		/*
		Should work but doesnt?
		*/
		Gizmo.Draw.Color = lineColor;
		Gizmo.Draw.Line( Vector3.Zero, Transform.LocalRotation.Forward * 96f );
	}
}
