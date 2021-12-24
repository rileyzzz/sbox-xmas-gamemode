using Sandbox;
using Sandbox.UI.Construct;
using System;

namespace ChristmasGame
{
	public partial class SleighCamera : Camera
	{
		public SleighCamera()
		{

		}
		public override void Update()
		{
			//use client sleigh reference
			if ( Local.Pawn is not FestivePlayer player )
				return;

			Vector3 sleighPos = player.ClientSleigh.Position;

			//Position = sleighPos + new Vector3(-440.0f, 0.0f, 440.0f);

			//Vector3 curPos = Position - sleighPos;

			Vector3 targetPos;
			Vector3 lookTarget;

			if ( player.TargetCannon == null )
			{
				targetPos = new Vector3( -640.0f, 0.0f, 640.0f );
				lookTarget = new Vector3();

				FieldOfView = 40;
			}
			else
			{
				//targetPos = player.TargetCannon.Position + new Vector3(0.0f, 0.0f, 20.0f) - sleighPos;
				//lookTarget = targetPos + new Vector3(100.0f, 0.0f, 0.0f);

				targetPos = new Vector3( 640.0f, 0.0f, -640.0f );
				lookTarget = targetPos + new Vector3( 100.0f, 0.0f, -35.0f );
				//lookTarget = targetPos + new Vector3( 100.0f, 80.0f, 10.0f );

				FieldOfView = 40;
			}

			//Position = sleighPos + Vector3.Lerp( curPos, targetPos, 0.1f );
			Position = sleighPos + targetPos;

			//Position = sleighPos + new Vector3(-640.0f, 0.0f, 640.0f);

			//Rotation = Rotation.LookAt(Target - Position, Vector3.Up);
			Rotation = Rotation.LookAt((lookTarget + sleighPos) - Position, Vector3.Up);

			//FieldOfView = 40;
			Viewer = null;
		}
	}
}
