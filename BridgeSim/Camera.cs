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

		public Matrix GetProjection(Game game)
		{
			return Matrix.CreatePerspectiveFieldOfView(.8f, game.GraphicsDevice.Viewport.AspectRatio, .1f, 100);
		}

		Vector3 Position, TargetPosition, Upper;
		readonly Vector3 BasePosition;
		readonly Vector3 Axis;
		readonly double RotationSpeed = .005;

		public Camera(Game game)
			: base(game)
		{
			TargetPosition = new Vector3(0, 5, 0);
			Upper = Vector3.Up;
			BasePosition = new Vector3(10, 10, 10);
			Axis = new Vector3(0, 1, 0);
		}

		float rot = 0;

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			var mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
			rot = mouse.ScrollWheelValue;
			var q = Quaternion.CreateFromAxisAngle(Axis, (float)(rot * RotationSpeed));
			Position = Vector3.Transform(BasePosition, q);
		}
	}
}
