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
			if ( Local.Pawn is not FestivePlayer player )
				return;

			Vector3 sleighPos = player.ClientSleigh.Position;

			//Position = sleighPos + new Vector3(-440.0f, 0.0f, 440.0f);
			Position = sleighPos + new Vector3(-640.0f, 0.0f, 640.0f);

			Vector3 Target = sleighPos;
			Rotation = Rotation.LookAt(Target - Position, Vector3.Up);

			FieldOfView = 40;
			Viewer = null;
		}
	}
}
