using Sandbox;

namespace ChristmasGame
{
	[Library( "ent_festive_house" )]
	//[Hammer.RenderFields]
	[Hammer.Model( Model = "models/house.vmdl", Archetypes = ModelArchetype.static_prop_model )]
	//[Hammer.EditorModel( "models/house.vmdl" )]
	public partial class FestiveHouse : ModelEntity
	{
		bool _stale = false;
		public bool Stale
		{
			get => _stale;
			set
			{
				_stale = value;
				RenderColor = Stale ? Color.Red : Color.White;
				GlowActive = Stale;
				GlowState = Stale ? GlowStates.On : GlowStates.Off;
			}
		}

		public FestiveHouse()
		{
			GlowColor = Color.Red;
			//GlowState = GlowStates.GlowStateOn;
		}

		public override void Spawn()
		{
			base.Spawn();

			SetupPhysicsFromModel( PhysicsMotionType.Static );
		}


		[Input]
		public void PresentHit( Entity activator = null )
		{
			if ( Stale )
				return;

			if ( activator is not PhysicsPresent present )
				return;

			Log.Info("present hit! " + IsServer + " " + Name);
			present.Delete();

			if ( Game.Current is not ChristmasGame game )
				return;

			game.PresentsDelivered++;
			Stale = true;
		}
	}
}
