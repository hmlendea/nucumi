using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui.Controls;
using NuciXNA.Primitives;

namespace Nucumi.Gui.Controls
{
    internal sealed class GuiStore : GuiControl
    {
        private static int SpriteFrameSize => 240;
        private static int MaxFrameIndex => 14;

        private GuiImage storeImage;
        private GuiText walnutCountText;

        public int WalnutCount { get; set; }

        public bool IsCurrentPlayerStore { get; set; }

        public LabelPlacement LabelPlacement { get; set; } = LabelPlacement.Right;

        protected override void DoLoadContent()
        {
            storeImage = new GuiImage
            {
                ContentFile = "board/basket"
            };

            walnutCountText = new GuiText
            {
                FontName = "DefaultFont",
                HorizontalAlignment = Alignment.Middle,
                VerticalAlignment = Alignment.Middle
            };

            RegisterChildren(storeImage, walnutCountText);
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() { }

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

                case LabelPlacement.Left:
                    int leftLabelWidth = Size.Width - Size.Width * 3 / 4;
                    spriteLocation = new Point2D(leftLabelWidth, 0);
                    spriteSize = new Size2D(Size.Width * 3 / 4, Size.Height);
                    labelLocation = Point2D.Empty;
                    labelSize = new Size2D(leftLabelWidth, Size.Height);

                    break;

                case LabelPlacement.Right:
                    int rightLabelWidth = Size.Width - Size.Width * 3 / 4;
                    spriteLocation = Point2D.Empty;
                    spriteSize = new Size2D(Size.Width * 3 / 4, Size.Height);
                    labelLocation = new Point2D(Size.Width * 3 / 4, 0);
                    labelSize = new Size2D(rightLabelWidth, Size.Height);

                    break;

                default: // Below.
                    spriteLocation = Point2D.Empty;
                    spriteSize = new Size2D(Size.Width, Size.Height * 3 / 4);
                    labelLocation = new Point2D(0, Size.Height * 3 / 4);
                    labelSize = new Size2D(Size.Width, Size.Height - Size.Height * 3 / 4);

                    break;
            }

            storeImage.Location = spriteLocation;
            storeImage.Size = spriteSize;
            storeImage.SourceRectangle = new Rectangle2D(frameIndex * SpriteFrameSize, 0, SpriteFrameSize, SpriteFrameSize);

            walnutCountText.Location = labelLocation;
            walnutCountText.Size = labelSize;
            walnutCountText.Text = WalnutCount.ToString();
            walnutCountText.ForegroundColour = Colour.Black;
        }
    }
}
