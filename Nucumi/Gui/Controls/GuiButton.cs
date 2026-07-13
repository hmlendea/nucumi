using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Input;
using NuciXNA.Primitives;

namespace Nucumi.Gui.Controls
{
    internal sealed class GuiButton : GuiControl
    {
        private static int SpriteFrameSize => 128;

        private static Colour HoverTintColour => new Colour(255, 220, 80);
        private static Colour PressedTintColour => new Colour(255, 140, 30);

        private GuiImage buttonImage;
        private bool isLeftMouseButtonHeld;

        public ButtonType ButtonType { get; set; }

        protected override void DoLoadContent()
        {
            buttonImage = new GuiImage
            {
                ContentFile = "board/buttons"
            };

            RegisterChildren(buttonImage);
            SetChildrenProperties();

            InputManager.Instance.MouseButtonPressed += OnMouseButtonPressed;
            InputManager.Instance.MouseButtonReleased += OnMouseButtonReleased;
        }

        protected override void DoUnloadContent()
        {
            InputManager.Instance.MouseButtonPressed -= OnMouseButtonPressed;
            InputManager.Instance.MouseButtonReleased -= OnMouseButtonReleased;
        }

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
                buttonImage.TintColour = HoverTintColour;
            }

            if (IsHovered && isLeftMouseButtonHeld)
            {
                buttonImage.TintColour = PressedTintColour;
            }
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (Equals(mouseButtonEventArgs.Button, MouseButton.Left))
            {
                isLeftMouseButtonHeld = true;
            }
        }

        private void OnMouseButtonReleased(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (Equals(mouseButtonEventArgs.Button, MouseButton.Left))
            {
                isLeftMouseButtonHeld = false;
            }
        }
    }
}
