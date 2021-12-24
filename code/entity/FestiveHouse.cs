using Sandbox;

namespace ChristmasGame
{
	[Library( "ent_festive_house" )]
	//[Hammer.RenderFields]
	[Hammer.Model( Model = "models/house.vmdl", Archetypes = ModelArchetype.static_prop_model )]
	//[Hammer.EditorModel( "models/house.vmdl" )]
	public partial class FestiveHouse : ModelEntity
	{
		public FestiveHouse()
		{
		}

		public override void Spawn()
		{
			base.Spawn();

			SetupPhysicsFromModel( PhysicsMotionType.Static );
		}

		[Input]
		public void PresentHit( Entity activator = null )
		{
			if ( activator is not PhysicsPresent present )
				return;

			Log.Info("present hit! " + IsServer + " " + Name);
			present.Delete();

			if ( Game.Current is not ChristmasGame game )
				return;

			game.PresentsDelivered++;
		}
	}
}
