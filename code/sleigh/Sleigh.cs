using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class Sleigh : ModelEntity
	{
		[Net] public GridEntity Grid { get; set; }

		public List<FestivePlayer> Players { get; } = new();

		Entity start;
		Entity end;
		float length;
		float dist = 0.0f;

		Entity parentDummy;


		public Sleigh()
		{
			Transmit = TransmitType.Always;

			
		}

		public void AddPlayer(FestivePlayer player)
		{
			player.ClientSleigh = this;
			//player.Position = Position;
			player.Parent = parentDummy;

			//player.Parent = this;
			//player.LocalPosition = new Vector3();

			Players.Add( player );
		}

		public override void Spawn()
		{
			base.Spawn();


			Log.Info("spawn server " + IsServer);

			SetModel( "models/sleigh.vmdl" );
			SetupPhysicsFromModel( PhysicsMotionType.Static );

			if(IsServer)
			{
				parentDummy = Create<Entity>();
				parentDummy.Position = Position;

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

		public void Reposition(Vector3 pos)
		{
			Vector3[] positions = new Vector3[Players.Count];

			//for ( int i = 0; i < Players.Count; i++ )
			//	positions[i] = Players[i].Position - Position;
			//positions[i] = Transform.PointToLocal( Players[i].Position );

			//foreach ( var player in Players )
			//	player.Parent = this;

			Position = pos;

			//foreach ( var player in Players )
			//{
			//	player.Parent = null;
			//	player.GroundEntity = this;
			//}

			//for ( int i = 0; i < Players.Count; i++ )
			//{
			//	Players[i].Position = positions[i] + Position;
			//	Players[i].GroundEntity = this;
			//}


			//Players[i].Position = Transform.PointToWorld( positions[i] );
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			Grid?.Simulate( cl );

			if(start == null || end == null)
			{
				start = Entity.FindByName( "sleigh_start" );
				end = Entity.FindByName( "sleigh_end" );
				length = (end.Position - start.Position).Length;

				Reposition( start.Position );
			}


			float speed = 50.0f;
			dist += speed * Time.Delta;

			if ( dist > length )
			{
				Log.Info("loop");
				dist = 0.0f;

				Reposition( start.Position );
			}

			//Vector3 target = Vector3.Lerp(start.Position, end.Position, );

			//Velocity = new Vector3(0.0f, -speed, 0.0f);
			//Position += Velocity * Time.Delta;

			Position = start.Position + new Vector3(0.0f, -dist, 0.0f);


			parentDummy.Position = Position;

			//Position.x += speed;
			//Rotation = new Angles(0.0f, Time.Now, 0.0f).ToRotation();
		}
	}
}
