using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace Nucumi.Gui.Controls
{
    internal sealed class GuiButton : GuiControl
    {
        private static int SpriteFrameSize => 128;

        private GuiImage buttonImage;

        public ButtonType ButtonType { get; set; }

        protected override void DoLoadContent()
        {
            buttonImage = new GuiImage
            {
                ContentFile = "board/buttons"
            };

            RegisterChildren(buttonImage);
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() { }

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void SetChildrenProperties()
        {
            buttonImage.Location = Point2D.Empty;
            buttonImage.Size = Size;
            buttonImage.SourceRectangle = new Rectangle2D((int)ButtonType * SpriteFrameSize, 0, SpriteFrameSize, SpriteFrameSize);
            buttonImage.TintColour = Colour.White;

            if (IsHovered)
            {
                buttonImage.TintColour = new Colour(255, 220, 80);
            }
        }
    }
}
