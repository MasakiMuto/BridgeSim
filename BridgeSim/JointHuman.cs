﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Xml.Linq;

namespace Masa.BridgeSim
{
	public enum Part
	{
		Head,
		Root,
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

	public class JointHuman : DrawableGameComponent
	{
		Joint root;
		Joint[] allJoint;
		

		public JointHuman(Game game)
			: base(game)
		{
		
			root = new Joint(Position.Center, Part.Root, 1, new Vector2(2, 4), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0))
			{
				Color = Color.Red
			};
			var leftArm = new Joint(Position.Left, Part.Kata, 0, Vector2.Zero, new Vector3(1, 2, 0), new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0))//肩
			{
				Visible = false
			};
			leftArm.AddChild(
				new Joint(Position.Left, Part.Hiji, 2, new Vector2(.3f, .3f), Vector3.Zero, new ValueWithRange(MathHelper.PiOver2, -MathHelper.PiOver2, MathHelper.Pi), new ValueWithRange(MathHelper.PiOver2 * .8f, -MathHelper.Pi, MathHelper.Pi), new ValueWithRange(0, -MathHelper.PiOver2, MathHelper.PiOver2)) { Color = Color.Pink }//ひじ
				.AddChild(
					new Joint(Position.Left, Part.Tekubi, 2f, new Vector2(.3f, .3f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0)) { Color = Color.Peru }//手首
					.AddChild(
						new Joint(Position.Left, Part.Tesaki, .7f, new Vector2(.5f, .3f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0)) { Color = Color.Green }//手先
					)
				)
			);
			root.AddChild(leftArm);
			root.AddChild(leftArm.Mirror());

			var leftLeg = new Joint(Position.Left, Part.Mata, 0, Vector2.Zero, new Vector3(.5f, -2, 0), new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0)) { Visible = false };//股関節
			leftLeg.AddChild(new Joint(Position.Left, Part.Hiza, 2.5f, new Vector2(1f, 1), Vector3.Zero, new ValueWithRange(0, -MathHelper.PiOver2, MathHelper.PiOver2), new ValueWithRange(MathHelper.PiOver2, -MathHelper.PiOver2, MathHelper.Pi), new ValueWithRange(0, -MathHelper.PiOver4, MathHelper.PiOver4)) { Color = Color.LightBlue }//膝
				.AddChild(
					new Joint(Position.Left, Part.Ashikubi, 2.5f, new Vector2(.8f, .8f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0, 0, MathHelper.Pi), new ValueWithRange(0, -MathHelper.Pi / 8, MathHelper.Pi / 8)) { Color = Color.Blue }//足首
					.AddChild(
						new Joint(Position.Left, Part.Tsumasaki, 1f, new Vector2(1f, .3f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(-MathHelper.PiOver2, -MathHelper.PiOver2 * 1.5f, -MathHelper.PiOver2 * .5f), new ValueWithRange(0, 0, 0)) { Color = Color.BlueViolet }//足先
					)
				)
			);
			root.AddChild(leftLeg);
			root.AddChild(leftLeg.Mirror());


			root.AddChild(new Joint(Position.Center, Part.Head, 1, new Vector2(.5f, .5f), new Vector3(0, 2, 0), new ValueWithRange(0), new ValueWithRange(-MathHelper.PiOver2, -MathHelper.PiOver2 * 3, 0), new ValueWithRange(0)) { Color = Color.Purple });

			allJoint = root.AllChildren().ToArray();
		}

		Joint GetPart(Position pos, Part part)
		{
			return allJoint.First(x => x.Position == pos && x.Name == part);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			
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
