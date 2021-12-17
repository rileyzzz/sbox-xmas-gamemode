using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class GridNode : ModelEntity
	{
		public string Type;

		public int X = 0;
		public int Y = 0;

		int _direction = 0;
		public int Direction
		{
			get => _direction;
			set
			{
				_direction = value;
				Rotation = new Angles( 0.0f, _direction * 90.0f, 0.0f ).ToRotation();
			}
		}


		public GridNode()
		{
		}

		public void Update()
		{

		}

		public void SetType(string type)
		{
			var typeData = ChristmasGame.Config.nodes[type];

			Type = type;

			SetModel( typeData.model );
		}

		public void Tick()
		{
			if ( Parent is not GridEntity grid )
				return;

			List<GridItem> items = grid.GetItemsInTile( X, Y );

			const float moveSpeed = 1.0f;

			foreach(var item in items)
			{

			}
		}
	}
}
