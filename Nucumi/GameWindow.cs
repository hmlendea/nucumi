using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.DataAccess.Content;
using NuciXNA.Graphics;
using NuciXNA.Gui.Screens;
using NuciXNA.Input;

using Nucumi.Gui.Screens;

namespace Nucumi
{
    internal sealed class GameWindow : Game
    {
        private readonly GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;

        public GameWindow()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 853
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() => base.Initialize();

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            NuciContentManager.Instance.LoadContent(Content, GraphicsDevice);
            GraphicsManager.Instance.Graphics = graphicsDeviceManager;
            GraphicsManager.Instance.SpriteBatch = spriteBatch;

            ScreenManager.Instance.StartingScreenType = typeof(GameScreen);
            ScreenManager.Instance.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.Instance.Update(Window);
            ScreenManager.Instance.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
            ScreenManager.Instance.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
