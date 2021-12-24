using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class ChristmasHUDEntity : Sandbox.HudEntity<RootPanel>
	{
		ChristmasHUD HUD;
		public ChristmasHUDEntity()
		{
			if ( !IsClient )
				return;

			RootPanel.StyleSheet.Load( "/ui/ChristmasHUD.scss" );

			HUD = RootPanel.AddChild<ChristmasHUD>( "HUD" );
		}
	}

	public partial class ChristmasHUD : Panel
	{
		NodeBar bar;
		NodeContext context;
		PresentMeter meter;
		AmmoDisplay ammo;
		public FuelMeter FuelMeter;

		Panel hintContainer;

		KeyHint buildHint;
		KeyHint placeHint;
		KeyHint rotateHint;

		KeyHint launchHint;
		KeyHint exitCannonHint;

		Panel upgradePromptContainer;
		Prompt upgradePrompt;


		InventoryItem pendingBuy = null;
		GridNode pendingUpgrade = null;


		Dictionary<CannonNode, KeyHint> FireHints = new();

		string GetInputButtonName(string input)
		{
			string name = Input.GetKeyWithBinding( input ).ToUpper();
			if ( name == "MOUSE1" ) return "LMB";
			if ( name == "MOUSE2" ) return "RMB";
			return name;
		}
		public ChristmasHUD()
		{
			bar = AddChild<NodeBar>( "nodeBar" );
			context = AddChild<NodeContext>( "context" );
			meter = AddChild<PresentMeter>( "meterContainer" );
			ammo = AddChild<AmmoDisplay>( "ammoDisplay" );
			FuelMeter = AddChild<FuelMeter>( "fuelMeter" );

			ammo.Style.Left = Length.Pixels( 10 );
			ammo.Style.Top = Length.Pixels( 10 );
			ammo.Style.Display = DisplayMode.None;

			FuelMeter.Style.Top = Length.Pixels( 10 );
			FuelMeter.Style.Right = Length.Pixels( 120 );

			hintContainer = AddChild<Panel>( "hintContainer" );


			buildHint = hintContainer.AddChild<KeyHint>( "keyHint" );

			placeHint = hintContainer.AddChild<KeyHint>( "keyHint" );
			//placeHint.SetText("LMB", "Place", true);
			placeHint.SetText( GetInputButtonName( "iv_attack" ), "Place", true );

			rotateHint = hintContainer.AddChild<KeyHint>( "keyHint" );
			//rotateHint.SetText("RMB", "Rotate", true);
			rotateHint.SetText( GetInputButtonName( "iv_attack2" ), "Rotate", true );

			launchHint = hintContainer.AddChild<KeyHint>( "keyHint" );
			launchHint.SetText( GetInputButtonName( "iv_attack" ), "Fire Present", true );
			launchHint.Style.Display = DisplayMode.None;

			exitCannonHint = hintContainer.AddChild<KeyHint>( "keyHint" );
			exitCannonHint.SetText( GetInputButtonName( "iv_use" ), "Exit Cannon" );
			exitCannonHint.Style.Display = DisplayMode.None;

			upgradePromptContainer = AddChild<Panel>( "promptContainer" );
			upgradePrompt = upgradePromptContainer.AddChild<Prompt>( "prompt" );
			upgradePrompt.Accept = PromptAccept;
			upgradePrompt.Cancel = PromptCancel;
			upgradePrompt.Style.Display = DisplayMode.None;

			Update();

			//meter.Presents = 0;
			//meter.MaxPresents = 100;

		}

		void ClosePrompt()
		{
			upgradePrompt.Style.Display = DisplayMode.None;

			pendingBuy = null;
			pendingUpgrade = null;
		}

		public void OpenBuyPrompt( InventoryItem item )
		{
			Log.Info( "open buy prompt" );
			pendingBuy = item;
			pendingUpgrade = null;

			var typeData = ChristmasGame.Config.nodes[item.Type];
			var tierData = typeData.tiers[0];

			upgradePrompt.Text = "Buy 1 '" + typeData.label + "' (" + tierData.cost.ToString() + " Presents)?";
			upgradePrompt.Style.Display = DisplayMode.Flex;
		}

		public void OpenUpgradePrompt( GridNode node )
		{
			Log.Info( "open upgrade prompt" );
			pendingBuy = null;
			pendingUpgrade = node;

			var typeData = ChristmasGame.Config.nodes[node.Type];
			var tierData = typeData.tiers[node.Tier + 1];

			upgradePrompt.Text = "Upgrade '" + typeData.label + "' to tier " + (node.Tier + 1).ToString() + " (" + tierData.cost + " Presents)?";
			upgradePrompt.Style.Display = DisplayMode.Flex;
		}

		void PromptAccept()
		{
			Log.Info( "prompt accept" );

			if( pendingBuy != null)
				FestivePlayer.PurchaseNodeServer( pendingBuy.Type );
			else if (pendingUpgrade != null)
				FestivePlayer.UpgradeNodeServer( pendingUpgrade.NetworkIdent );

			ClosePrompt();
		}

		void PromptCancel()
		{
			ClosePrompt();
		}

		public void CreateFireHints()
		{
			foreach ( var hint in FireHints )
				hint.Value.Delete();

			FireHints.Clear();

			if ( Local.Pawn is not FestivePlayer player )
				return;

			var grid = player.ClientSleigh.Grid;

			foreach ( var node in grid.Nodes )
			{
				//if ( node.Type != "cannon" )
				//	continue;
				if ( node is not CannonNode cannon )
					continue;

				CreateFireHint( cannon );
			}
		}

		void CreateFireHint(CannonNode cannon)
		{
			Log.Info( "creating fire hint" );

			var hint = AddChild<KeyHint>( "keyHint" );
			hint.Style.Position = PositionMode.Absolute;

			SetHintPosition(hint, cannon);

			hint.SetText( GetInputButtonName( "iv_use" ), "Fire" );

			FireHints[cannon] = hint;
		}

		void SetHintPosition(Panel hint, CannonNode cannon)
		{
			Vector2 pos = cannon.WorldSpaceBounds.Center.ToScreen();
			pos = Parent.ScreenPositionToPanelPosition( pos * Screen.Size ) * ScaleFromScreen;

			hint.Style.Left = pos.x - 40.0f;
			hint.Style.Top = pos.y - 80.0f;

			hint.Style.Display = DisplayMode.Flex;
		}

		public void Update()
		{
			bool showNodeBar = false;

			if( Game.Current is ChristmasGame game )
			{
				meter.Presents = game.PresentsDelivered;
				meter.MaxPresents = game.MaxPresents;
			}

			if ( Local.Pawn is FestivePlayer player )
			{
				showNodeBar = player.BuildMode && player.TargetCannon == null;

				bar.Style.Display = showNodeBar ? DisplayMode.Flex : DisplayMode.None;
				if( showNodeBar ) bar.Update();
				context.Node = player.ClientSleigh.Grid.SelectedNode;

				if( player.TargetCannon == null )
				{
					buildHint.Style.Display = DisplayMode.Flex;
					launchHint.Style.Display = DisplayMode.None;
					exitCannonHint.Style.Display = DisplayMode.None;
					ammo.Style.Display = DisplayMode.None;
				}
				else
				{
					buildHint.Style.Display = DisplayMode.None;
					launchHint.Style.Display = DisplayMode.Flex;
					exitCannonHint.Style.Display = DisplayMode.Flex;
					ammo.Style.Display = DisplayMode.Flex;

					ammo.Presents.Text.Text = player.TargetCannon.NumPresents.ToString();
				}
			}


			//buildHint.SetText( "Tab", showNodeBar ? "Exit Build Mode" : "Build Mode", true );
			buildHint.SetText( GetInputButtonName( "iv_score" ), showNodeBar ? "Exit Build Mode" : "Build Mode", true );

			placeHint.Style.Display = showNodeBar ? DisplayMode.Flex : DisplayMode.None;
			rotateHint.Style.Display = showNodeBar ? DisplayMode.Flex : DisplayMode.None;

			hintContainer.Style.Left = Length.Pixels( 10 );
			hintContainer.Style.Bottom = showNodeBar ? Length.Pixels( 150 ) : Length.Pixels( 10 );

			if(FireHints.Count == 0)
				CreateFireHints();
		}

		public override void Tick()
		{
			base.Tick();
			if ( Local.Pawn is not FestivePlayer player )
				return;

			foreach (var entry in FireHints)
			{
				var cannon = entry.Key;
				var hint = entry.Value;

				SetHintPosition( hint, cannon );

				const float hintDist = 75.0f;
				const float radiusOffset = 25.0f;

				var dist = cannon.GetPlayerDistance() - radiusOffset;
				hint.Style.Opacity = player.BuildMode ? 0.0f : Math.Clamp( (hintDist - dist) / hintDist, 0.0f, 1.0f );
			}
		}

		//float test = 0.0f;
		//bool parity = false;
		//public override void Tick()
		//{
		//	base.Tick();

		//	test += Time.Delta;

		//	if ( test > 2.0f )
		//	{
		//		Log.Info( "alternate" );
		//		parity = !parity;
		//		test = 0.0f;

		//		meter.Presents = parity ? 80 : 10;
		//	}
		//}
	}
}
