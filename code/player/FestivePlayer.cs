using Sandbox;
using Sandbox.UI.Construct;
using System;

namespace ChristmasGame
{
	public partial class FestivePlayer : Player
	{
		[Net] public Sleigh ClientSleigh { get; set; }

		bool BuildMode = false;

		int BuildRotation = 0;
		int BuildTileX = 0;
		int BuildTileY = 0;
		
		ModelEntity BuildHint;

		public FestivePlayer()
		{
		}

		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			//
			// Use WalkController for movement (you can make your own PlayerController for 100% control)
			//
			Controller = new WalkController();

			//
			// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
			//
			Animator = new StandardPlayerAnimator();

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

					Ray tr = new Ray( Input.Cursor.Origin, Input.Cursor.Direction );
					//DebugOverlay.Line( tr.Origin, tr.Origin + tr.Direction * 1000.0f, 0.1f );

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
					}

					Rotation targetRotation = Rotation.From(new Angles( 0.0f, BuildRotation * 90.0f, 0.0f ));
					BuildHint.Rotation = Rotation.Lerp( BuildHint.Rotation, targetRotation, 0.4f );
				}
			}

		}

		public override void OnKilled()
		{
			base.OnKilled();

			EnableDrawing = false;
		}

		public void ToggleBuildMode()
		{
			BuildMode = !BuildMode;

			if(BuildMode)
			{
				Log.Info("Entering build mode.");

				BuildHint = Create<ModelEntity>();
				BuildHint.SetModel( "models/props/cs_office/chair_office.vmdl" );
			}
			else
			{
				Log.Info("Exiting build mode.");

				BuildHint.Delete();
				BuildHint = null;
			}

			ClientSleigh.Grid.SetTileOverlayVisible( BuildMode );
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

		public void PlaceNodeServer( int x, int y, int dir )
		{
			if ( !IsServer )
				return;

			Log.Info( "placing node" );
			ClientSleigh.Grid.PlaceNode<GridNode>( x, y, dir );
		}
		
		[ServerCmd]
		static void PlaceNode(int x, int y, int dir)
		{
			Entity pawn = ConsoleSystem.Caller.Pawn;
			(pawn as FestivePlayer).PlaceNodeServer( x, y, dir );
		}

		public override void BuildInput( InputBuilder input )
		{
			if( input.Pressed( InputButton.Score ) )
			{
				ToggleBuildMode();
			}

			if( BuildMode )
			{
				if ( input.Pressed( InputButton.Attack2 ) || input.Pressed( InputButton.Reload ) )
				{
					RotateBuildLeft();

					Log.Info("rotation " + BuildRotation);
					//if ( ++BuildRotation > 3 )
					//	BuildRotation = 0;
					
				}

				if ( input.MouseWheel != 0 )
				{
					int delta = Math.Clamp(input.MouseWheel, -1, 1);
					if ( delta > 0 ) RotateBuildLeft(); else RotateBuildRight();

					Log.Info( "rotation " + BuildRotation );
				}

				if ( input.Pressed( InputButton.Attack1 ) )
				{
					Log.Info( "place" );
					PlaceNode( BuildTileX, BuildTileY, BuildRotation );
				}
			}


			base.BuildInput( input );
		}
	}
}
