using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Masa.BridgeSim
{
	public class Joint
	{
		//public Vector3 Position;
		Joint Parent;
		List<Joint> Children;

		ValueWithRange Yaw, Pitch, Roll;

		float Length;//親と自分との距離
		Vector2 Size;//肉の大きさ
		public Color Color { get; set; }
		Vector3 ParentOffset;
		public bool Visible { get; set; }


		public Joint(float length, Vector2 size, Vector3 parentOffset, ValueWithRange yaw, ValueWithRange pitch, ValueWithRange roll)
			: this()
		{
			Length = length;
			Size = size;
			Yaw = yaw;
			Pitch = pitch;
			Roll = roll;
			Color = Color.CornflowerBlue;
			ParentOffset = parentOffset;
			Visible = true;
		}

		public Joint()
		{
			Children = new List<Joint>();
		}

		public Joint AddChild(Joint child)
		{
			child.Parent = this;
			Children.Add(child);
			return this;
		}

	

		/// <summary>
		/// 関節自体の位置
		/// </summary>
		/// <returns></returns>
		Vector3 GetAbsolutePosition()
		{
			if (Parent == null)
			{
				return Vector3.Zero;
			}
			else
			{
				return Parent.GetAbsolutePosition() + ParentOffset + Vector3.Transform(Vector3.UnitZ * Length, GetWorldRotation());
			}
		}

		Matrix GetWorldRotation()
		{
			var m = Matrix.CreateFromYawPitchRoll(Yaw.Value, Pitch.Value, Roll.Value);
			if (Parent == null)
			{
				return m;
			}
			else
			{
				return Parent.GetWorldRotation() * m;
			}
		}

		/// <summary>
		/// 自分と親とをつなぐ肉の中心
		/// </summary>
		/// <returns></returns>
		Vector3 GetAbsoluteCenter()
		{
			if (Parent == null)
			{
				return GetAbsolutePosition();
			}
			else
			{
				return (Parent.GetAbsolutePosition() + ParentOffset + GetAbsolutePosition()) * .5f;
			}
		}

		public Joint Mirror()
		{
			var root = MirrorSingle();
			foreach (var item in Children)
			{
				root.AddChild(item.Mirror());
			}
			return root;
		}

		Joint MirrorSingle()
		{
			return new Joint(Length, Size, new Vector3(-ParentOffset.X, ParentOffset.Y, ParentOffset.Z), Yaw.Mirror(), Pitch, Roll.Mirror())
			{
				Color = this.Color,
				Visible = this.Visible
			};
		}

		public void Draw(BoxRenderer render)
		{
			if (Visible)
			{
				var trans = Matrix.CreateTranslation(GetAbsoluteCenter());
				var rot = GetWorldRotation();
				var scale = Matrix.CreateScale(Size.X * .5f, Size.Y * .5f, Length * .5f);
				render.DrawBox(Color, scale * rot * trans);
			}

			foreach (var item in Children)
			{
				item.Draw(render);
			}
		}

	}

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
						new Joint(.7f, new Vector2(.5f, .3f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0)) { Color = Color.Green}//手先
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
						new Joint(1f, new Vector2(1f, .3f), Vector3.Zero, new ValueWithRange(0), new ValueWithRange(-MathHelper.PiOver2, -MathHelper.PiOver2 * 1.5f, -MathHelper.PiOver2 * .5f), new ValueWithRange(0, 0, 0)) { Color = Color.BlueViolet}//足先
					)
				)
			);
			root.AddChild(leftLeg);
			root.AddChild(leftLeg.Mirror());

			root.AddChild(new Joint(1, new Vector2(.5f, .5f), new Vector3(0, 2, 0), new ValueWithRange(0), new ValueWithRange(-MathHelper.PiOver2, -MathHelper.PiOver2 * 3, 0), new ValueWithRange(0)) { Color = Color.Purple });//頭
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			var box = Game.GetComponent<BoxRenderer>();
			box.Begin();
			root.Draw(box);
		}
	}

	public class ValueWithRange
	{
		float val;
		public readonly float Min, Max;

		public float Value
		{
			get { return val; }
			set
			{
				val = value;
				Debug.Assert(Min <= Value && Value <= Max);
			}
		}

		public ValueWithRange(float value)
		{
			Min = float.NegativeInfinity;
			Max = float.PositiveInfinity;
			Value = value;
		}

		public ValueWithRange(float value, float min, float max)
		{
			Min = min;
			Max = max;
			Value = value;
		}

		public ValueWithRange Mirror()
		{
			return new ValueWithRange(-Value, -Max, -Min);
		}

	}

}
