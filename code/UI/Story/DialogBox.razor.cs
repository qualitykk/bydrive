using Sandbox.ActionGraphs;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class DialogBox : PanelComponent
{
	const float RESPONSE_HOLD_COOLDOWN = 0.3f;
	public static void Show(DialogEntry entry)
	{
		Current?.SetMessage( entry );
	}

	[ActionGraphNode("dialog.show")]
	[Title("Show Message"), Group("Dialog"), Icon( "question_answer" )]
	public static void Show(string text, string name = "", float speed = DialogEntry.DEFAULT_SPEED)
	{
		DialogEntry entry = new(text, speed, name);
		Current?.SetMessage(entry );
	}
	[ActionGraphNode( "dialog.show_character" )]
	[Title( "Show Message (Character)" ), Group( "Dialog" ), Icon( "question_answer" )]
	public static void Show( string text, CharacterDefinition character, float speed = DialogEntry.DEFAULT_SPEED )
	{
		DialogEntry entry = new( text, speed, character );

		Current?.SetMessage( entry );
	}
	[ActionGraphNode( "dialog.show_tree" )]
	[Title( "Show Message (Resource)" ), Group( "Dialog" ), Icon( "question_answer" )]
	public static void Show(DialogTree dialog)
	{
		Current?.SetMessage( dialog.Root );
	}
	public static DialogBox Current { get; set; }
	public DialogEntry Entry { get; set; }
	public string Text => Entry?.Text;
	public float Speed => Entry?.Speed ?? 0f;
	public string Title => Entry?.Title;
	RealTimeSince timeSinceMessage;
	RealTimeSince timeSinceResponseChanged;
	bool finished = false;

	int selectedIndex;
	DialogResponse selectedResponse;
	public DialogBox()
	{
		Current = this;
	}
	public void SetMessage(DialogEntry entry)
	{
		Entry?.OnFinished?.Invoke();
		timeSinceMessage = 0;
		timeSinceResponseChanged = 0;
		finished = false;

		Entry = entry;
		selectedIndex = 0;
		selectedResponse = entry?.Responses?.ElementAtOrDefault( 0 );

		StateHasChanged();
	}
	public void Next()
	{
		if(ShowResponses())
		{
			selectedResponse.OnPicked?.Invoke();
			SetMessage( selectedResponse.Next );
		}
		else
		{
			SetMessage( Entry?.Next ?? default );
		}
	}
	protected override void OnUpdate()
	{
		if(Panel.IsVisible)
		{
			UI.MakeMenu( Panel );
		}
		else
		{
			UI.MakeMenuInactive( Panel );
		}

		if(Input.Pressed(InputActions.DIALOG_SKIP) || Input.Pressed(InputActions.USE))
		{
			if(finished)
			{
				Next();
				return;
			}

			finished = true;
		}

		// TODO: Prevent movement
		if ( ShowResponses())
		{
			if(Input.Pressed(InputActions.DIALOG_DOWN))
			{
				MoveSelection( 1 );
			}
			else if(Input.Pressed(InputActions.DIALOG_UP))
			{
				MoveSelection( -1 );
			}
			else
			{
				float forwardInput = Input.AnalogMove.x;
				if ( forwardInput != 0 && timeSinceResponseChanged > RESPONSE_HOLD_COOLDOWN )
				{
					MoveSelection( -MathF.Sign( forwardInput ) );
				}
			}
		}
	}
	public string GetCurrentText()
	{
		if ( finished ) return Text;

		int letters = MathX.FloorToInt(timeSinceMessage * Speed);
		finished = letters >= Text?.Length;
		var text = Text?.Take( letters ).ToArray();
		return new string(text);
	}
	private bool ShowResponses()
	{
		return finished && Entry?.Responses?.Any() == true;
	}
	private void MoveSelection(int offset)
	{
		var responses = Entry.Responses;
		selectedIndex += offset;

		if(selectedIndex < 0)
		{
			selectedIndex = responses.Count - 1;
		}
		else if(selectedIndex >= responses.Count)
		{
			selectedIndex = 0;
		}

		selectedResponse = responses[selectedIndex];
		timeSinceResponseChanged = 0;
	}

	[ConCmd("ui_show_dialog")]
	private static void Command_Show(string text, string name, float speed = DialogEntry.DEFAULT_SPEED)
	{
		Show( text, name, speed );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( RealTime.Now, Entry );
	}
}
