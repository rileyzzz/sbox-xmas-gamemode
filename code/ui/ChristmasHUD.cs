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

		public ChristmasHUD()
		{
			bar = AddChild<NodeBar>( "nodeBar" );
			context = AddChild<NodeContext>( "context" );
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
	}
}
