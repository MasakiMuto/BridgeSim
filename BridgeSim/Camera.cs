using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;


namespace Masa.BridgeSim
{
	public class Camera : GameComponent
	{
		public Matrix View
		{
			get
			{
				return Matrix.CreateLookAt(Position, TargetPosition, Upper);
			}
		}
		Vector3 Position, TargetPosition, Upper;
		readonly Vector3 BasePosition;
		readonly Vector3 Axis;
		readonly double RotationSpeed = .4;

		public Camera(Game game)
			: base(game)
		{
			TargetPosition = Vector3.Zero;
			Upper = Vector3.Up;
			BasePosition = new Vector3(10, 10, 10);
			Axis = new Vector3(0, 1, 0);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			var q = Quaternion.CreateFromAxisAngle(Axis, (float)(gameTime.TotalGameTime.TotalSeconds * RotationSpeed));
			Position = Vector3.Transform(BasePosition, q);
		}
	}
}
