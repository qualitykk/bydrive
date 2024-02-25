using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public interface IInteractible
{
	public bool OnInteract();
	public Vector3 Position { get; }
	public BBox Bounds { get; }
	public string Title { get; }
	public virtual string Input => InputActions.USE;
	public virtual string Subtitle => "";
	public virtual InteractionHint Hint => new(Title, Input, Subtitle);
	public virtual bool CanUse()
	{
		return true;
	}
}

public class InteractionHint
{
	public string Title { get; set; }
	public string Input { get; set; }
	public string Subtitle { get; set; }
	public InteractionHint( string title, string input, string subtitle = "" )
	{
		Title = title;
		Input = input;
		Subtitle = subtitle;
	}
}
