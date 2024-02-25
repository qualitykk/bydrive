using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[GameResource("NPC Dialog", "dialog", "Character Dialog")]
public class DialogTree : GameResource
{
	public DialogEntry Root { get; set; }
	public static implicit operator DialogEntry(DialogTree tree) => tree.Root;
}
