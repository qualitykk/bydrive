namespace Bydrive;

public partial class StoryMenu
{
	internal bool Visible { get; set; } = false;
	protected override void OnUpdate()
	{
		SetClass( "active", Visible );

		if(Input.EscapePressed)
		{
			Visible = !Visible;
		}
	}
}
