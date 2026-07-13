using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace Nucumi.Gui.Controls
{
    internal sealed class GuiStatusBar : GuiControl
    {
        private GuiImage backgroundImage;
        private GuiText statusText;

        public string Text
        {
            get
            {
                string text = string.Empty;

                if (statusText is not null)
                {
                    text = statusText.Text;
                }

                return text;
            }
            set => statusText?.Text = value;
        }

        protected override void DoLoadContent()
        {
            backgroundImage = new GuiImage
            {
                ContentFile = "board/status_bar",
                SamplerState = SamplerState.LinearClamp
            };

            statusText = new GuiText
            {
                FontName = "DefaultFont",
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle,
                ForegroundColour = Colour.White
            };

            RegisterChildren(backgroundImage, statusText);
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void SetChildrenProperties()
        {
            backgroundImage.Location = Point2D.Empty;
            backgroundImage.Size = Size;

            statusText.Location = Point2D.Empty;
            statusText.Size = Size;
        }
    }
}
