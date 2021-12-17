using Sandbox;
using Sandbox.UI.Construct;
using System;

namespace ChristmasGame
{

	public partial class GridItem : BaseNetworkable
	{
		[Net] public string Type { get; set; }
		[Net, Predicted] public Vector2 Pos { get; set; }

		public ModelEntity ClientModel;

		public GridItem()
		{
		}

		public void UpdateModel()
		{
			ClientModel.Position = new Vector3(Pos.x, Pos.y, 0.0f);
		}
	}
}
