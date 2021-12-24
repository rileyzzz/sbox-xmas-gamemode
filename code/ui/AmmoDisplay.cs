using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class AmmoDisplay : Panel
	{
		public AmmoEntry Presents;
		public AmmoEntry Coal;

		public AmmoDisplay()
		{
			StyleSheet.Load( "/ui/AmmoDisplay.scss" );

			Presents = AddChild<AmmoEntry>( "entry" );
			Presents.Icon.Text = "redeem";

			//Coal = AddChild<AmmoEntry>( "entry" );
			//Coal.Icon.Text = "fireplace";
		}
	}

	public partial class AmmoEntry : Panel
	{
		public IconPanel Icon;
		public Label Text;

		public AmmoEntry()
		{
			Icon = AddChild<IconPanel>( "icon" );
			Text = AddChild<Label>( "text" );

			Text.Text = "0";

			//Icon.Text = 
		}
	}
}
