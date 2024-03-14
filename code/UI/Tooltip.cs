using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Bydrive;

public class Tooltip : Panel
{
	public static Tooltip Show(string text, string classnames, Vector2 position)
	{
		return new Tooltip( text, position, classnames );
	}
	public static Tooltip Show(string text, string classnames)
	{
		return new Tooltip(text, Mouse.Position, classnames );
	}
	private Tooltip(string text, Vector2 position, string classnames) : base()
	{
		Classes = classnames;
		Style.Position = PositionMode.Absolute;
		Style.Top = Length.ViewHeight( position.y );
		Style.Left = Length.ViewWidth( position.x );

		Add.Label( text );
	}
}
