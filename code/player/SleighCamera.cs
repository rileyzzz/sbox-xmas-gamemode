using Sandbox;
using Sandbox.UI.Construct;
using System;

namespace ChristmasGame
{
	public partial class SleighCamera : Camera
	{
		public override void Update()
		{
			//use client sleigh reference

			Position = new Vector3(0.0f, 280.0f, 240.0f);

			Vector3 Target = new Vector3(0.0f, 0.0f, 0.0f);
			Rotation = Rotation.LookAt(Target - Position, Vector3.Up);

			FieldOfView = 70;
			Viewer = null;
		}
	}
}
