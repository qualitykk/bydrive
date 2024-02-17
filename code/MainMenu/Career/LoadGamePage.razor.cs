using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class LoadGamePage
{
	void OnClickSave(SaveFile save)
	{
		StoryMode.Load( save );
	}
	void OnClickBack()
	{
		StartMenu.Current.NavPanel?.GoBack();
	}
}
