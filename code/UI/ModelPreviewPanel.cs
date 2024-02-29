using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bydrive;

public class ModelPreviewPanel : ScenePanel
{
	public Model Model 
	{
		get => currentModel;
		set => requestedModel = value;
	}
	public string ModelPath
	{
		get => currentModel.ResourcePath;
		set => requestedModel = Model.Load( value );
	}
	public Rotation StartRotation { get; set; }
	public float ModelRotationSpeed { get; set; } = 1;
	public Vector3 CameraPosition { get; set; }
	public Angles CameraAngles { get; set; }
	public Color LightingColor { get; set; } = Color.White;
	private Model requestedModel;
	private Model currentModel;
	private SceneModel sceneModel;

	public ModelPreviewPanel() : base()
	{
		Camera.World = new();
	}
	protected override void OnParametersSet()
	{
		Camera.Position = CameraPosition;
		Camera.Angles = CameraAngles;
		new SceneDirectionalLight( World, Rotation.LookAt( Vector3.Down ), LightingColor );
	}
	public override void Tick()
	{
		if ( !World.IsValid() )
			return;

		if(requestedModel != currentModel)
		{
			SetModel( requestedModel );
		}

		if ( sceneModel == null || ModelRotationSpeed == 0 )
			return;

		
		// This doesnt work. investigate this in the future.

		float rotateDegrees = ModelRotationSpeed * ConstantSpin.DEGREES_PER_SECOND * Time.Delta;
		if(sceneModel.Rotation == default)
		{
			sceneModel.Rotation = Rotation.FromYaw( rotateDegrees );
		}
		else
		{
			sceneModel.Rotation = sceneModel.Rotation.RotateAroundAxis( Vector3.Up, rotateDegrees );
		}
	}
	private void SetModel(Model model)
	{
		if(model == null || model.IsError)
		{
			return;
		}
		
		sceneModel?.Delete();
		sceneModel = new( World, model, new(Vector3.Zero, StartRotation) );
		currentModel = model;
	}
}
