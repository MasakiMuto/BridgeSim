using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Masa.BridgeSim
{
	struct MyVertex : IVertexType
	{
		public Vector3 Position;
		public Vector3 Normal;

		public MyVertex(Vector3 pos, Vector3 norm)
		{
			Position = pos;
			Normal = norm;
		}

		public VertexDeclaration VertexDeclaration
		{
			get
			{
				return new VertexDeclaration(
					new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
					new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0));
			}
		}
	}
}
