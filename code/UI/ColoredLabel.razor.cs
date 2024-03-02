using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace Bydrive;

public partial class ColoredLabel : Panel
{
	public ColoredLabel Clear()
	{
		DeleteChildren( true );
		return this;
	}

	public ColoredLabel AddText( string text )
	{
		Add.Label( text );
		return this;
	}

	public ColoredLabel AddColoredText( string text, string color )
	{
		var label = Add.Label( text );
		label.Style.Set( "color", color );
		return this;
	}

	public ColoredLabel AddTextWithClasses( string text, string classes )
	{
		var label = Add.Label( text );
		label.Classes = classes;
		return this;
	}
}
