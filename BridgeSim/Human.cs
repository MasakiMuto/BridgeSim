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
		Effect myeffect;
		VertexBuffer vertex;
		IndexBuffer index;

		HumanNode Root;
		HumanNode Head;
		HumanNode LeftArm, RightArm;
		HumanNode LeftLeg, RightLeg;

		void CreateParts()
		{
			Root = new HumanNode(new Vector3(0, 0, 1))
			{
				Position = Vector3.Zero,
				Scale = new Vector3(1, 3, .5f),
			};
			Head = new HumanNode(new Vector3(1, 0, 0))
			{
				Position = new Vector3(0, 4f, 0),
				Scale = new Vector3(.7f, 1f, 0.5f)
			};
			LeftArm = new HumanNode(new Vector3(0, 1, 0))
			{
				Position = new Vector3(3.5f, 1.2f, 0),
				Scale = new Vector3(2.5f, .3f, .3f)
			};
			LeftLeg = new HumanNode(new Vector3(1, 1, 0))
			{
				Position = new Vector3(.5f, -6f, 0),
				Scale = new Vector3(.3f, 3f, .5f)
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
			vertex = new VertexBuffer(GraphicsDevice, typeof(MyVertex), 8, BufferUsage.WriteOnly);
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
			}.Select(x => new MyVertex(x, x))
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
			myeffect = new Effect(GraphicsDevice, System.IO.File.ReadAllBytes("effect.bin"));
			myeffect.Parameters["View"].SetValue(Matrix.CreateLookAt(new Vector3(10, 10, 10), new Vector3(), Vector3.Up));
			myeffect.Parameters["Projection"].SetValue(Matrix.CreatePerspectiveFieldOfView(.8f, GraphicsDevice.Viewport.AspectRatio, .1f, 100));
			myeffect.Parameters["DiffuseDir"].SetValue(Vector3.Normalize(Vector3.One));
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
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			GraphicsDevice.SetVertexBuffer(vertex);
			GraphicsDevice.Indices = index;
			myeffect.Parameters["View"].SetValue(Game.Components.OfType<Camera>().Single().View);
			foreach (var item in EnumrateNode())
			{
				myeffect.Parameters["Diffuse"].SetValue(item.Diffuse);
				myeffect.Parameters["World"].SetValue(item.Transform);
				myeffect.CurrentTechnique.Passes[0].Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertex.VertexCount, 0, index.IndexCount / 3);
			}
		}
	}

	public class HumanNode
	{
		public Vector3 Position;//親からの相対座標
		public Vector3 Scale;
		public Vector3 Diffuse;

		public Matrix Transform
		{
			get
			{
				//var trans = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(Position);
				//if (Parent == null)
				//{
				//	return trans;
				//}
				//else
				//{
				//	return Matrix.CreateTranslation(Parent.Position) * trans;
				//}
				return Matrix.CreateScale(Scale) * AbsolutePosition;
			}
		}

		Matrix AbsolutePosition
		{
			get
			{
				var trans = Matrix.CreateTranslation(Position);
				if (Parent == null)
				{
					return trans;
				}
				else
				{
					return Parent.AbsolutePosition * trans;
				}
			}
		}

		public HumanNode Parent;
		public List<HumanNode> Children;

		public HumanNode(Vector3 diffuse)
		{
			Children = new List<HumanNode>();
			Diffuse = diffuse;
		}

		public HumanNode Mirror()
		{
			var mirror = new HumanNode(Diffuse)
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
