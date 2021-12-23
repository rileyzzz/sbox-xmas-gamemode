using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ChristmasGame
{

	public partial class InventoryItem : BaseNetworkable
	{
		[Net] public string Type { get; set; }
		[Net] public int Tier { get; set; }
		[Net] public int Count { get; set; }

		public InventoryItem(string type, int tier)
		{
			Type = type;
			Tier = tier;
			Count = 0;
		}

		public InventoryItem()
		{
		}
	}

	public partial class FestivePlayer : Player
	{
		[Net] public Sleigh ClientSleigh { get; set; }

		bool GridHovered = false;
		public bool BuildMode = false;

		int BuildRotation = 0;
		int BuildTileX = 0;
		int BuildTileY = 0;
		
		ModelEntity BuildHint;
		ModelEntity BuildHintArrow;

		InventoryItem _activeItem;
		public InventoryItem ActiveItem
		{
			get => _activeItem;
			set
			{
				_activeItem = value;
				UpdateHintModel();
			}
		}

		public CannonNode TargetCannon = null;


		[Net] public IList<InventoryItem> NodeInventory { get; set; }

		public FestivePlayer()
		{
			NodeInventory = new List<InventoryItem>();

			if ( IsServer )
			{
				foreach(var type in ChristmasGame.Config.nodes)
				{
					if ( type.Value.type == "consumer" )
						continue;

					for( int i = 0; i < type.Value.tiers.Count; i++ )
					{
						var item = new InventoryItem( type.Key, i );
						//if ( type.Key == "factory" )
						//	item.Count = 1;
						//if ( type.Key == "boxer" )
						//	item.Count = 1;
						//if ( type.Key == "wrapper" )
						//	item.Count = 1;
						//item.Count = 1;

						if ( i == 0 && type.Key == "conveyorbelt" )
							item.Count = 999;

						NodeInventory.Add( item );
					}

					Log.Info("adding to node inventory");
				}
			}
		}

		static void UpdateHUD()
		{
			if ( !Local.Pawn.IsClient )
				return;

			foreach( var child in Local.Hud.Children )
				(child as ChristmasHUD).Update();
		}

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			//Controller = new WalkController();
			Controller = new TopDownController();

			//Animator = new StandardPlayerAnimator();
			Animator = new TopDownAnimator();

			Camera = new SleighCamera();
			
			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			base.Respawn();
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			SimulateActiveChild( cl, ActiveChild );


			if( IsClient )
			{
				if ( BuildMode )
				{
					GridNode hoveredNode = NodeTrace();

					if(hoveredNode != null)
						GridHovered = false;
					else
						UpdateHintLocation();

					BuildHint.EnableDrawing = GridHovered;
					BuildHintArrow.EnableDrawing = GridHovered;
				}
			}

		}

		GridNode NodeTrace()
		{
			Ray tr = new Ray( Input.Cursor.Origin, Input.Cursor.Direction );
			var result = Trace.Ray( tr, 1000.0f ).HitLayer(CollisionLayer.Hitbox, true).WithAllTags( "festive_node" ).Run();

			if ( result.Hit && result.Entity is GridNode node )
				return node;

			return null;
		}

		void UpdateHintLocation()
		{
			Ray tr = new Ray( Input.Cursor.Origin, Input.Cursor.Direction );

			int hitX = 0;
			int hitY = 0;
			Vector3 worldPos = new Vector3();
			if ( ClientSleigh.Grid.RayTest( tr, ref hitX, ref hitY, ref worldPos ) )
			{
				//Log.Info("hit tile " + hitX + " " + hitY);

				BuildHint.Position = worldPos;
				//BuildHint.Position = Vector3.Lerp( BuildHint.Position, worldPos, 0.8f );

				BuildTileX = hitX;
				BuildTileY = hitY;

				GridHovered = true;
			}
			else
				GridHovered = false;


			Rotation targetRotation = Rotation.From( new Angles( 0.0f, BuildRotation * 90.0f, 0.0f ) );
			BuildHint.Rotation = Rotation.Lerp( BuildHint.Rotation, targetRotation, 0.4f );
		}

		public override void OnKilled()
		{
			base.OnKilled();

			EnableDrawing = false;
		}

		void UpdateHintModel()
		{
			if ( BuildHint == null || BuildHintArrow == null )
				return;

			BuildHint.SetModel( ActiveItem != null ? ChristmasGame.Config.nodes[ActiveItem.Type].tiers[ActiveItem.Tier].model : "" );
			BuildHint.SetMaterialOverride( Material.Load( "materials/hint.vmat" ) );

			BuildHintArrow.EnableDrawing = ActiveItem != null;
		}

		public void ToggleBuildMode()
		{
			BuildMode = !BuildMode;

			if(BuildMode)
			{
				Log.Info("Entering build mode.");

				BuildHint = Create<ModelEntity>();

				BuildHintArrow = Create<ModelEntity>();
				BuildHintArrow.Parent = BuildHint;
				BuildHintArrow.SetModel( "models/hint_arrow.vmdl" );

				//BuildHint.SetModel( "models/props/cs_office/chair_office.vmdl" );
				UpdateHintModel();
			}
			else
			{
				Log.Info("Exiting build mode.");

				BuildHintArrow.Delete();
				BuildHintArrow = null;

				BuildHint.Delete();
				BuildHint = null;

				ClientSleigh.Grid.SelectedNode = null;
			}

			ClientSleigh.Grid.SetTileOverlayVisible( BuildMode );

			UpdateHUD();
		}

		void RotateBuildLeft()
		{
			if ( --BuildRotation < 0 )
				BuildRotation = 3;

			//BuildHint.Rotation = new Angles( 0.0f, BuildRotation * 90.0f, 0.0f ).ToRotation();
		}

		void RotateBuildRight()
		{
			if ( ++BuildRotation > 3 )
				BuildRotation = 0;

			//BuildHint.Rotation = new Angles( 0.0f, BuildRotation * 90.0f, 0.0f ).ToRotation();
		}

		public void PlaceNodeServer( Client cl, string type, int x, int y, int dir, int tier )
		{
			if ( !IsServer )
				return;

			Log.Info( "placing node" );
			ClientSleigh.Grid.PlaceNode<GridNode>( type, x, y, dir, tier );
		}


		//public void PlaceItemServer( string type, int x, int y )
		//{
		//	if ( !IsServer )
		//		return;

		//	Log.Info( "placing node" );
		//	ClientSleigh.Grid.PlaceItem( type, x, y );
		//}

		//[ServerCmd]
		//static void PlaceNode( string type, int x, int y, int dir, int tier )
		//{
			
			
		//}

		//[ServerCmd]
		//static void PlaceItem( string type, int x, int y )
		//{
		//	Entity pawn = ConsoleSystem.Caller.Pawn;
		//	(pawn as FestivePlayer).PlaceItemServer( type, x, y );
		//}

		[ClientRpc]
		public static void NodePlaced()
		{
			Log.Info("node placed client callback");

			if ( Local.Pawn is not FestivePlayer player )
				return;

			if ( player.ActiveItem != null && player.ActiveItem.Count == 0 )
				player.ActiveItem = null;

			player.ClientSleigh.Grid.SelectedNode = null;
			UpdateHUD();
		}

		[ServerCmd]
		static void Place( string type, int tier, int x, int y, int dir )
		{
			Client cl = ConsoleSystem.Caller;

			if ( cl.Pawn is not FestivePlayer player )
				return;

			//Log.Info("Attempting to place item " + itemId);
			//var item = (player.NodeInventory as List<InventoryItem>).Find( a => a.NetworkIdent == itemId );
			InventoryItem item = null;
			foreach( var a in player.NodeInventory )
			{
				if( a.Type == type && a.Tier == tier )
				{
					item = a;
					break;
				}
			}

			//InventoryItem item = (player.NodeInventory as List<InventoryItem>).Find( a => a.Type == type && a.Tier == tier );

			if ( item == null || item.Count == 0 )
			{
				Log.Warning("Failed to place item.");
				return;
			}

			item.Count--;
			(cl.Pawn as FestivePlayer).PlaceNodeServer( cl, item.Type, x, y, dir, item.Tier );

			NodePlaced( To.Single( cl ) );
		}

		[ServerCmd]
		static void RotateNodeServer( int networkId )
		{
			Client cl = ConsoleSystem.Caller;

			if ( cl.Pawn is not FestivePlayer player )
				return;

			Log.Info( "rotating node " + networkId );
			GridNode node = null;

			foreach ( var a in player.ClientSleigh.Grid.Nodes )
			{
				if ( a.NetworkIdent == networkId )
				{
					node = a;
					break;
				}
			}

			if ( node != null )
				node.Direction--;
			else
				Log.Warning( "Could not find node." );
		}

		public void RotateNode( GridNode node )
		{
			if ( node == null )
				return;

			RotateNodeServer( node.NetworkIdent );
		}

		[ClientRpc]
		public static void NodeRemoved()
		{
			if ( Local.Pawn is not FestivePlayer player )
				return;

			player.ClientSleigh.Grid.SelectedNode = null;
			UpdateHUD();
		}

		[ServerCmd]
		static void RemoveNodeServer( int networkId )
		{
			Client cl = ConsoleSystem.Caller;

			if ( cl.Pawn is not FestivePlayer player )
				return;

			Log.Info( "removing node " + networkId );
			GridNode node = null;

			foreach ( var a in player.ClientSleigh.Grid.Nodes )
			{
				if ( a.NetworkIdent == networkId )
				{
					node = a;
					break;
				}
			}

			if ( node != null )
			{
				foreach( var item in player.NodeInventory )
				{
					if(item.Type == node.Type && item.Tier == node.Tier)
					{
						item.Count++;
						break;
					}
				}

				player.ClientSleigh.Grid.DeleteNode( node );
			}
			else
				Log.Warning( "Could not find node." );

			NodeRemoved( To.Single( cl ) );
		}

		public void RemoveNode( GridNode node )
		{
			if ( node == null )
				return;

			RemoveNodeServer( node.NetworkIdent );
		}

		public override void BuildInput( InputBuilder input )
		{
			if( input.Pressed( InputButton.Score ) )
			{
				ToggleBuildMode();
			}

			if( BuildMode )
			{
				if( GridHovered && ActiveItem != null )
				{
					if ( input.Pressed( InputButton.Attack2 ) || input.Pressed( InputButton.Reload ) )
					{
						RotateBuildLeft();

						Log.Info( "rotation " + BuildRotation );
						//if ( ++BuildRotation > 3 )
						//	BuildRotation = 0;

					}

					if ( input.MouseWheel != 0 )
					{
						int delta = Math.Clamp( input.MouseWheel, -1, 1 );
						if ( delta > 0 ) RotateBuildLeft(); else RotateBuildRight();

						Log.Info( "rotation " + BuildRotation );
					}
				}



				if ( input.Pressed( InputButton.Attack1 ) )
				{
					GridNode hoveredNode = NodeTrace();

					if(hoveredNode != null)
					{
						ClientSleigh.Grid.SelectedNode = hoveredNode.Behavior != NodeBehavior.Consumer ? hoveredNode : null;
					}
					else
					{
						ClientSleigh.Grid.SelectedNode = null;

						if ( GridHovered && ActiveItem != null )
						{
							Log.Info( "place" );
							Place( ActiveItem.Type, ActiveItem.Tier, BuildTileX, BuildTileY, BuildRotation );
						}
					}

					UpdateHUD();
				}

				//if ( input.Pressed( InputButton.Attack1 ) )
				//{
				//	Log.Info( "place" );
				//	PlaceNode( "conveyorbelt", BuildTileX, BuildTileY, BuildRotation, 0 );
				//}

				//if ( input.Pressed( InputButton.Jump ) )
				//{
				//	Log.Info( "place" );
				//	PlaceNode( "boxer", BuildTileX, BuildTileY, BuildRotation, 0 );
				//}

				//if ( input.Pressed( InputButton.Use ) )
				//{
				//	Log.Info( "place" );
				//	PlaceNode( "factory", BuildTileX, BuildTileY, BuildRotation, 1 );
				//}

				//if ( input.Pressed( InputButton.Flashlight ) )
				//{
				//	PlaceItem( "toy", 0, 0 );
				//}
			}
			else
			{
				if ( input.Pressed( InputButton.Use ) )
				{
					if( TargetCannon == null )
					{
						float closestDist = 1000.0f;
						CannonNode closest = null;

						foreach ( var node in ClientSleigh.Grid.Nodes )
						{
							if ( node is not CannonNode cannon )
								continue;

							var dist = cannon.GetPlayerDistance();

							if ( dist < closestDist )
							{
								closestDist = dist;
								closest = cannon;
							}
						}

						if ( closest != null && closestDist < 100.0f )
						{
							Log.Info( "switch to cannon" );
							TargetCannon = closest;
						}
					}
					else
					{
						TargetCannon = null;
					}

					UpdateHUD();
				}
			}


			base.BuildInput( input );
		}
	}
}
