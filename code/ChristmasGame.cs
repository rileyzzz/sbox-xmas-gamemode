using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class ChristmasGame : Game
	{
		[Net] List<Sleigh> ActiveSleighs { get; set; } = new();

		public ChristmasGame()
		{
			if ( IsServer )
			{
				ActiveSleighs.Add( Create<Sleigh>() );

				//new MinimalHudEntity();
				new ChristmasHUDEntity();
			}

			if ( IsClient )
			{

			}
		}

		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new FestivePlayer();
			client.Pawn = player;
			player.ClientSleigh = ActiveSleighs[0];

			player.Respawn();

			//player.Parent = ActiveSleighs[0];
		}

		public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
		{
			base.ClientDisconnect( cl, reason );

		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if( IsServer )
			{
				foreach ( var sleigh in ActiveSleighs )
					sleigh.Simulate( cl );
			}

		}
	}
}
