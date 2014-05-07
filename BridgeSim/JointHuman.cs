using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Xml.Linq;

namespace Masa.BridgeSim
{
	public class JointHuman : DrawableGameComponent
	{
		Joint root;

		public JointHuman(Game game)
			: base(game)
		{
			root = new Joint(1, new Vector2(2, 4), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0))
			{
				Color = Color.Red
			};
			var leftArm = new Joint(0, Vector2.Zero, new Vector3(1, 2, 0), new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0))//肩
			{
				Visible = false
			};
			leftArm.AddChild(
				new Joint(2, new Vector2(.3f, .3f), Vector3.Zero, new ValueWithRange(.4f), new ValueWithRange(0), new ValueWithRange(0)) { Color = Color.Pink }//ひじ
				.AddChild(
					new Joint(2f, new Vector2(.3f, .3f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0)) { Color = Color.Peru }//手首
					.AddChild(
						new Joint(.7f, new Vector2(.5f, .3f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0)) { Color = Color.Green }//手先
					)
				)
			);
			root.AddChild(leftArm);
			root.AddChild(leftArm.Mirror());

			var leftLeg = new Joint(0, Vector2.Zero, new Vector3(.5f, -2, 0), new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0)) { Visible = false };//股関節
			leftLeg.AddChild(new Joint(2.5f, new Vector2(1f, 1), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0)) { Color = Color.LightBlue }//膝
				.AddChild(
					new Joint(2.5f, new Vector2(1f, 1f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0)) { Color = Color.Blue }//足首
					.AddChild(
						new Joint(1f, new Vector2(1f, .3f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(-MathHelper.PiOver2, -MathHelper.PiOver2 * 1.5f, -MathHelper.PiOver2 * .5f), new ValueWithRange(0, 0, 0)) { Color = Color.BlueViolet }//足先
					)
				)
			);
			root.AddChild(leftLeg);
			root.AddChild(leftLeg.Mirror());

			root.AddChild(new Joint(1, new Vector2(.5f, .5f), new Vector3(0, 2, 0), new ValueWithRange(0), new ValueWithRange(-MathHelper.PiOver2, -MathHelper.PiOver2 * 3, 0), new ValueWithRange(0)) { Color = Color.Purple });//頭
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
