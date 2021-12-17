using Sandbox;
using Sandbox.UI.Construct;
using System;

namespace ChristmasGame
{
	public partial class GridNode : ModelEntity
	{
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
	}

	public partial class ProducerNode : GridNode
	{

	}

	public partial class ConsumerNode : GridNode
	{

	}

	public partial class TransformerNode : GridNode
	{

	}
}
