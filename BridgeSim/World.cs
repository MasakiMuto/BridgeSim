using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Masa.BridgeSim
{
	public class World : DrawableGameComponent
	{
		public World(Game game)
			: base(game)
		{

		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			var box = Game.GetComponent<BoxRenderer>();
			box.Begin();
			GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
			box.DrawBox(new Vector3(.5f, .5f, .5f), Matrix.CreateScale(10));
		}
	}
}
