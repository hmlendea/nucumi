using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NuciXNA.Gui.Controls;
using NuciXNA.Input;
using NuciXNA.Primitives;
using Nucumi.Model;

namespace Nucumi.Gui.Controls
{
    // Board layout (all positions relative to control origin):
    //
    //   [P2 store] [P2b5] [P2b4] [P2b3] [P2b2] [P2b1] [P2b0] [P1 store]
    //              [P1b0] [P1b1] [P1b2] [P1b3] [P1b4] [P1b5]
    //
    // P2 basket in column c has board index 12 − c (so P2b5 = index 12 is leftmost).
    internal sealed class GuiGameBoard : GuiControl
    {
        // Ratio BasketSize:HorizontalGap = 143:17 derived from the 1536×1024 reference image pixel analysis.
        // 8 baskets + 2 stores + 7 horizontal gaps must fill the board width: 8*143 + 7*17 = 1263.
        private static int BasketSizeNumerator => 143;
        private static int TotalInnerWidthAtReference => 1263;

        private readonly Board board;
        private readonly GuiBasket[] player1Baskets;
        private readonly GuiBasket[] player2Baskets;
        private GuiBasket player1Store;
        private GuiBasket player2Store;

        public GuiGameBoard(Board board)
        {
            this.board = board;
            player1Baskets = new GuiBasket[Board.BasketsPerPlayer];
            player2Baskets = new GuiBasket[Board.BasketsPerPlayer];
        }

        protected override void DoLoadContent()
        {
            player2Store = new GuiBasket
            {
                BoardIndex = Board.Player2StoreIndex,
                LabelPlacement = LabelPlacement.Above
            };
            player1Store = new GuiBasket
            {
                BoardIndex = Board.Player1StoreIndex,
                LabelPlacement = LabelPlacement.Below
            };

            for (int columnIndex = 0; columnIndex < Board.BasketsPerPlayer; columnIndex++)
            {
                player2Baskets[columnIndex] = new GuiBasket { BoardIndex = 12 - columnIndex, LabelPlacement = LabelPlacement.Below };
                player1Baskets[columnIndex] = new GuiBasket { BoardIndex = columnIndex, LabelPlacement = LabelPlacement.Above };
            }

            RegisterChildren(player1Baskets);
            RegisterChildren(player2Baskets);
            RegisterChildren(player1Store, player2Store);

            InputManager.Instance.MouseButtonPressed += OnMouseButtonPressed;
        }

        protected override void DoUnloadContent()
            => InputManager.Instance.MouseButtonPressed -= OnMouseButtonPressed;

        protected override void DoUpdate(GameTime gameTime)
        {
            UpdateLayout();
            UpdateControls();
        }

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void UpdateLayout()
        {
            int boardWidth = Size.Width;
            int boardHeight = Size.Height;
            int basketSize = boardWidth * BasketSizeNumerator / TotalInnerWidthAtReference;

            // Label area is 1/3 of the basket sprite size; total control extent = 4/3 * basketSize.
            int labelExtent = basketSize / 3;
            int basketControlHeight = basketSize + labelExtent;
            int storeSpriteY = (boardHeight - basketSize) / 2;

            // Stores stay at the board edges.
            // P2 store (left): LabelPlacement.Above → sprite is in lower 3/4; shift control up by labelExtent.
            player2Store.Location = new Point2D(0, storeSpriteY - labelExtent);
            player2Store.Size = new Size2D(basketSize, basketControlHeight);

            // P1 store (right): LabelPlacement.Below → sprite is in upper 3/4; control top aligns with sprite.
            player1Store.Location = new Point2D(boardWidth - basketSize, storeSpriteY);
            player1Store.Size = new Size2D(basketSize, basketControlHeight);

            // Baskets use a tighter gap and are centered as a group within the board width.
            int basketGap = basketSize / 16;
            int basketGroupWidth = Board.BasketsPerPlayer * basketSize + (Board.BasketsPerPlayer - 1) * basketGap;
            int basketGroupStartX = (boardWidth - basketGroupWidth) / 2;

            for (int columnIndex = 0; columnIndex < Board.BasketsPerPlayer; columnIndex++)
            {
                int basketX = basketGroupStartX + columnIndex * (basketSize + basketGap);

                // P2 baskets (top row): sprite at top, label below.
                player2Baskets[columnIndex].Location = new Point2D(basketX, 0);
                player2Baskets[columnIndex].Size = new Size2D(basketSize, basketControlHeight);

                // P1 baskets (bottom row): label above, sprite at bottom.
                player1Baskets[columnIndex].Location = new Point2D(basketX, boardHeight - basketControlHeight);
                player1Baskets[columnIndex].Size = new Size2D(basketSize, basketControlHeight);
            }
        }

        private void UpdateControls()
        {
            player1Store.WalnutCount = board.GetWalnuts(player1Store.BoardIndex);
            player2Store.WalnutCount = board.GetWalnuts(player2Store.BoardIndex);

            for (int columnIndex = 0; columnIndex < Board.BasketsPerPlayer; columnIndex++)
            {
                GuiBasket player1Basket = player1Baskets[columnIndex];
                GuiBasket player2Basket = player2Baskets[columnIndex];

                player1Basket.WalnutCount = board.GetWalnuts(player1Basket.BoardIndex);
                player1Basket.IsSelectable = board.IsMoveAllowed(player1Basket.BoardIndex);

                player2Basket.WalnutCount = board.GetWalnuts(player2Basket.BoardIndex);
                player2Basket.IsSelectable = board.IsMoveAllowed(player2Basket.BoardIndex);
            }
        }

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!Equals(mouseButtonEventArgs.Button, MouseButton.Left))
            {
                return;
            }

            for (int columnIndex = 0; columnIndex < Board.BasketsPerPlayer; columnIndex++)
            {
                GuiBasket player1Basket = player1Baskets[columnIndex];
                GuiBasket player2Basket = player2Baskets[columnIndex];

                if (player1Basket.DisplayRectangle.Contains(mouseButtonEventArgs.Location))
                {
                    if (board.IsMoveAllowed(player1Basket.BoardIndex))
                    {
                        board.Move(player1Basket.BoardIndex);
                    }

                    return;
                }

                if (player2Basket.DisplayRectangle.Contains(mouseButtonEventArgs.Location))
                {
                    if (board.IsMoveAllowed(player2Basket.BoardIndex))
                    {
                        board.Move(player2Basket.BoardIndex);
                    }

                    return;
                }
            }
        }
    }
}
