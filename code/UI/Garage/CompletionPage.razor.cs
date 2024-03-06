using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;
public partial class CompletionPage
{
	private string GetAvatar()
	{
		return $"avatarbig:{Game.SteamId}";
	}

	void OnClickBack()
	{
		this.Navigate( "/" );
	}
}
