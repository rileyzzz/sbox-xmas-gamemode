using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class CannonNode : GridNode
	{
		[Net] public int NumPresents { get; set; }
		public CannonNode()
		{

		}

		protected override void Consume( int count )
		{
			base.Consume( count );

			NumPresents += count;

			Log.Info( "cannon input" );
		}
	}
}
