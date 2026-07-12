using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui;
using NuciXNA.Gui.Controls;
using NuciXNA.Gui.Screens;
using NuciXNA.Primitives;

namespace Nucumi.Screens
{
    internal sealed class GameScreen : Screen
    {
        private GuiText helloWorldText;

        public GameScreen()
        {
            BackgroundColour = Colour.Black;
        }

        protected override void DoLoadContent()
        {
            helloWorldText = new GuiText
            {
                Text = "Hello World",
                FontName = "DefaultFont",
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            };

            GuiManager.Instance.RegisterControls(helloWorldText);
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime)
        {
            SetChildrenProperties();
        }

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void SetChildrenProperties()
        {
            helloWorldText.Location = Point2D.Empty;
            helloWorldText.Size = new Size2D(
                ScreenManager.Instance.Size.Width,
                ScreenManager.Instance.Size.Height);
            helloWorldText.ForegroundColour = Colour.White;
        }
    }
}
