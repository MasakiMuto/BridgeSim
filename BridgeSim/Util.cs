using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Masa.BridgeSim
{
	public static class Util
	{
		public static T GetComponent<T>(this Game game) where T : GameComponent
		{
			return game.Components.First(x => x is T) as T;
		}

		public static XElement ToXml(this Vector3 v, string name)
		{
			var elm = new XElement(name);
			elm.SetAttributeValue("x", v.X);
			elm.SetAttributeValue("y", v.Y);
			elm.SetAttributeValue("z", v.Z);
			return elm;
		}

		public static XElement ToXml(this Vector2 v, string name)
		{
			var elm = new XElement(name);
			elm.SetAttributeValue("x", v.X);
			elm.SetAttributeValue("y", v.Y);
			return elm;
		}
	}
}
