using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Masa.BridgeSim
{
	public class MyGame : Game
	{
		GraphicsDeviceManager dev;

		public MyGame()
			: base()
		{
			dev = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferHeight = 600,
				PreferredBackBufferWidth = 800,
				PreferredBackBufferFormat = SurfaceFormat.Color,
			};
			dev.PreparingDeviceSettings += dev_PreparingDeviceSettings;
		}

		void dev_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			GraphicsDevice.Clear(Color.CornflowerBlue);
		}
	}
}
