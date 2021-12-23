using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class NodeContext : Panel
	{
		ContextButton rotate;
		ContextButton upgrade;
		ContextButton remove;

		const float width = 120.0f;

		GridNode _node;
		public GridNode Node
		{
			get => _node;
			set
			{
				_node = value;
				UpdateNode();
				Style.Display = _node != null ? DisplayMode.Flex : DisplayMode.None;
			}
		}

		public NodeContext()
		{
			StyleSheet.Load( "/ui/NodeContext.scss" );

			Style.Display = DisplayMode.None;
			Style.Width = width;

			rotate = AddChild<ContextButton>( "button" );
			rotate.Clicked = Rotate;
			rotate.Icon.SetText( "refresh" );

			upgrade = AddChild<ContextButton>( "button" );
			upgrade.Clicked = Upgrade;
			upgrade.Icon.SetText( "upgrade" );

			remove = AddChild<ContextButton>( "button" );
			remove.Clicked = Remove;
			remove.Icon.SetText( "delete" );
		}

		public void UpdateNode()
		{
			if ( Node == null )
				return;

			bool canUpgrade = Node.Tier + 1 < ChristmasGame.Config.nodes[Node.Type].tiers.Count;
			upgrade.Style.Display = canUpgrade ? DisplayMode.Flex : DisplayMode.None;

			//Log.Info("position " + Node.Position + " local position " + Node.LocalPosition);
			Vector2 pos = Node.WorldSpaceBounds.Center.ToScreen();
			pos = Parent.ScreenPositionToPanelPosition( pos * Screen.Size ) * ScaleFromScreen;
			
			Log.Info( "position " + Node.WorldSpaceBounds.Center + " screen position " + pos );

			Style.Left = pos.x - width / 2.0f;
			Style.Top = pos.y - 80.0f;
		}

		void Rotate()
		{
			if ( Node == null || Local.Pawn is not FestivePlayer player )
				return;

			Log.Info( "rotate" );

			//player.ClientSleigh.Grid.RotateNode( Node );
			player.RotateNode( Node );
		}

		void Upgrade()
		{
			Log.Info( "upgrade" );
		}

		void Remove()
		{
			if ( Node == null || Local.Pawn is not FestivePlayer player )
				return;

			Log.Info( "remove" );

			player.RemoveNode( Node );
		}
	}

	public partial class ContextButton : Panel
	{
		public Action Clicked;
		public IconPanel Icon;

		public ContextButton()
		{
			Icon = AddChild<IconPanel>( "icon" );
		}

		protected override void OnClick( MousePanelEvent e )
		{
			base.OnClick( e );

			Clicked();
			e.StopPropagation();
		}

		protected override void OnMouseOver( MousePanelEvent e )
		{
			base.OnMouseOver( e );

			SetClass( "hover",  true );
			e.StopPropagation();
		}

		protected override void OnMouseOut( MousePanelEvent e )
		{
			base.OnMouseOut( e );

			SetClass( "hover", false );
			e.StopPropagation();
		}
	}
}
