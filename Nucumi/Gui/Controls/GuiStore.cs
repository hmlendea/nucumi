using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace Nucumi.Gui.Controls
{
    internal sealed class GuiStore : GuiControl
    {
        private static Colour DefaultTintColour => new(30, 20, 10);
        private static Colour CurrentPlayerTintColour => new(40, 65, 25);

        private GuiImage backgroundImage;
        private GuiText walnutCountText;

        public int WalnutCount { get; set; }

        public bool IsCurrentPlayerStore { get; set; }

        protected override void DoLoadContent()
        {
            backgroundImage = new GuiImage
            {
                ContentFile = "ScreenManager/FillImage"
            };

            walnutCountText = new GuiText
            {
                FontName = "DefaultFont",
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            };

            RegisterChildren(backgroundImage, walnutCountText);
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void SetChildrenProperties()
        {
            backgroundImage.Location = Point2D.Empty;
            backgroundImage.Size = Size;

            Colour tintColour = DefaultTintColour;

            if (IsCurrentPlayerStore)
            {
                tintColour = CurrentPlayerTintColour;
            }

            backgroundImage.TintColour = tintColour;

            walnutCountText.Location = Point2D.Empty;
            walnutCountText.Size = Size;
            walnutCountText.Text = WalnutCount.ToString();
            walnutCountText.ForegroundColour = Colour.White;
        }
    }
}
