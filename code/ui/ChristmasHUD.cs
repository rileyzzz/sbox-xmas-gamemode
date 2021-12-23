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


		public ChristmasHUD()
		{
			bar = AddChild<NodeBar>( "nodeBar" );
			context = AddChild<NodeContext>( "context" );
			meter = AddChild<PresentMeter>( "meterContainer" );

			//meter.Presents = 0;
			//meter.MaxPresents = 100;

		}

		public void Update()
		{
			if ( Local.Pawn is FestivePlayer player )
			{
				bar.Style.Display = player.BuildMode ? DisplayMode.Flex : DisplayMode.None;
				if(player.BuildMode) bar.Update();
				context.Node = player.ClientSleigh.Grid.SelectedNode;
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
