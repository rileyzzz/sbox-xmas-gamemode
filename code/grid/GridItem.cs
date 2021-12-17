using Sandbox;
using Sandbox.UI.Construct;
using System;

namespace ChristmasGame
{

	public partial class GridItem : BaseNetworkable
	{
		[Net] public GridEntity Grid { get; set; }

		[Net] public string Type { get; set; }

		[Net] public Vector2 Pos { get; set; }
		//[Net] Vector2 _pos { get; set; }

		//public Vector2 Pos
		//{
		//	get => _pos;
		//	set
		//	{
		//		_pos = value;
		//		Log.Info("position set, model exists " + Model != null + " server " + Model.IsServer );
		//		if(Model != null && Model.Parent is GridEntity grid)
		//		{
		//			Log.Info("updating model position");
		//			Model.Position = grid.GridToLocal( new Vector3( _pos.x, _pos.y, 0.0f ) );
		//		}
		//	}
		//}

		[Net] public ModelEntity Model { get; set; }

		public GridItem()
		{
		}

		public void SetType(string type)
		{
			Assert.True( Grid.IsServer );

			Type = type;

			var typeData = ChristmasGame.Config.items[type];

			Model = Entity.Create<ModelEntity>();
			Model.Parent = Grid;
			Model.Transmit = TransmitType.Always;
			

			int targetModel = ChristmasGame.rand.Next( 0, typeData.models.Count );
			Model.SetModel( typeData.models[targetModel] );
		}

		public void Update()
		{
			if ( Model.Parent is not GridEntity grid )
				return;

			Model.Position = grid.GridToLocal( new Vector3( Pos.x, Pos.y, 0.0f ) );
		}
	}
}
