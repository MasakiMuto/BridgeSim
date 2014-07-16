using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Masa.BridgeSim
{
	public class BoxRenderer : DrawableGameComponent
	{
		Effect effect;
		VertexBuffer vertex;
		IndexBuffer index;

		RasterizerState wire;
		Texture2D[] tex;

		public BoxRenderer(Game game)
			: base(game)
		{
			Visible = false;
		}

		protected override void LoadContent()
		{
			base.LoadContent();
			effect = new Effect(GraphicsDevice, System.IO.File.ReadAllBytes("effect.bin"));
			vertex = new VertexBuffer(GraphicsDevice, typeof(MyVertex), 8, BufferUsage.WriteOnly);
			index = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 6 * 3 * 2, BufferUsage.WriteOnly);
			vertex.SetData(new[]{
				new VertexPositionTexture(new Vector3(-1, -1, -1), new Vector2(0.25f, 0.25f)),
				new VertexPositionTexture(new Vector3(-1, -1,  1), new Vector2(0, 0)),
				new VertexPositionTexture(new Vector3(-1,  1, -1), new Vector2(0.25f, .75f)),
				new VertexPositionTexture(new Vector3(-1,  1,  1), new Vector2(0, 1)),
				new VertexPositionTexture(new Vector3( 1, -1, -1), new Vector2(.75f, .25f)),
				new VertexPositionTexture(new Vector3( 1, -1,  1), new Vector2(1, 0)),
				new VertexPositionTexture(new Vector3( 1,  1, -1), new Vector2(.75f, .75f)),
				new VertexPositionTexture(new Vector3( 1,  1,  1), new Vector2(1, 1)), 
			}.Select(x => new MyVertex(x.Position, x.Position, x.TextureCoordinate))
			.ToArray()
			);
			index.SetData<short>(new short[]{
				0, 2, 1,
				2, 3, 1,
				5, 6, 4,
				5, 7, 6,
				2, 6, 7,
				7, 3, 2,
				5, 4, 0,
				0, 1, 5,
				0, 4, 6,
				6, 2, 0,
				5, 1, 7,
				1, 3, 7
			});

			wire = new RasterizerState()
			{
				CullMode = CullMode.CullCounterClockwiseFace,
				FillMode = FillMode.WireFrame
			};
			tex = new[]{"edge2.png", "edge3.png"}.Select(x=> Texture2D.FromStream(GraphicsDevice, System.IO.File.OpenRead(x))).ToArray();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			var camera = Game.GetComponent<Camera>();
			effect.Parameters["Projection"].SetValue(camera.GetProjection(Game));
			effect.Parameters["View"].SetValue(camera.View);
			effect.Parameters["DiffuseDir"].SetValue(Vector3.Normalize(Vector3.One));
		}

		public void Begin()
		{
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			//GraphicsDevice.RasterizerState = wire;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			GraphicsDevice.SetVertexBuffer(vertex);
			GraphicsDevice.Indices = index;
		
		}

		public void DrawBox(Color diffuse, Matrix world, int textureIndex = 1)
		{
			diffuse = Color.White;
			GraphicsDevice.Textures[0] = tex[textureIndex];
			effect.Parameters["Diffuse"].SetValue(diffuse.ToVector3());
			effect.Parameters["World"].SetValue(world);
			effect.CurrentTechnique.Passes[0].Apply();
			GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertex.VertexCount, 0, index.IndexCount / 3);
			
		}

	}
}
