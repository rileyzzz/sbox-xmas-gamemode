using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;

namespace ChristmasGame
{
	public partial class PhysicsPresent : Prop
	{
		public PhysicsPresent()
		{
		}

		public override void Spawn()
		{
			base.Spawn();

			Tags.Add( "present" );
		}
	}
}
