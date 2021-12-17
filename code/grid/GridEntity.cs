using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{

	public partial class GridEntity : Entity
	{
		const float gridScale = 50.0f;

		[Net] public int SizeX { get; set; }
		[Net] public int SizeY { get; set; }

		[Net] List<GridNode> Nodes { get; set; } = new();

		Model TileModel;
		ModelEntity TileOverlay;

		public Vector3 OverlayPosition => new Vector3( -SizeX / 2.0f * gridScale, -SizeY / 2.0f * gridScale, 10.0f );

		public GridEntity()
		{
			Transmit = TransmitType.Always;
		}

		public override void Spawn()
		{
			base.Spawn();
		}

		public override void ClientSpawn()
		{
			base.ClientSpawn();

			CreateTileMesh();

		}

		void CreateTileMesh()
		{
			Assert.True( IsClient );

			Material tileMaterial = Material.Load( "materials/tile.vmat" );

			Mesh tileMesh = new Mesh( tileMaterial );

			Vector3[] points =
			{
				new Vector3( 0.0f, 0.0f, 0.0f ),
				new Vector3( SizeX * gridScale, 0.0f, 0.0f ),
				new Vector3( SizeX * gridScale, SizeY * gridScale, 0.0f ),
				new Vector3( 0.0f, SizeY * gridScale, 0.0f )
			};

			Vertex[] vertices =
			{
				new Vertex( points[0], Vector3.Up, Vector3.Right, new Vector2(SizeX, 0) ),
				new Vertex( points[1], Vector3.Up, Vector3.Right, new Vector2(0, 0) ),
				new Vertex( points[2], Vector3.Up, Vector3.Right, new Vector2(0, SizeY) ),
				new Vertex( points[3], Vector3.Up, Vector3.Right, new Vector2(SizeX, SizeY) ),
			};

			VertexBuffer vb = new VertexBuffer();
			vb.AddQuad( vertices[0], vertices[1], vertices[2], vertices[3] );

			tileMesh.CreateBuffers( vb );

			TileModel = Model.Builder.AddMesh( tileMesh ).Create(); //.AddCollisionHull(points)

		}

		public void SetTileOverlayVisible( bool visible )
		{
			Log.Info( "set visible " + visible );

			if(visible)
			{
				TileOverlay?.Delete();

				TileOverlay = Create<ModelEntity>();
				TileOverlay.Parent = this;
				TileOverlay.Position = OverlayPosition;
				TileOverlay.SetModel( TileModel );
			}
			else
			{
				TileOverlay?.Delete();
				TileOverlay = null;
			}
		}

		public GridNode GetNodeAt(int x, int y)
		{
			foreach(var node in Nodes)
			{
				if ( node.X == x && node.Y == y )
					return node;
			}

			return null;
		}

		public bool PlaceNode<T>(int x, int y, int direction = 0) where T : GridNode
		{
			Assert.True( IsServer );

			if ( GetNodeAt( x, y ) != null )
				return false;

			Log.Info("Creating node at " + x + ", " + y + " with direction " + direction);

			var node = Create<T>();
			node.Parent = this;

			node.X = x;
			node.Y = y;

			node.Position = new Vector3((x - SizeX / 2.0f) * gridScale + gridScale / 2.0f, (y - SizeY / 2.0f) * gridScale + gridScale / 2.0f, 0.0f);
			node.Direction = direction;

			node.SetModel( "models/props/cs_office/chair_office.vmdl" );

			Nodes.Add( node );

			return true;
		}

		public void UpdateNodes()
		{
			foreach ( var node in Nodes )
				node.Update();
		}

		public bool RayTest( Ray ray, ref int hitX, ref int hitY, ref Vector3 worldPos )
		{
			Transform worldTransform = Parent.Transform.ToWorld( new Transform( OverlayPosition ) );
			Plane gridPlane = new Plane( worldTransform.Position, Vector3.Up );
			Vector3? hit = gridPlane.Trace(ray);

			if ( hit == null )
				return false;

			DebugOverlay.Line( (Vector3)hit, (Vector3)hit + new Vector3(0.0f, 0.0f, 50.0f) );

			Vector3 localPos = worldTransform.PointToLocal( (Vector3)hit );
			localPos /= gridScale;
			//localPos += new Vector3(SizeX / 2.0f, SizeY / 2.0f, 0.0f);

			if ( localPos.x < 0 || localPos.x >= SizeX || localPos.y < 0 || localPos.y >= SizeY )
				return false;

			//Log.Info("local pos " + localPos + " hit pos " + (Vector3)hit );
			hitX = (int)localPos.x;
			hitY = (int)localPos.y;
			worldPos = worldTransform.PointToWorld( new Vector3( hitX * gridScale + gridScale / 2.0f, hitY * gridScale + gridScale / 2.0f, 0.0f ) );

			//Log.Info("grid position " + localPos.ToString());
			return true;
		}
	}
}
