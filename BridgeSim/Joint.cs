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
	public class RotationState
	{
		public readonly float Yaw, Pitch, Roll;

		public RotationState(float yaw, float pitch, float roll)
		{
			Yaw = yaw;
			Pitch = pitch;
			Roll = roll;
		}

		public RotationState Mirror()
		{
			return new RotationState(-Yaw, Pitch, -Roll);
		}

		public RotationState Lerp(RotationState s2, float weight)
		{
			return new RotationState(MathHelper.Lerp(Yaw, s2.Yaw, weight), MathHelper.Lerp(Pitch, s2.Pitch, weight), MathHelper.Lerp(Roll, s2.Roll, weight));
		}

		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", Yaw, Pitch, Roll);
		}
	}

	public class Joint
	{
		//public Vector3 Position;
		Joint Parent;
		List<Joint> Children;

		public ValueWithRange Yaw{get; private set;}
		public ValueWithRange Pitch{get; private set;}
		public ValueWithRange Roll { get; private set; }

		float Length;//親と自分との距離
		Vector2 Size;//肉の大きさ
		public Color Color { get; set; }
		Vector3 ParentOffset;
		public bool Visible { get; set; }

		public Part Name { get; private set; }
		public Position Position { get; private set; }

		public float Mass { get; set; }

		public Joint(Position pos, Part name, float length, Vector2 size, Vector3 parentOffset, ValueWithRange yaw, ValueWithRange pitch, ValueWithRange roll)
			: this()
		{
			Position = pos;
			Name = name;
			Length = length;
			Size = size;
			Yaw = yaw;
			Pitch = pitch;
			Roll = roll;
			Color = Color.CornflowerBlue;
			ParentOffset = parentOffset;
			Visible = true;
			Mass = length * size.X * size.Y;
		}

		public Joint(Position pos, Part name, Vector3 parentOffset)
			: this(pos, name, 0, Vector2.Zero, parentOffset, new ValueWithRange(0), new ValueWithRange(0), new ValueWithRange(0))
		{
			Visible = false;
		}

		public override int GetHashCode()
		{
			return (int)Position * (int)Name;
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
		/// 自分を含む子孫全て
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Joint> AllChildren()
		{
			yield return this;
			foreach (var item in Children.SelectMany(x=>x.AllChildren()))
			{
				yield return item;
			}
		}

		public override string ToString()
		{
			return Position.ToString() + Name.ToString() + " " + base.ToString();
		}

		/// <summary>
		/// 関節自体の位置
		/// </summary>
		/// <returns></returns>
		public Vector3 GetAbsolutePosition()
		{
			if (Parent == null)
			{
				return JointHuman.Human.Translate;
			}
			else
			{
				return Parent.GetAbsolutePosition() + Vector3.Transform(ParentOffset, GetWorldRotation()) + Vector3.Transform(-Vector3.UnitY * Length, GetWorldRotation());
			}
		}

		Quaternion GetWorldRotation()
		{
			var m = Quaternion.CreateFromYawPitchRoll(Yaw.Value, Pitch.Value, Roll.Value);
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
			Position mp = BridgeSim.Position.Center;
			switch (Position)
			{
				case Position.Center:
					mp = BridgeSim.Position.Center;
					break;
				case Position.Left:
					mp = BridgeSim.Position.Right;
					break;
				case Position.Right:
					mp = BridgeSim.Position.Left;
					break;
			}
			return new Joint(mp, Name, Length, Size, new Vector3(-ParentOffset.X, ParentOffset.Y, ParentOffset.Z), Yaw.Mirror(), Pitch.Clone(), Roll.Mirror())
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
				var rot = Matrix.CreateFromQuaternion(GetWorldRotation());
				var scale = Matrix.CreateScale(Size.X * .5f, Length * .5f, Size.Y * .5f);
				render.DrawBox(Color, scale * rot * trans);
			}

			foreach (var item in Children)
			{
				item.Draw(render);
			}
		}

		public void ApplyState(RotationState s)
		{
			Yaw.Value = s.Yaw;
			Pitch.Value = s.Pitch;
			Roll.Value = s.Roll;
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
