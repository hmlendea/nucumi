using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Gui.Controls;
using NuciXNA.Input;
using NuciXNA.Primitives;

namespace Nucumi.Gui.Controls
{
    internal sealed class GuiBasket : GuiControl
    {
        private static int SpriteFrameSize => 240;
        private static int MaxFrameIndex => 14;

        private static Colour HoverTintColour => new(255, 220, 80);
        private static Colour PressedTintColour => new(255, 140, 30);

        private GuiImage basketImage;
        private GuiLabel walnutCountLabel;
        private bool isLeftMouseButtonHeld;
        private bool isMouseButtonPressedOverThis;

        public event MouseButtonEventHandler Released;

        public int BoardIndex { get; set; }

        public int WalnutCount { get; set; }

        public bool IsSelectable { get; set; }

        public LabelPlacement LabelPlacement { get; set; } = LabelPlacement.Below;

        protected override void DoLoadContent()
        {
            basketImage = new GuiImage
            {
                ContentFile = "board/basket"
            };

            walnutCountLabel = new GuiLabel();

            RegisterChildren(basketImage, walnutCountLabel);
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
            int frameIndex = Math.Min(WalnutCount, MaxFrameIndex);

            Point2D spriteLocation;
            Size2D spriteSize;
            Point2D labelLocation;
            Size2D labelSize;

            switch (LabelPlacement)
            {
                case LabelPlacement.Above:
                    int aboveLabelHeight = Size.Height - Size.Height * 3 / 4;
                    spriteLocation = new Point2D(0, aboveLabelHeight);
                    spriteSize = new Size2D(Size.Width, Size.Height * 3 / 4);
                    labelLocation = Point2D.Empty;
                    labelSize = new Size2D(Size.Width, aboveLabelHeight);

                    break;

                default: // Below.
                    spriteLocation = Point2D.Empty;
                    spriteSize = new Size2D(Size.Width, Size.Height * 3 / 4);
                    labelLocation = new Point2D(0, Size.Height * 3 / 4);
                    labelSize = new Size2D(Size.Width, Size.Height - Size.Height * 3 / 4);

                    break;
            }

            basketImage.Location = spriteLocation;
            basketImage.Size = spriteSize;
            basketImage.SourceRectangle = new Rectangle2D(frameIndex * SpriteFrameSize, 0, SpriteFrameSize, SpriteFrameSize);

            basketImage.TintColour = Colour.White;

            if (IsSelectable && IsHovered)
            {
                basketImage.TintColour = HoverTintColour;
            }

            if (IsSelectable && IsHovered && isLeftMouseButtonHeld)
            {
                basketImage.TintColour = PressedTintColour;
            }

            walnutCountLabel.Location = labelLocation;
            walnutCountLabel.Size = labelSize;
            walnutCountLabel.BackgroundWidth = labelSize.Width;
            walnutCountLabel.Text = WalnutCount.ToString();
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!Equals(mouseButtonEventArgs.Button, MouseButton.Left))
            {
                return;
            }

            isLeftMouseButtonHeld = true;
            isMouseButtonPressedOverThis = DisplayRectangle.Contains(mouseButtonEventArgs.Location);
        }

        private void OnMouseButtonReleased(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!Equals(mouseButtonEventArgs.Button, MouseButton.Left))
            {
                return;
            }

            isLeftMouseButtonHeld = false;

            if (isMouseButtonPressedOverThis && DisplayRectangle.Contains(mouseButtonEventArgs.Location))
            {
                Released?.Invoke(this, mouseButtonEventArgs);
            }

            isMouseButtonPressedOverThis = false;
        }
    }
}
