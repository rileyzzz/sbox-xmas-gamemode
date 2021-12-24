using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class Prompt : Panel
	{
		Label promptLabel;
		Panel buttonContainer;

		PromptButton yesButton;
		PromptButton noButton;

		public string Text { set => promptLabel.Text = value; }
		public Action Accept { set => yesButton.Click = value; }
		public Action Cancel { set => noButton.Click = value; }

		public Prompt()
		{
			StyleSheet.Load( "/ui/Prompt.scss" );

			promptLabel = AddChild<Label>( "text" );

			buttonContainer = AddChild<Panel>( "buttonContainer" );

			yesButton = buttonContainer.AddChild<PromptButton>( "button" );
			yesButton.Text.Text = "Yes";

			noButton = buttonContainer.AddChild<PromptButton>( "button" );
			noButton.Text.Text = "No";
		}
	}

	public partial class PromptButton : Panel
	{
		public Label Text;
		public Action Click = null;

		public PromptButton()
		{
			Text = AddChild<Label>( "text" );
		}

		protected override void OnMouseOver( MousePanelEvent e )
		{
			base.OnMouseOver( e );

			SetClass( "hover", true );
			e.StopPropagation();
		}

		protected override void OnMouseOut( MousePanelEvent e )
		{
			base.OnMouseOut( e );

			SetClass( "hover", false );
			e.StopPropagation();
		}

		protected override void OnClick( MousePanelEvent e )
		{
			base.OnClick( e );

			Click();
			e.StopPropagation();
		}
	}
}
