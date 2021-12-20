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

		[Net] public List<GridNode> Nodes { get; set; } = new();
		//[Net] Dictionary<Tuple<int, int>, GridNode> Nodes { get; set; } = new();
		[Net] public List<GridItem> Items { get; set; } = new();

		Model TileModel;
		ModelEntity TileOverlay;

		public Vector3 OverlayPosition => new Vector3( -SizeX / 2.0f * gridScale, -SizeY / 2.0f * gridScale, 4.0f );

		GridNode _selectedNode;
		public GridNode SelectedNode
		{
			get => _selectedNode;
			set
			{
				_selectedNode = value;
				UpdateSelectedNode();
			}
		}

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

		public void UpdateSelectedNode()
		{
			foreach ( var node in Nodes )
				node.SetMaterialOverride( "" );

			if ( SelectedNode == null )
				return;

			SelectedNode.SetMaterialOverride( Material.Load( "materials/hint.vmat" ) );
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			foreach ( var node in Nodes )
				node.Tick();

			List<GridItem> staleItems = new();

			foreach ( var item in Items )
			{
				item.Update();

				if ( GetNodeAt( (int)Math.Floor(item.Pos.x), (int)Math.Floor(item.Pos.y) ) == null )
					staleItems.Add( item );
			}

			foreach ( var item in staleItems )
				DeleteItem( item );
		}

		public void DeleteItem(GridItem item)
		{
			item.Delete();
			Items.Remove( item );
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
			//if ( Nodes.TryGetValue( new Tuple<int, int>( x, y ), out GridNode node ) )
			//	return node;
			//return Nodes.Find( node => node.X == x && node.Y == y );

			foreach(var node in Nodes)
			{
				if ( node.X == x && node.Y == y )
					return node;
			}

			return null;
		}

		public List<GridItem> GetItemsInTile(int x, int y)
		{
			List<GridItem> items = new List<GridItem>();

			foreach(var item in Items)
			{
				if ( x == (int)Math.Floor(item.Pos.x) && y == (int)Math.Floor(item.Pos.y) )
					items.Add( item );
			}

			return items;
		}

		public bool PlaceNode<T>( string type, int x, int y, int direction = 0, int tier = 0 ) where T : GridNode
		{
			Assert.True( IsServer );

			if ( GetNodeAt( x, y ) != null )
				return false;

			Log.Info("Creating node at " + x + ", " + y + " with direction " + direction);

			var node = Create<T>();
			node.Parent = this;
			node.SetType( type, tier );

			node.X = x;
			node.Y = y;

			node.Position = new Vector3((x - SizeX / 2.0f) * gridScale + gridScale / 2.0f, (y - SizeY / 2.0f) * gridScale + gridScale / 2.0f, 0.0f);
			node.Direction = direction;

			//node.SetModel( "models/props/cs_office/chair_office.vmdl" );

			//Nodes.Add( new Tuple<int, int>(x, y), node );
			Nodes.Add( node );

			return true;
		}

		public void PlaceItem( string type, int x, int y )
		{
			Assert.True( IsServer );

			var item = new GridItem();
			item.Grid = this;
			item.SetType(type);

			item.Pos = new Vector2( x + 0.5f, y + 0.5f );

			Items.Add( item );
		}

		public void UpdateNodes()
		{
			foreach ( var node in Nodes )
				node.Update();
		}

		public Vector3 GridToLocal(Vector2 pos)
		{
			return new Vector3( (pos.x - SizeX / 2) * gridScale, (pos.y - SizeY / 2) * gridScale, 0.0f );
		}

		public Vector3 GridToWorld(Vector2 pos)
		{
			Transform worldTransform = Parent.Transform.ToWorld( new Transform( OverlayPosition ) );
			return worldTransform.PointToWorld( new Vector3( pos.x, pos.y, 0.0f ) * gridScale );
		}

		public Vector2 WorldToGrid(Vector3 pos)
		{
			Transform worldTransform = Parent.Transform.ToWorld( new Transform( OverlayPosition ) );
			return worldTransform.PointToLocal( pos ) / gridScale;
		}

		public bool RayTest( Ray ray, ref int hitX, ref int hitY, ref Vector3 worldPos )
		{
			Transform worldTransform = Parent.Transform.ToWorld( new Transform( OverlayPosition ) );
			Plane gridPlane = new Plane( worldTransform.Position, Vector3.Up );
			Vector3? hit = gridPlane.Trace(ray);

			if ( hit == null )
				return false;

			DebugOverlay.Line( (Vector3)hit, (Vector3)hit + new Vector3(0.0f, 0.0f, 50.0f) );

			Vector3 localPos = WorldToGrid( (Vector3)hit );

			if ( localPos.x < 0 || localPos.x >= SizeX || localPos.y < 0 || localPos.y >= SizeY )
				return false;

			//Log.Info("local pos " + localPos + " hit pos " + (Vector3)hit );
			hitX = (int)localPos.x;
			hitY = (int)localPos.y;
			worldPos = GridToWorld( new Vector3( hitX + 0.5f, hitY + 0.5f, 0.0f ) );

			//Log.Info("grid position " + localPos.ToString());
			return true;
		}
	}
}
