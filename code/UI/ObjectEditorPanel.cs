using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class ObjectEditorPanel : Panel
{
	private object target;
	public object Target { 
		get => target; 
		set
		{
			bool rebuild = target?.GetHashCode() != value?.GetHashCode();
			target = value;
			if( rebuild )
			{
				BuildProperties();
			}
		}
	}

	public Action Save { get; set; }
	private T GetValue<T>(PropertyDescription prop) => (T)prop.GetValue( target );

	private void SetValue(PropertyDescription prop, object value) => prop.SetValue( target, value );

	private void BuildProperties()
	{
		DeleteChildren();

		PropertyDescription[] properties = TypeLibrary.GetPropertyDescriptions(target);

		foreach( var group in properties.OrderBy(p => p.Order)
										.GroupBy(p => p.Group))
		{
			Panel groupPanel = AddGroup( group.Key );
			foreach(PropertyDescription prop in group)
			{
				Panel propertyPanel = new( groupPanel, "property" );
				propertyPanel.Add.Label( prop.Title ?? prop.Name, "name" );

				var control = AddPropertyControl( prop );
				control.AddClass( "control" );
				propertyPanel.AddChild(control);
			}
		}

		Focus();
	}
	private Panel AddGroup(string group)
	{
		Panel groupPanel = Add.Panel( "group" );
		if(!string.IsNullOrWhiteSpace(group))
		{
			groupPanel.Add.Label( group, "group_name" );
		}
		return groupPanel;
	}
	private Panel AddPropertyControl(PropertyDescription prop)
	{
		Type t = prop.PropertyType;
		var minMax = prop.GetCustomAttribute<MinMaxAttribute>();

		if ( t == typeof( string ) )
		{
			TextEntry entry = new()
			{
				Text = GetValue<string>( prop )
			};
			entry.AddEventListener( "value.changed", () =>
			{
				SetValue( prop, float.Parse( entry.Value ) );
				DoSave();
			} );
			return entry;
		}
		else if ( t.IsAssignableTo(typeof(float)) )
		{
			if ( minMax != null )
			{
				// Slider
				SliderControl slider = new()
				{
					Min = minMax.MinValue,
					Max = minMax.MaxValue,
					Step = t == typeof( int ) ? 1.0f : 0.1f,
					Value = GetValue<float>( prop ),
					ShowRange = false,
				};
				slider.OnValueChanged += ( float value ) =>
				{
					SetValue(prop, value);
					DoSave();
				};
				return slider;
			}
			else
			{
				TextEntry entry = new()
				{
					Value = GetValue<float>( prop ).ToString(),
					Numeric = true
				};
				entry.AddEventListener( "value.changed", () =>
				{
					SetValue( prop, float.Parse( entry.Value ) );
					DoSave();
				} );

				return entry;
			}
		}
		else if ( t == typeof( bool ) )
		{
			Checkbox check = new()
			{
				Checked = GetValue<bool>( prop )
			};
			check.ValueChanged += ( bool value ) =>
			{
				SetValue( prop, value );
				DoSave();
			};
			return check;
		}
		else if ( t.IsEnum )
		{
			DropDown drop = new();
			drop.BuildOptions = () => BuildDropdownOptions(t);
			drop.Value = GetValue<Enum>(prop);
			drop.ValueChanged += ( string value ) =>
			{
				SetValue( prop, Enum.Parse(t, drop.Value.ToString()) );
				DoSave();
			};
			return drop;
		}

		throw new ArgumentException($"Cant create editor for {t.Name}!");
	}

	private List<Option> BuildDropdownOptions(Type enumType)
	{
		List<Option> options = new();
		var values = Enum.GetValues( enumType );
		var valueDisplay = DisplayInfo.ForEnumValues( enumType );
		for ( int i = 0; i < values.Length; i++ )
		{
			DisplayInfo display = valueDisplay[i];
			options.Add( new( display.Name, display.Icon, values.GetValue(i) ) );
		}

		return options;
	}

	private void DoSave()
	{
		CreateEvent( "save" );
		Save?.Invoke();
	}
}
