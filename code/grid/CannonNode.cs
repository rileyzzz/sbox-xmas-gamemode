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

		public override void ClientSpawn()
		{
			base.ClientSpawn();

			//Log.Info( "spawn isclient " + IsClient );
			//if( IsClient )
			//{
			//	foreach ( var child in Local.Hud.Children )
			//		(child as ChristmasHUD).CreateFireHint( this );
			//}

		}
	}
}
