using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class FuelMeter : Panel
	{
		Label fuelLabel;
		Panel fuelBg;

		Panel fuelNeedle;

		float _fuel = 0.0f;
		float _fuelMax = 10.0f;

		public float Fuel
		{
			get => _fuel;
			set
			{
				_fuel = value;
				UpdateNeedle();
			}
		}

		public float FuelMax
		{
			get => _fuelMax;
			set
			{
				_fuelMax = value;
				UpdateNeedle();
			}
		}

		public FuelMeter()
		{
			StyleSheet.Load( "/ui/FuelMeter.scss" );

			fuelBg = AddChild<Panel>( "bg" );
			fuelLabel = AddChild<Label>( "text" );
			fuelLabel.Text = "Fuel";

			fuelNeedle = fuelBg.AddChild<Panel>( "needle" );
			//fuelNeedle.SetTexture("");
		}

		void UpdateNeedle()
		{
			//Log.Info( "fuel: " + Fuel / FuelMax * 100.0f + "%" );

			PanelTransform t = new PanelTransform();
			t.AddRotation(0.0f, 0.0f, Fuel / FuelMax * 360.0f);

			fuelNeedle.Style.Transform = t;
		}
	}
}
