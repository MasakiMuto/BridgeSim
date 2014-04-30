using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Masa.BridgeSim
{
	public static class Util
	{
		public static T GetComponent<T>(this Game game) where T : GameComponent
		{
			return game.Components.First(x => x is T) as T;
		}
	}
}
