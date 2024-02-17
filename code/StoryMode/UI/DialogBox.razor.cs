using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class DialogBox
{
	const float DIALOG_SPEED = 15.0f;
	public static void Show(string text, float speed = DIALOG_SPEED, string name = "", List<string> responses = null)
	{
		Current?.SetMessage( text, speed, name, responses );
	}
	public static DialogBox Current { get; set; }
	public string Text { get; set; }
	public float Speed { get; set; }
	public string Name { get; set; }
	public List<string> Responses { get; set; }
	public Action<int> OnResponseSelected { get; set; }
	RealTimeSince timeSinceMessage;
	bool finished = false;
	public DialogBox()
	{
		Current = this;
	}
	public void SetMessage(string text, float speed, string name, List<string> responses = null)
	{
		Text = text;
		Speed = speed;
		Name = name;
		Responses = responses;

		timeSinceMessage = 0;
		finished = false;

		StateHasChanged();
	}
	protected override void OnUpdate()
	{
		if(Input.Pressed(InputActions.SKIP_DIALOG))
		{
			finished = true;
		}
	}
	public string GetCurrentText()
	{
		if ( finished ) return Text;

		int letters = MathX.FloorToInt(timeSinceMessage * Speed);
		finished = finished || letters >= Text?.Length;
		var text = Text?.Take( letters ).ToArray();
		return new string(text);
	}

	[ConCmd("ui_show_dialog")]
	public static void Command_Show(string text, string name, float speed = DIALOG_SPEED)
	{
		Show( text, speed, name );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( RealTime.Now );
		//return HashCode.Combine( Text, Name, Responses, timeSinceMessage, finished );
	}
}
