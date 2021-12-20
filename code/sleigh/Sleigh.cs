using Sandbox;
using Sandbox.UI.Construct;
using System;

namespace ChristmasGame
{
	public partial class Sleigh : ModelEntity
	{
		[Net] public GridEntity Grid { get; set; }

		public Sleigh()
		{
			Transmit = TransmitType.Always;
		}

		public override void Spawn()
		{
			base.Spawn();

			SetModel( "models/test_sleigh.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Static );

			if(IsServer)
			{
				Grid = Create<GridEntity>();
				Grid.Parent = this;
				Grid.SizeX = 10;
				Grid.SizeY = 12;

			}
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			Grid?.Simulate( cl );

			//Rotation = new Angles(0.0f, Time.Now, 0.0f).ToRotation();
		}
	}
}
