using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace Nucumi.Gui.Controls
{
    internal sealed class GuiBasket : GuiControl
    {
        private static Colour DefaultTintColour => new(60, 40, 20);
        private static Colour SelectableTintColour => new(130, 90, 45);
        private static Colour HoveredTintColour => new(200, 150, 70);

        private GuiImage backgroundImage;
        private GuiText walnutCountText;

        public int BoardIndex { get; set; }

        public int WalnutCount { get; set; }

        public bool IsSelectable { get; set; }

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

            if (IsSelectable)
            {
                tintColour = SelectableTintColour;
            }

            if (IsSelectable && IsHovered)
            {
                tintColour = HoveredTintColour;
            }

            backgroundImage.TintColour = tintColour;

            walnutCountText.Location = Point2D.Empty;
            walnutCountText.Size = Size;
            walnutCountText.Text = WalnutCount.ToString();
            walnutCountText.ForegroundColour = Colour.White;
        }
    }
}
