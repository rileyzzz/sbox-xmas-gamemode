using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class PresentMeter : Panel
	{
		Panel meter;
		Panel bar;
		Label barText;

		Label text;

		int _presents = 0;
		int _maxPresents = 100;

		public int Presents
		{
			get => _presents;
			set
			{
				_presents = value;
				Update();
			}
		}

		public int MaxPresents
		{
			get => _maxPresents;
			set
			{
				_maxPresents = value;
				Update();
			}
		}

		public PresentMeter()
		{
			StyleSheet.Load( "/ui/PresentMeter.scss" );

			meter = AddChild<Panel>( "meter" );
			bar = meter.AddChild<Panel>( "bar" );
			barText = bar.AddChild<Label>( "text" );
			barText.Text = "0";

			text = AddChild<Label>( "text" );
			text.Text = "Present-O-Meter";
		}

		public void Update()
		{
			bar.Style.Height = Length.Percent( (float)Presents / MaxPresents * 100.0f );
			barText.Text = Presents.ToString();
		}
	}
}
