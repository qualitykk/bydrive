using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public partial class VehicleController
{
	private List<AttachmentDefinition> _attachments = new();
	private Dictionary<GameObject, AttachmentDefinition> _attachmentInstances = new();
	public void AddAttachment(Transform localTransform, AttachmentDefinition attachment)
	{
		GameObject attachmentModel = new();
		attachmentModel.Parent = GameObject;
		attachmentModel.Transform.Local = localTransform;

		var modelRenderer = attachmentModel.Components.Create<SkinnedModelRenderer>();
		modelRenderer.Model = attachment.Model;

		_attachments.Add( attachment );
		_attachmentInstances.Add(attachmentModel, attachment);
	}
}
