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
		[Net] string _type { get; set; }

		public string Type
		{
			get => _type;
			set
			{
				_type = value;
				Update();
			}
		}

		[Net] int _tier { get; set; } = 0;

		public int Tier
		{
			get => _tier;
			set
			{
				_tier = value;
				Update();
			}
		}

		[Net] public NodeBehavior Behavior { get; set; }
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
				if ( _direction > 3 )
					_direction = 0;
				if ( _direction < 0 )
					_direction = 3;

				Rotation = new Angles( 0.0f, _direction * 90.0f, 0.0f ).ToRotation();
			}
		}

		float accum = 0.0f;

		const int maxItems = 10;

		public GridNode()
		{
			Tags.Add( "festive_node" );
		}

		void Update()
		{
			var typeData = ChristmasGame.Config.nodes[Type];
			Behavior = GetBehavior( typeData.type );

			inputs = typeData.inputs;
			outputs = typeData.outputs;

			if ( Tier < typeData.tiers.Count )
			{
				var tierData = typeData.tiers[Tier];
				rate = tierData.rate;
				SetModel( tierData.model );
				SetupPhysicsFromModel( PhysicsMotionType.Static );
				//CollisionGroup = CollisionGroup.Trigger;
				ClearCollisionLayers();
				AddCollisionLayer( CollisionLayer.Hitbox );
				//RemoveCollisionLayer( CollisionLayer.Player );
			}
		}

		//[ServerCmd]
		//public void Rotate()
		//{
		//	Assert.True( IsServer );
		//	Direction++;
		//}

		public void UpdateTypeTier()
		{

		}

		public void SetType(string type, int tier = 0)
		{
			Type = type;
			Tier = tier;
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

			const float moveSpeed = 1.0f;

			foreach( var item in items )
			{
				var direction = GetDirectionVector( Direction );

				var relativePosition = item.Pos - new Vector2( X + 0.5f, Y + 0.5f );

				//only move inputs to the center
				if ( !inputs.Contains( item.Type ) || Vector2.GetDot( relativePosition, direction ) < 0 )
				{
					var targetPos = item.Pos + direction * moveSpeed * Time.Delta;
					if ( direction.x == 0.0f ) targetPos.x = lerp( targetPos.x, X + 0.5f, 0.1f );
					if ( direction.y == 0.0f ) targetPos.y = lerp( targetPos.y, Y + 0.5f, 0.1f );

					item.Pos = targetPos;
				}

				if ( inputs.Contains( item.Type ) )
					pendingTransform.Add( item );

				//Log.Info( "moving item to " + targetPos.ToString() + " client " + IsClient );
			}

			//Log.Info("is client " + IsClient);

			if( IsServer )
			{
				if ( pendingTransform.Count > maxItems )
				{
					//Log.Info( "item count exceeded" );
					for ( int i = maxItems; i < pendingTransform.Count; i++ )
						grid.DeleteItem( pendingTransform[i] );
				}

				accum += Time.Delta;
				if ( accum > rate )
				{
					accum = 0.0f;
					switch ( Behavior )
					{
						case NodeBehavior.Transformer:
							if ( outputs.Count > 0 && pendingTransform.Count > 0 )
							{
								//Log.Info( "item transform" );
								pendingTransform[0].SetType( outputs[0] );
							}
							break;
						case NodeBehavior.Producer:
							if ( outputs.Count > 0 )
							{
								//Log.Info( "produce item" );
								grid.PlaceItem( outputs[0], X, Y );
							}
							break;
						case NodeBehavior.Consumer:
							if( pendingTransform.Count > 0 )
							{
								Consume( pendingTransform.Count );
								foreach ( var item in pendingTransform )
									grid.DeleteItem( item );
							}
							break;
					}
				}
			}
		}

		protected virtual void Consume( int count )
		{
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
