using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Masa.BridgeSim
{
	public class MyGame : Game
	{
		public static MyGame Game;
		GraphicsDeviceManager dev;

		public MyGame()
			: base()
		{
			Game = this;
			dev = new GraphicsDeviceManager(this)
			{
				GraphicsProfile = Microsoft.Xna.Framework.Graphics.GraphicsProfile.HiDef,
				PreferredBackBufferHeight = 600,
				PreferredBackBufferWidth = 800,
				PreferredBackBufferFormat = SurfaceFormat.Color,
				PreferMultiSampling = true,
			};
			dev.PreparingDeviceSettings += dev_PreparingDeviceSettings;
			CreateHuman();
			Components.Add(new Camera(this));
			Components.Add(new BoxRenderer(this));
			Components.Add(new World(this));
		}

		void dev_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
		}

		protected override void LoadContent()
		{
			base.LoadContent();
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		void CreateHuman()
		{
			var human = new JointHuman(this);
			Components.Add(human);
			//human.SaveToXml("human.xml");
		}

		protected override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Components.Remove(this.GetComponent<JointHuman>());
				CreateHuman();
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.CornflowerBlue, 1, 0);
			base.Draw(gameTime);
			
		}
	}
}
