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

		

		protected override void LoadContent()
		{
			base.LoadContent();
		}

		public Human(Game game)
			: base(game)
		{
			CreateParts();
		}

		
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			var box = Game.GetComponent<BoxRenderer>();
			box.Begin();
			foreach (var item in EnumrateNode())
			{
				box.DrawBox(item.Diffuse, item.Transform);
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
