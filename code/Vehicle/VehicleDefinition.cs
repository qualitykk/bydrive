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
	public static VehicleDefinition GetDefault()
	{
		return ResourceLibrary.GetAll<VehicleDefinition>().FirstOrDefault();
	}
	public string Title { get; set; }
	public string Description { get; set; }
	public bool Hidden { get; set; }
	[Category("Preview")] public Model PreviewModel { get; set; }
	[Category("Preview")] public Vector3 PreviewPosition { get; set; }
	public VehicleStats Stats { get;set; }
	public PrefabFile Prefab { get; set; }
	public List<AttachmentSlotPosition> AttachmentSlots { get; set; }
	public override string ToString()
	{
		return Title;
	}
}

public class AttachmentSlotPosition
{
	[Hide] public Guid Id { get; set; } = Guid.NewGuid();
	public AttachmentSlot Slot { get; set; }
	public Vector3 LocalPosition { get; set; }
	public Rotation LocalRotation { get; set; }
}
