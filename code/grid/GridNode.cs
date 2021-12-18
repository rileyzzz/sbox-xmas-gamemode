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
		List<string> inputs;
		List<string> outputs;
		float rate = 1.0f;

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

		float accum = 0.0f;

		const int maxItems = 10;


		public GridNode()
		{
		}

		public void Update()
		{

		}

		public void SetType(string type, int tier = 1)
		{
			var typeData = ChristmasGame.Config.nodes[type];

			Type = type;

			SetModel( typeData.model );
			Behavior = GetBehavior( typeData.type );
			inputs = typeData.inputs;
			outputs = typeData.outputs;

			if(tier < typeData.tiers.Count)
			{
				var tierData = typeData.tiers[tier];
				rate = tierData.rate;
			}
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
			List<GridItem> pendingTransform = new();

			const float moveSpeed = 0.05f;

			foreach( var item in items )
			{
				var direction = GetDirectionVector( Direction );

				var relativePosition = item.Pos - new Vector2( X + 0.5f, Y + 0.5f );

				//only move inputs to the center
				if ( !inputs.Contains( item.Type ) || Vector2.GetDot( relativePosition, direction ) < 0 )
				{
					var targetPos = item.Pos + direction * moveSpeed;
					if ( direction.x == 0.0f ) targetPos.x = lerp( targetPos.x, X + 0.5f, 0.3f );
					if ( direction.y == 0.0f ) targetPos.y = lerp( targetPos.y, Y + 0.5f, 0.3f );

					item.Pos = targetPos;
				}

				if ( inputs.Contains( item.Type ) )
					pendingTransform.Add( item );

				//Log.Info( "moving item to " + targetPos.ToString() + " client " + IsClient );
			}

			if(pendingTransform.Count > maxItems)
			{
				Log.Info("item count exceeded");
				for ( int i = maxItems; i < pendingTransform.Count; i++ )
					grid.DeleteItem( pendingTransform[i] );
			}

			accum += Time.Delta;
			if( accum > rate )
			{
				switch( Behavior )
				{
				case NodeBehavior.Transformer:
					if( outputs.Count > 0 && pendingTransform.Count > 0 )
					{
						accum = 0.0f;
						Log.Info("item transform");
						pendingTransform[0].SetType( outputs[0] );
					}
					break;
				case NodeBehavior.Producer:
					if( outputs.Count > 0 )
					{
						accum = 0.0f;
						Log.Info("produce item");
						grid.PlaceItem( outputs[0], X, Y );
					}
					break;
				}
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
