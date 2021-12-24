using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class EngineNode : GridNode
	{
		public int FuelToBurn = 0;

		public EngineNode()
		{
		}

		protected override void Consume( int count )
		{
			base.Consume( count );

			FuelToBurn += count;

			//Log.Info( "engine input" );
		}
	}
}
