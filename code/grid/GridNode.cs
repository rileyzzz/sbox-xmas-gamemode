using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public enum NodeBehavior
	{
		Producer,
		Transformer,
		Consumer,
		Conveyor
	}

	public partial class GridNode : ModelEntity
	{
		public string Type;

		NodeBehavior Behavior;

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
			Behavior = GetBehavior( typeData.type );
		}

		float lerp( float v0, float v1, float t )
		{
			return (1 - t) * v0 + t * v1;
		}

		public void Tick()
		{
			if ( Parent is not GridEntity grid )
				return;

			List<GridItem> items = grid.GetItemsInTile( X, Y );

			const float moveSpeed = 0.1f;

			foreach( var item in items )
			{
				
				var direction = GetDirectionVector( Direction );
				var targetPos = item.Pos + direction * moveSpeed;
				if ( direction.x == 0.0f ) targetPos.x = lerp( targetPos.x, X + 0.5f, 0.5f );
				if ( direction.y == 0.0f ) targetPos.y = lerp( targetPos.y, Y + 0.5f, 0.5f );

				item.Pos = targetPos;

				//Log.Info( "moving item to " + targetPos.ToString() + " client " + IsClient );
			}
		}

		public static NodeBehavior GetBehavior(string behavior)
		{
			if ( behavior == "producer" ) return NodeBehavior.Producer;
			if ( behavior == "consumer" ) return NodeBehavior.Consumer;
			if ( behavior == "transformer" ) return NodeBehavior.Transformer;
			return NodeBehavior.Conveyor;
		}

		public static Vector2 GetDirectionVector(int dir)
		{
			if ( dir == 0 ) return Vector2.Left;
			if ( dir == 1 ) return Vector2.Up;
			if ( dir == 2 ) return Vector2.Right;
			if ( dir == 3 ) return Vector2.Down;

			return new Vector2(0.0f, 0.0f);
		}
	}
}
