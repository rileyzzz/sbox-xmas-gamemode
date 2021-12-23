using Sandbox;
using Sandbox.UI;

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

		Panel hintContainer;

		KeyHint buildHint;
		KeyHint placeHint;
		KeyHint rotateHint;

		public ChristmasHUD()
		{
			bar = AddChild<NodeBar>( "nodeBar" );
			context = AddChild<NodeContext>( "context" );
			meter = AddChild<PresentMeter>( "meterContainer" );

			hintContainer = AddChild<Panel>( "hintContainer" );


			buildHint = hintContainer.AddChild<KeyHint>( "keyHint" );

			placeHint = hintContainer.AddChild<KeyHint>( "keyHint" );
			placeHint.SetText("LMB", "Place", true);

			rotateHint = hintContainer.AddChild<KeyHint>( "keyHint" );
			rotateHint.SetText("RMB", "Rotate", true);

			Update();

			//meter.Presents = 0;
			//meter.MaxPresents = 100;

		}

		public void Update()
		{
			bool showNodeBar = false;

			if ( Local.Pawn is FestivePlayer player )
			{
				showNodeBar = player.BuildMode;

				bar.Style.Display = showNodeBar ? DisplayMode.Flex : DisplayMode.None;
				if( showNodeBar ) bar.Update();
				context.Node = player.ClientSleigh.Grid.SelectedNode;
			}

			buildHint.SetText( "Tab", showNodeBar ? "Exit Build Mode" : "Build Mode", true );

			placeHint.Style.Display = showNodeBar ? DisplayMode.Flex : DisplayMode.None;
			rotateHint.Style.Display = showNodeBar ? DisplayMode.Flex : DisplayMode.None;

			hintContainer.Style.Left = Length.Pixels( 10 );
			hintContainer.Style.Bottom = showNodeBar ? Length.Pixels( 150 ) : Length.Pixels( 10 );
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
