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
		VertexBuffer vertex;
		IndexBuffer index;
		BasicEffect effect;

		public World(Game game)
			: base(game)
		{

		}

		protected override void LoadContent()
		{
			base.LoadContent();
			vertex = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 4, BufferUsage.WriteOnly);
			vertex.SetData(new[]
				{
					new Vector2(0, 0),
					new Vector2(0, 1),
					new Vector2(1, 0),
					new Vector2(1, 1)
				}
				.Select(x => new VertexPositionColor(new Vector3(x.X * 2 - 1, 0, x.Y * 2 - 1), Color.Gray)).ToArray()
			);

			index = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
			index.SetData(new short[] { 0, 2, 1, 2, 3, 1 });
			effect = new BasicEffect(GraphicsDevice)
			{
				VertexColorEnabled = true,
				LightingEnabled = false,
				TextureEnabled = false,
				World = Matrix.CreateScale(10)
			};
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			var camera = Game.GetComponent<Camera>();
			effect.Projection = camera.GetProjection(Game);
			effect.View = camera.View;
			GraphicsDevice.SetVertexBuffer(vertex);
			GraphicsDevice.Indices = index;
			effect.CurrentTechnique.Passes[0].Apply();
			GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
		}
	}
}
