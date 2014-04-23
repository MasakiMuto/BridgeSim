using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Masa.BridgeSim
{
	public class Human : DrawableGameComponent
	{
		BasicEffect effect;
		VertexBuffer vertex;
		IndexBuffer index;

		HumanNode Root;
		HumanNode Head;
		HumanNode LeftArm, RightArm;
		HumanNode LeftLeg, RightLeg;

		void CreateParts()
		{
			Root = new HumanNode()
			{
				Position = Vector3.Zero,
				Scale = new Vector3(1, 3, .5f),

			};
			Head = new HumanNode()
			{
				Position = new Vector3(0, 3, 0),
				Scale = new Vector3(.7f, 1, 0.5f)
			};
			LeftArm = new HumanNode()
			{
				Position = new Vector3(2, 1.2f, 0),
				Scale = new Vector3(2.5f, .3f, .3f)
			};
			LeftLeg = new HumanNode()
			{
				Position = new Vector3(.5f, 5, 0),
				Scale = new Vector3(.5f, 4f, .5f)
			};

			Root.AppendChild(Head);
			Root.AppendChild(LeftArm);
			Root.AppendChild(LeftLeg);
			RightArm = LeftArm.Mirror();
			RightLeg = LeftLeg.Mirror();

		}

		IEnumerable<HumanNode> EnumrateNode()
		{
			return new[]{
				Root,
				Head,
				LeftArm,
				LeftLeg,
				RightArm,
				RightLeg
			};
		}

		void InitRender()
		{
			effect = new BasicEffect(GraphicsDevice);
			vertex = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), 8, BufferUsage.WriteOnly);
			index = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 6 * 3 * 2, BufferUsage.WriteOnly);
			vertex.SetData(new[]{
				new Vector3(-1, -1, -1),
				new Vector3(-1, -1, 1),
				new Vector3(-1, 1, -1),
				new Vector3(-1, 1, 1),
				new Vector3(1, -1, -1),
				new Vector3(1, -1, 1),
				new Vector3(1, 1, -1),
				new Vector3(1, 1, 1)
			}.Select(x => new VertexPositionColor(x, Color.Green))
			.ToArray()
			);
			index.SetData<short>(new short[]{
				0, 2, 1,
				2, 3, 1,
				5, 6, 4,
				6, 7, 5,
				2, 6, 7,
				7, 3, 2,
				5, 4, 0,
				0, 1, 5,
				0, 4, 6,
				6, 2, 0,
				7, 1, 5,
				1, 3, 7
			});
			effect.TextureEnabled = false;
			//effect.EnableDefaultLighting();
			effect.View = Matrix.CreateLookAt(new Vector3(10, 10, 10), new Vector3(), Vector3.Up);
			effect.Projection = Matrix.CreatePerspectiveFieldOfView(.8f, GraphicsDevice.Viewport.AspectRatio, .1f, 100);
		}

		protected override void LoadContent()
		{
			base.LoadContent();
			InitRender();
		}

		public Human(Game game)
			: base(game)
		{
			CreateParts();
		}

		

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			GraphicsDevice.RasterizerState = RasterizerState.CullNone;
			GraphicsDevice.SetVertexBuffer(vertex);
			GraphicsDevice.Indices = index;
			foreach (var item in EnumrateNode())
			{
				effect.World = item.Transform;
				effect.CurrentTechnique.Passes[0].Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertex.VertexCount, 0, index.IndexCount / 3);
			}
			
			
		}
	}

	public class HumanNode
	{
		public Vector3 Position;//親からの相対座標
		public Vector3 Scale;

		public Matrix Transform
		{
			get
			{
				var trans = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
				if (Parent == null)
				{
					return trans;
				}
				else
				{
					return Parent.Transform * trans;
				}
			}
		}

		public HumanNode Parent;
		public List<HumanNode> Children;

		public HumanNode()
		{
			Children = new List<HumanNode>();
		}

		public HumanNode Mirror()
		{
			var mirror = new HumanNode()
			{
				Scale = this.Scale,
				Position = new Vector3(-this.Position.X, this.Position.Y, this.Position.Z)
			};
			this.Parent.AppendChild(mirror);
			return mirror;
		}

		public void AppendChild(HumanNode child)
		{
			Debug.Assert(child.Parent == null);
			Children.Add(child);
			child.Parent = this;
		}

	}
}
