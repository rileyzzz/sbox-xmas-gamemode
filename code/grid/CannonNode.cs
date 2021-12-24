using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class CannonNode : GridNode
	{
		[Net, Change] public int NumPresents { get; set; }
		public CannonNode()
		{
		}

		public void OnNumPresentsChanged( int oldValue, int newValue )
		{
			//Log.Info("cannon onchange");
			if ( Local.Pawn is not FestivePlayer player )
				return;

			if ( player.TargetCannon != this )
				return;

			foreach ( var child in Local.Hud.Children )
				(child as ChristmasHUD).Update();
		}

		protected override void Consume( int count )
		{
			base.Consume( count );

			NumPresents += count;

			//Log.Info( "cannon input" );
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

		public float GetPlayerDistance()
		{
			return Vector3.DistanceBetween( Position, Local.Pawn.Position );
		}
	}
}
