using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;
public class DialogEntry
{
	public const float DEFAULT_SPEED = 60.0f;

	public string Text { get; set; }
	public float Speed { get; set; } = DEFAULT_SPEED;
	[Hide] public string Title => Character?.Name ?? TitleOverride;
	[Hide] public Texture Image => Character?.GetDialogImage( Expression ) ?? ImageOverride;
	[Category("Character")] public CharacterDefinition Character { get; set; }
	[Category( "Character" )] public string Expression { get; set; }
	[Title( "Title" ), Category("Non-Character")] private string TitleOverride { get; set; }
	/// <summary>
	/// Image of the character speaking
	/// </summary>
	[Title("Image"), Category( "Non-Character" )] private Texture ImageOverride { get; set; }
	public DialogEntry Next { get; set; }
	public List<DialogResponse> Responses { get; set; }
	public Action OnFinished { get; set; }
	public DialogEntry()
	{
		
	}
	public DialogEntry(string text, float speed, CharacterDefinition character)
	{
		Text = text;
		Speed = speed;
		Character = character;
	}
	public DialogEntry( string text, float speed, string title = "", Texture image = null )
	{
		Text = text;
		TitleOverride = title;
		Speed = speed;
		ImageOverride = image;
	}
	public DialogEntry( string text, float speed, CharacterDefinition character, string expression, DialogEntry next ) : this( text, speed, character )
	{
		Expression = expression;
		Next = next;
	}
	public DialogEntry( string text, float speed, string titleOverride, Texture imageOverride, string expression, DialogEntry next ) : this( text, speed, titleOverride, imageOverride )
	{
		Expression = expression;
		Next = next;
	}

	public DialogEntry( string text, float speed, string titleOverride, Texture imageOverride, string expression, List<DialogResponse> respones, Action onFinished = null ) : this( text, speed, titleOverride, imageOverride )
	{
		Expression = expression;
		Responses = respones;
		OnFinished = onFinished;
	}

	public DialogEntry( string text, float speed, CharacterDefinition character, string expression, List<DialogResponse> respones, Action onFinished = null ) : this( text, speed, character )
	{
		Expression = expression;
		Responses = respones;
		OnFinished = onFinished;
	}

	public override string ToString()
	{
		return $"{Text} ({Title})";
	}
}

public class DialogResponse
{
	public string Title { get; set; }
	public DialogEntry Next { get; set; }
	public Action OnPicked { get; set; }

	public override string ToString()
	{
		if(Next != null)
		{
			return $"{Title} => {Next}";
		}
		
		if(OnPicked != null)
		{
			return $"{Title} | Action";
		}

		return Title;
	}
}
