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
			box.DrawBox(Color.Gray, Matrix.CreateScale(10));
		}
	}
}
