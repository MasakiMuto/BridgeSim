using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Input;

namespace Masa.BridgeSim
{
	public enum Part
	{
		Head,
		Root,
		Kubi,
		Kata,
		Hiji,
		Tekubi,
		Tesaki,
		Mata,
		Hiza,
		Ashikubi,
		Tsumasaki
	}

	public enum Position
	{
		Center,
		Left,
		Right,
	}

	public struct PartId
	{
		public readonly Part Part;
		public readonly Position Position;

		public PartId(Part part, Position position)
		{
			Part = part;
			Position = position;
		}

		public static bool operator ==(PartId p1, PartId p2)
		{
			return p1.Part == p2.Part && p1.Position == p2.Position;
		}

		public static bool operator !=(PartId p1, PartId p2)
		{
			return !(p1 == p2);
		}

		public override string ToString()
		{
			return Position.ToString() + Part.ToString();
		}
	}

	public class JointHuman : DrawableGameComponent
	{
		public static JointHuman Human { get; private set; }

		Joint root;
		public Vector3 Translate { get; private set; }

		KeyFrameAnime anime;

		Dictionary<PartId, Joint> allJoint;

		public JointHuman(Game game)
			: base(game)
		{
			Human = this;
			CreateNodes();
			var all = root.AllChildren().ToArray();
			allJoint = all.ToDictionary(x=> new PartId(x.Name, x.Position), x => x);
			CreateAnime();
			ApplyAnime(0);
			SetBind();
			
		}


		void CreateAnime()
		{
			anime = new KeyFrameAnime();
			var pos = new PartId(Part.Hiji, Position.Left);
			anime.AddFrameWithMirror(new KeyFrameAnime.Frame(3, pos, new RotationState(0, 0, 0)));
			anime.AddFrameWithMirror(new KeyFrameAnime.Frame(5, pos, new RotationState(0, -MathHelper.PiOver2, 0)));
			anime.AddFrame(new KeyFrameAnime.Frame(6, new PartId(Part.Root, Position.Center), new RotationState(MathHelper.Pi, 0, MathHelper.TwoPi)));
			anime.Setup();

		}

		void ApplyAnime(double time)
		{
			var states = anime.Update(time).ToArray();
			foreach (var item in allJoint)
			{
				item.Value.ApplyState(states.Single(x => x.Part == item.Key).State);
			}
		}

		void CreateNodes()
		{
			root = new Joint(Position.Center, Part.Root, 1, new Vector2(2, 4), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0))
			{
				Color = Color.Red
			};
			var leftArm = new Joint(Position.Left, Part.Kata, new Vector3(1, 2, 0))//肩
			{
				Visible = false
			};
			leftArm.AddChild(
				new Joint(Position.Left, Part.Hiji, 2, new Vector2(.3f, .3f), Vector3.Zero, new ValueWithRange(-MathHelper.PiOver2, MathHelper.Pi), new ValueWithRange(-MathHelper.Pi, MathHelper.Pi), new ValueWithRange(-MathHelper.PiOver2, MathHelper.PiOver2)) { Color = Color.Pink }//ひじ
				.AddChild(
					new Joint(Position.Left, Part.Tekubi, 2f, new Vector2(.3f, .3f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0)) { Color = Color.Peru }//手首
					.AddChild(
						new Joint(Position.Left, Part.Tesaki, .7f, new Vector2(.5f, .3f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0)) { Color = Color.Green }//手先
					)
				)
			);
			root.AddChild(leftArm);
			root.AddChild(leftArm.Mirror());

			var leftLeg = new Joint(Position.Left, Part.Mata, new Vector3(.5f, -2, 0));//股関節
			leftLeg.AddChild(new Joint(Position.Left, Part.Hiza, 2.5f, new Vector2(1f, 1), Vector3.Zero, new ValueWithRange(-MathHelper.PiOver2, MathHelper.PiOver2), new ValueWithRange(-MathHelper.PiOver2, MathHelper.Pi), new ValueWithRange(-MathHelper.PiOver4, MathHelper.PiOver4)) { Color = Color.LightBlue }//膝
				.AddChild(
					new Joint(Position.Left, Part.Ashikubi, 2.5f, new Vector2(.8f, .8f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0, MathHelper.Pi), new ValueWithRange(-MathHelper.Pi / 8, MathHelper.Pi / 8)) { Color = Color.Blue }//足首
					.AddChild(
						new Joint(Position.Left, Part.Tsumasaki, 1f, new Vector2(1f, .3f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(-MathHelper.PiOver2 * 1.5f, -MathHelper.PiOver2 * .5f), new ValueWithRange(0, 0)) { Color = Color.BlueViolet }//足先
					)
				)
			);
			root.AddChild(leftLeg);
			root.AddChild(leftLeg.Mirror());

			root.AddChild(
				new Joint(Position.Center, Part.Kubi, new Vector3(0, 2, 0))
				.AddChild(
					new Joint(Position.Center, Part.Head, 1, new Vector2(.5f, .5f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(-MathHelper.PiOver2 * 3, 0), new ValueWithRange(0)) { Color = Color.Purple }));	
		}

		void SetBind()
		{
			var left = GetPart(Position.Left, Part.Tsumasaki);
			var right = GetPart(Position.Right, Part.Tsumasaki);
			Translate = new Vector3(0, -left.GetAbsolutePosition().Y, 0);
		}

		Joint GetPart(Position pos, Part part)
		{
			return allJoint[new PartId(part, pos)];
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			var state = Keyboard.GetState();
			if (state.IsKeyDown(Keys.Escape))
			{
				anime.Reset();
			}
			var delta = gameTime.ElapsedGameTime.TotalSeconds;
			if (state.IsKeyDown(Keys.Space))
			{
				delta = 0;
			}
			ApplyAnime(delta);
			//var part = GetPart(Position.Left, Part.Kata);
			//part.Yaw.Value = (float)(gameTime.TotalGameTime.TotalSeconds * 1.5f);
		}

		public void SaveToXml(string name)
		{
			var doc = new XDocument();
			doc.Add(new XElement("human"));
			doc.Root.Add(root.ToXml());
			doc.Save(name);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			var box = Game.GetComponent<BoxRenderer>();
			box.Begin();
			root.Draw(box);
		}
	}
}
