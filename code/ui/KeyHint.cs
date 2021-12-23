using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class KeyHint : Panel
	{
		Panel KeyPanel;
		Label KeyText;

		Label HintText;

		public KeyHint()
		{
			StyleSheet.Load( "/ui/KeyHint.scss" );

			KeyPanel = AddChild<Panel>( "key" );
			KeyText = KeyPanel.AddChild<Label>( "text" );

			HintText = AddChild<Label>( "hint" );
		}

		public void SetText(string keyText, string hintText, bool wide = false)
		{
			KeyText.Text = keyText;
			HintText.Text = hintText;

			KeyPanel.SetClass( "wide", wide );
		}
	}
}
