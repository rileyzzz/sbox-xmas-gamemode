using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public struct GridConfig
	{
		public Dictionary<string, ItemType> items { get; set; }
		public Dictionary<string, NodeType> nodes { get; set; }
	}

	public struct ItemType
	{
		public string label { get; set; }
		public List<string> models { get; set; }
	}

	public struct NodeType
	{
		public string label { get; set; }
		//public string model { get; set; }
		public string type { get; set; }
		public List<string> inputs { get; set; }
		public List<string> outputs { get; set; }
		public List<NodeTier> tiers { get; set; }
	}

	public struct NodeTier
	{
		public float rate { get; set; }
		public string icon { get; set; }
		public string model { get; set; }
	}

	public partial class ChristmasGame : Game
	{
		public static GridConfig Config;
		public static Random rand;


		[Net] List<Sleigh> ActiveSleighs { get; set; } = new();

		public ChristmasGame()
		{
			rand = new Random();

			//GridConfig test = new GridConfig();
			//test.items["test"] = new ItemType() { label = "test item" };

			//NodeType testnode = new NodeType() { label = "test node" };
			////testnode.inputs.Add( "a" );
			//testnode.outputs.Add( "b" );
			//testnode.tiers.Add( new NodeTier() { rate = 1.0f } );
			//test.nodes["cool"] = testnode;

			//FileSystem.Data.WriteJson( "testgrid.json", test );
			//Config = FileSystem.Mounted.ReadJson<GridConfig>( "testgrid.json" );

			Config = FileSystem.Mounted.ReadJson<GridConfig>( "grid.json" );

			Log.Info("Config loaded.");
			Log.Info(Config.items.Count + " item types, " + Config.nodes.Count + " node types.");

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
