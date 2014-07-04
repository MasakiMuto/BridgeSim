using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Masa.BridgeSim
{
	public enum Part
	{
		Head,
		Root,
		//Kubi,
		Spine1,
		Spine2,
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
			//anime.AddFrameWithMirror(new KeyFrameAnime.Frame(3, pos, new RotationState(0, 0, 0)));
			//anime.AddFrameWithMirror(new KeyFrameAnime.Frame(5, pos, new RotationState(0, 0, -MathHelper.PiOver2)));
			//anime.AddFrame(new KeyFrameAnime.Frame(6, new PartId(Part.Root, Position.Center), new RotationState(0, -MathHelper.PiOver2, 0)));
			SetBodyLine();
			Ojigi(1);
			SetBridge(5);
			anime.Setup();

		}

		void SetBridge(float time)
		{
			var a = MathHelper.PiOver2 * 1.5f;
			
			Bridge(time, 0);
			Bridge(time + 2, a);
			Bridge(time + 3, a);
			Bridge(time + 4, 0);
			Bridge(time + 5, 0);
			
		}

		void Ojigi(double time)
		{
			var hiji = new PartId(Part.Tekubi, Position.Left);
			var kata = new PartId(Part.Hiji, Position.Left);
			var kubi = new PartId(Part.Spine1, Position.Center);
			var koshi = new PartId(Part.Spine2, Position.Center);
			anime.SetAsZero(hiji, time, true);
			anime.SetAsZero(kata, time, true);
			anime.SetAsZero(kubi, time, false);
			anime.SetAsZero(koshi, time, false);
			var mid = time + 1;
			var end = time + 3;
			var length = 1;
			anime.AddFrameWithMirror(new KeyFrameAnime.Frame(mid, kata, new RotationState(0, -MathHelper.PiOver2 / 2, 0)).ShiftTime(length));
			anime.AddFrameWithMirror(new KeyFrameAnime.Frame(mid, hiji, new RotationState(-MathHelper.Pi * .15f, -MathHelper.PiOver2 * 1.2f, 0)).ShiftTime(length));
			anime.AddFrame(new KeyFrameAnime.Frame(mid, kubi, new RotationState(0, -MathHelper.PiOver4 * .3f, 0)).ShiftTime(length));
			anime.AddFrame(new KeyFrameAnime.Frame(mid, koshi, new RotationState(0, -MathHelper.Pi * .1f, 0)).ShiftTime(length));
			anime.SetAsZero(hiji, end, true);
			anime.SetAsZero(kata, end, true);
			anime.SetAsZero(kubi, end, false);
			anime.SetAsZero(koshi, end, false);
		}

		

		void CreateNodes()
		{
			//root = new Joint(Position.Center, Part.Root, 1, new Vector2(2, 2), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0))
			//{
			//	Color = Color.Red
			//};
			root = new Joint(Position.Center, Part.Root, Vector3.Zero);//首元のルート
			var leftArm = new Joint(Position.Left, Part.Kata, new Vector3(1, 0, 0))//肩
			{
				Visible = false
			};
			leftArm.AddChild(
				new Joint(Position.Left, Part.Hiji, 2, new Vector2(.3f, .3f), Vector3.Zero, new ValueWithRange(-MathHelper.PiOver2, MathHelper.PiOver2), new ValueWithRange(-MathHelper.Pi, MathHelper.PiOver2), new ValueWithRange(-MathHelper.PiOver2, MathHelper.Pi)) { Color = Color.Pink }//ひじ
				.AddChild(
					new Joint(Position.Left, Part.Tekubi, 2f, new Vector2(.3f, .3f), Vector3.Zero, new ValueWithRange(-MathHelper.PiOver2, MathHelper.PiOver2), new ValueWithRange(-MathHelper.PiOver4 * 3, 0), new ValueWithRange(-MathHelper.PiOver4 * 3, 0)) { Color = Color.Peru }//手首
					.AddChild(
						new Joint(Position.Left, Part.Tesaki, .7f, new Vector2(.5f, .3f), Vector3.Zero, new ValueWithRange(-MathHelper.PiOver2, MathHelper.PiOver2), new ValueWithRange(0, 0), new ValueWithRange(-MathHelper.PiOver2, MathHelper.PiOver2)) { Color = Color.Green }//手先
					)
				)
			);
			root.AddChild(leftArm);
			root.AddChild(leftArm.Mirror());

			var mata = new Joint(Position.Center, Part.Spine2, 2, new Vector2(2, 1), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(-MathHelper.PiOver4 / 2, MathHelper.PiOver4), new ValueWithRange(0));//下半身

			var leftLeg = new Joint(Position.Left, Part.Mata, new Vector3(.5f, 0, 0));//股関節
			leftLeg.AddChild(new Joint(Position.Left, Part.Hiza, 2.5f, new Vector2(1f, 1), Vector3.Zero, new ValueWithRange(-MathHelper.PiOver4, MathHelper.PiOver4), new ValueWithRange(-MathHelper.PiOver2, MathHelper.PiOver4), new ValueWithRange(-MathHelper.PiOver2, MathHelper.PiOver2)) { Color = Color.LightBlue }//膝
				.AddChild(
					new Joint(Position.Left, Part.Ashikubi, 2.5f, new Vector2(.8f, .8f), Vector3.Zero, new ValueWithRange(0, 0), new ValueWithRange(0, MathHelper.PiOver4 * 3), new ValueWithRange(0, 0)) { Color = Color.Blue }//足首
					.AddChild(
						new Joint(Position.Left, Part.Tsumasaki, .3f, new Vector2(1f, 1f), Vector3.Zero, new ValueWithRange(-MathHelper.PiOver4 / 2, MathHelper.PiOver4 / 2), new ValueWithRange(-MathHelper.PiOver2 * .5f, MathHelper.PiOver2 * .5f), new ValueWithRange(0, 0)) { Color = Color.BlueViolet }//足先
					)
				)
			);
			mata.AddChild(leftLeg);
			mata.AddChild(leftLeg.Mirror());

			root.AddChild(new Joint(Position.Center, Part.Spine1, 2, new Vector2(2, 1), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(-MathHelper.PiOver4 / 2, MathHelper.PiOver4), new ValueWithRange(0))//胸部
				.AddChild(mata));
				
			root.AddChild(
					new Joint(Position.Center, Part.Head, 1, new Vector2(.5f, .5f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0, MathHelper.TwoPi), new ValueWithRange(0)) { Color = Color.Purple });
		}

		List<Joint[]> bodyLineList;

		void SetBodyLine()
		{
			bodyLineList = new List<Joint[]>();
			Action<Part> addPart = x => bodyLineList.Add(new[]{GetPart(Position.Center, x)});
			Action<Part> addDouble = x => bodyLineList.Add(new[]{GetPart(Position.Left, x), GetPart(Position.Right, x)});
			//addPart(Part.Head);
			addPart(Part.Spine1);
			addPart(Part.Spine2);
			addDouble(Part.Hiza);
			addDouble(Part.Ashikubi);
			addDouble(Part.Tsumasaki);
		}

		

		void SetBind()
		{
			var left = GetPart(Position.Left, Part.Tsumasaki);
			var right = GetPart(Position.Right, Part.Tsumasaki);
			//Translate += new Vector3(0, -left.GetAbsolutePosition().Y, 0);
			Joint.GlobalInverse = Matrix.Invert(left.GetWorld());
		}

		Joint GetPart(Position pos, Part part)
		{
			return allJoint[new PartId(part, pos)];
		}

		void Bridge(float time, float angle)
		{
			int i = 0;
			
			foreach (var set in bodyLineList.Reverse<Joint[]>())
			{
				float a = angle / (bodyLineList.Count - 1);
				foreach (var item in set)
				{
					anime.AddFrame(new KeyFrameAnime.Frame(time, new PartId(item.Name, item.Position), new RotationState(0, a, 0)));
				}
				i++;
			}
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
			//if (state.IsKeyDown(Keys.Z))
			//{
			//	float from = (float)gameTime.TotalGameTime.TotalSeconds;
			//	anime.Reset();
			//	Bridge(from + 5, MathHelper.PiOver2 * 1.5f);
			//	Bridge(from + 10, 0);
			//	anime.Setup();
			//}
			ApplyAnime(delta);
		}

		void ApplyAnime(double time)
		{
			var states = anime.Update(time).ToArray();
			foreach (var item in allJoint)
			{
				item.Value.ApplyState(states.Single(x => x.Part == item.Key).State);
			}
			SetBind();
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
			//DrawGravityCenter();
		}

		Vector3 GetGravityCenter()
		{
			var a = Vector3.Zero;
			var w = 0f;
			foreach (var item in allJoint.Values)
			{
				a += item.GetAbsoluteCenter() * item.Mass;
				w += item.Mass;
			}
			return a / w;
		}

		void DrawGravityCenter()
		{
			var p = GetGravityCenter();
			GraphicsDevice.DepthStencilState = DepthStencilState.None;
			Game.GetComponent<BoxRenderer>().DrawBox(Color.Pink, Matrix.CreateScale(.1f) * Matrix.CreateTranslation(p) * Joint.GlobalInverse);
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}
	}
}
