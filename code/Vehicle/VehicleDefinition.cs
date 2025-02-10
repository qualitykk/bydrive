using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

[GameResource( "Vehicle Definition", "vehicle", "A Bydrive Vehicle" )]
[Icon( "electric_car" )]
public class VehicleDefinition : GameResource, IPrefabProvider
{
	public static IEnumerable<VehicleDefinition> GetAllVisible()
	{
		return ResourceLibrary.GetAll<VehicleDefinition>().Where( v => !v.Hidden );
	}
	public static VehicleDefinition GetDefault()
	{
		return GetAllVisible().FirstOrDefault();
	}
	public string Title { get; set; }
	public string FullTitle { get; set; }
	[TextArea] public string Description { get; set; }
	public bool Hidden { get; set; }
	public VehicleStats Stats { get;set; }
	public PrefabFile Prefab { get; set; }
	public List<AttachmentSlotPosition> AttachmentSlots { get; set; }
	[Category("Preview")] public Model PreviewModel { get; set; }
	[Category( "Preview" )] public Color PreviewTint { get; set; } = Color.White;
	[Category("Preview")] public Vector3 PreviewPosition { get; set; }
	[Category("Preview"), Range(0f, 1f)] public float DisplaySpeed { get; set; }
	[Category("Preview"), Range( 0f, 1f )] public float DisplayAcceleration { get; set; }
	[Category("Preview"), Range( 0f, 1f )] public float DisplayHandling { get; set; }
	[Category("Preview"), Range( 0f, 1f )] public float DisplayBoost { get; set; }
	public override string ToString()
	{
		return Title ?? ResourcePath;
	}
}

public class AttachmentSlotPosition
{
	[Hide] public Guid Id { get; set; } = Guid.NewGuid();
	public AttachmentSlot Slot { get; set; }
	public Vector3 LocalPosition { get; set; }
	public Rotation LocalRotation { get; set; }
}
