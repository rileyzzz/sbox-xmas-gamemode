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

			SetModel( "models/sleigh.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Static );

			if(IsServer)
			{
				Grid = Create<GridEntity>();
				Grid.Parent = this;
				Grid.SizeX = 10;
				Grid.SizeY = 12;

				Grid.PlaceNode<EngineNode>( "engine", 0, 10, 2, 0 );
				Grid.PlaceNode<EngineNode>( "engine", 0, 7, 2, 0 );
				Grid.PlaceNode<EngineNode>( "engine", 0, 4, 2, 0 );
				Grid.PlaceNode<EngineNode>( "engine", 0, 1, 2, 0 );

				Grid.PlaceNode<CannonNode>( "cannon", 9, 10, 0, 0 );
				Grid.PlaceNode<CannonNode>( "cannon", 9, 7, 0, 0 );
				Grid.PlaceNode<CannonNode>( "cannon", 9, 4, 0, 0 );
				Grid.PlaceNode<CannonNode>( "cannon", 9, 1, 0, 0 );

				//defaults
				Grid.PlaceNode<GridNode>( "mine", 2, 10, 2, 0 );
				Grid.PlaceNode<GridNode>( "conveyorbelt", 1, 10, 2, 0 );


				Grid.PlaceNode<GridNode>( "factory", 4, 10, 0, 0 );
				Grid.PlaceNode<GridNode>( "conveyorbelt", 5, 10, 0, 0 );
				Grid.PlaceNode<GridNode>( "boxer", 6, 10, 0, 0 );
				Grid.PlaceNode<GridNode>( "conveyorbelt", 7, 10, 0, 0 );
				Grid.PlaceNode<GridNode>( "wrapper", 8, 10, 0, 0 );
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
