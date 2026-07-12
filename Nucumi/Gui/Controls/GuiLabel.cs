using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace Nucumi.Gui.Controls
{
    internal sealed class GuiLabel : GuiControl
    {
        private static int TextureWidth => 480;
        private static int TextureHeight => 179;

        private GuiImage backgroundImage;
        private GuiText labelText;

        public string Text
        {
            get => labelText?.Text ?? string.Empty;
            set => labelText?.Text = value;
        }

        public int BackgroundWidth { get; set; }

        protected override void DoLoadContent()
        {
            backgroundImage = new GuiImage
            {
                ContentFile = "board/label",
                SamplerState = SamplerState.LinearClamp
            };

            labelText = new GuiText
            {
                FontName = "DefaultFont",
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle,
                ForegroundColour = Colour.White
            };

            RegisterChildren(backgroundImage, labelText);
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void SetChildrenProperties()
        {
            int bgWidth = BackgroundWidth > 0 ? BackgroundWidth : Size.Width;
            int bgHeight = bgWidth * TextureHeight / TextureWidth;
            int bgX = (Size.Width - bgWidth) / 2;
            int bgY = (Size.Height - bgHeight) / 2;

            backgroundImage.Location = new Point2D(bgX, bgY);
            backgroundImage.Size = new Size2D(bgWidth, bgHeight);

            labelText.Location = Point2D.Empty;
            labelText.Size = Size;
        }
    }
}
