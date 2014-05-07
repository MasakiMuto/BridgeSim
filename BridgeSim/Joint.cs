using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Xml.Linq;

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

		public XElement ToXml()
		{
			var elm = new XElement("joint");
			elm.Add(Yaw.ToXml("yaw"), Pitch.ToXml("pitch"), Roll.ToXml("roll"));
			elm.Add(ParentOffset.ToXml("offset"));
			elm.Add(new XElement("length", Length));
			elm.Add(Size.ToXml("size"));
			elm.Add(Color.ToVector3().ToXml("color"));
			elm.Add(new XElement("visible", Visible));
			foreach (var item in Children)
			{
				elm.Add(item.ToXml());
			}
			return elm;
		}

	}

	

}
