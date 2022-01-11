using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ChristmasGame
{
	public partial class NodeBar : Panel
	{
		List<ItemButton> ItemButtons = new();

		public NodeBar()
		{
			StyleSheet.Load( "/ui/NodeBar.scss" );

			//Update();
			//if ( Local.Pawn is FestivePlayer player )
			//	(player.NodeInventory as ObservableCollection<InventoryItem>).CollectionChanged += UpdateList;

			Style.Display = DisplayMode.None;

			Update();
		}

		//void UpdateList( object sender, NotifyCollectionChangedEventArgs args )
		//{
		//	Update();
		//}

		public void Update()
		{
			DeleteChildren();
			ItemButtons.Clear();

			if ( Local.Pawn is not FestivePlayer player )
				return;

			List<InventoryItem> availableItems = new();
			List<InventoryItem> purchaseItems = new();

			foreach( var item in player.NodeInventory )
			{
				if ( item.Count > 0 )
					availableItems.Add( item );
				else if ( item.Tier == 0 )
					purchaseItems.Add( item );
			}

			foreach ( var item in availableItems )
				AddItemButton( item );
			foreach ( var item in purchaseItems )
				AddItemButton( item );

			UpdateActive();
		}

		public void UpdateActive()
		{
			if ( Local.Pawn is not FestivePlayer player )
				return;

			foreach ( var button in ItemButtons )
				button.SetClass( "active", button.Item == player.ActiveItem );
		}

		public void AddItemButton( InventoryItem item )
		{
			var button = AddChild<ItemButton>( "itemButton" );
			button.Item = item;
			ItemButtons.Add( button );
		}
	}

	public partial class ItemButton : Panel
	{
		Image Icon;
		Label OverlayText;

		InventoryItem _item;

		public InventoryItem Item
		{
			get => _item;
			set
			{
				_item = value;
				//Icon.SetTexture( ChristmasGame.Config.nodes[_item.Type].tiers[_item.Tier].icon );

				var typeData = ChristmasGame.Config.nodes[_item.Type].tiers[_item.Tier];

				Icon.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, typeData.icon );

				if ( Item.Count == 0 )
				{
					Icon.Style.BackgroundTint = new Color( 0.2f, 0.2f, 0.2f );
					OverlayText.SetText( "Buy (" + typeData.cost.ToString() + ")" );
					SetClass( "disabled", true );
				}
			}
		}
		public ItemButton()
		{
			Icon = AddChild<Image>( "image" );
			OverlayText = AddChild<Label>( "label" );
		}

		protected override void OnMouseOver( MousePanelEvent e )
		{
			base.OnMouseOver( e );

			SetClass( "hover", true );
		}

		protected override void OnMouseOut( MousePanelEvent e )
		{
			base.OnMouseOut( e );

			SetClass( "hover", false );
		}

		protected override void OnClick( MousePanelEvent e )
		{
			base.OnClick( e );

			Log.Info( "item click" );
			if ( Local.Pawn is not FestivePlayer player )
				return;

			if ( player.ActiveItem == Item )
				player.ActiveItem = null;
			else if ( Item.Count > 0 )
				player.ActiveItem = Item;
			else
				TryPurchase();

			(Parent as NodeBar).UpdateActive();

			e.StopPropagation();
		}

		void TryPurchase()
		{
			(Parent.Parent as ChristmasHUD).OpenBuyPrompt( Item );
		}

		//protected override void OnMouseDown( MousePanelEvent e )
		//{
		//	base.OnMouseDown( e );

		//	Log.Info("item click");
		//	if ( Local.Pawn is not FestivePlayer player )
		//		return;

		//	if(Item.Count > 0)
		//	{
		//		player.ActiveItem = Item;
		//		(Parent as NodeBar).UpdateActive();
		//	}

		//	e.StopPropagation();
		//}
	}
}
