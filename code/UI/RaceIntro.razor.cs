using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class RaceIntro
{
	[Property] public float AutoStopTime { get; set; } = 10f;

	RealTimeSince timeSinceStartDraw;
	bool startedDraw;
	bool finishedDraw;
	public void Stop()
	{
		finishedDraw = true;
		Race?.Start();
	}
	protected override void OnUpdate()
	{
		if ( finishedDraw ) return;

		if(ShouldDraw() && !startedDraw)
		{
			timeSinceStartDraw = 0;
			startedDraw = true;
			UI.HideRaceHud();
		}
		else if(!ShouldDraw() && startedDraw)
		{
			startedDraw = false;
		}

		if(startedDraw && timeSinceStartDraw >= AutoStopTime)
		{
			Stop();
		}
		else if(Input.Down(InputActions.USE) || Input.Down(InputActions.DIALOG_SKIP))
		{
			Stop();
		}
	}
	private bool ShouldDraw()
	{
		return !finishedDraw && RaceContext != null && RaceContext.CurrentDefinition != null && Race != null && !Race.HasCountdownStarted;
	}
	private string GetVariantTitle()
	{
		if( RaceContext == null || RaceContext.CurrentVariables == null || !RaceContext.CurrentVariables.Any() )
		{
			return "";
		}

		List<string> variants = new();

		foreach((string key, string value) in RaceContext.CurrentVariables)
		{
			TrackVariable variableDefinition = RaceContext.CurrentDefinition.Variables.Where(v => v.Key == key).FirstOrDefault();
			if(!string.IsNullOrEmpty(variableDefinition.Title))
			{
				variants.Add($"{variableDefinition.Title} {FormatVariable(variableDefinition, value)}");
			}
			else
			{
				variants.Add( value.ToTitleCase() );
			}
		}

		return string.Join( " ,", variants );
	}

	private string FormatVariable(TrackVariable definition, string value)
	{
		if(definition.Key == "route")
		{
			UI.FormatRoute( value );
		}

		return value.ToTitleCase();
	}
	protected override int BuildHash()
	{
		return HashCode.Combine( ShouldDraw() );
	}
}
