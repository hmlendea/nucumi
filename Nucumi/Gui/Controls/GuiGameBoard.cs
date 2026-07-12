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
        private static int BasketSize => 110;
        private static int StoreWidth => 110;
        private static int StoreHeight => 240;
        private static int GapSize => 20;
        private static int ColumnWidth => BasketSize + GapSize;

        public static int TotalWidth => 2 * StoreWidth + GapSize + Board.BasketsPerPlayer * ColumnWidth;
        public static int TotalHeight => 2 * BasketSize + GapSize;

        private readonly Board board;
        private readonly GuiBasket[] player1Baskets;
        private readonly GuiBasket[] player2Baskets;
        private GuiStore player1Store;
        private GuiStore player2Store;

        public GuiGameBoard(Board board)
        {
            this.board = board;
            player1Baskets = new GuiBasket[Board.BasketsPerPlayer];
            player2Baskets = new GuiBasket[Board.BasketsPerPlayer];
        }

        protected override void DoLoadContent()
        {
            List<IGuiControl> children = [];

            player2Store = new GuiStore
            {
                Location = Point2D.Empty,
                Size = new Size2D(StoreWidth, StoreHeight)
            };

            player1Store = new GuiStore
            {
                Location = new Point2D(StoreWidth + GapSize + Board.BasketsPerPlayer * ColumnWidth, 0),
                Size = new Size2D(StoreWidth, StoreHeight)
            };

            for (int columnIndex = 0; columnIndex < Board.BasketsPerPlayer; columnIndex++)
            {
                int basketX = StoreWidth + GapSize + columnIndex * ColumnWidth;

                player2Baskets[columnIndex] = new GuiBasket
                {
                    BoardIndex = 12 - columnIndex,
                    Location = new Point2D(basketX, 0),
                    Size = new Size2D(BasketSize, BasketSize)
                };

                player1Baskets[columnIndex] = new GuiBasket
                {
                    BoardIndex = columnIndex,
                    Location = new Point2D(basketX, BasketSize + GapSize),
                    Size = new Size2D(BasketSize, BasketSize)
                };
            }

            children.Add(player2Store);

            for (int basketIndex = 0; basketIndex < Board.BasketsPerPlayer; basketIndex++)
            {
                children.Add(player2Baskets[basketIndex]);
                children.Add(player1Baskets[basketIndex]);
            }

            children.Add(player1Store);

            RegisterChildren(children);

            for (int basketIndex = 0; basketIndex < Board.BasketsPerPlayer; basketIndex++)
            {
                player1Baskets[basketIndex].Clicked += OnBasketClicked;
                player2Baskets[basketIndex].Clicked += OnBasketClicked;
            }

            UpdateControls();
        }

        protected override void DoUnloadContent()
        {
            for (int basketIndex = 0; basketIndex < Board.BasketsPerPlayer; basketIndex++)
            {
                player1Baskets[basketIndex].Clicked -= OnBasketClicked;
                player2Baskets[basketIndex].Clicked -= OnBasketClicked;
            }
        }

        protected override void DoUpdate(GameTime gameTime) => UpdateControls();

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void UpdateControls()
        {
            player1Store.WalnutCount = board.GetWalnuts(Board.Player1StoreIndex);
            player1Store.IsCurrentPlayerStore = board.CurrentPlayer.Equals(Player.Player1);

            player2Store.WalnutCount = board.GetWalnuts(Board.Player2StoreIndex);
            player2Store.IsCurrentPlayerStore = board.CurrentPlayer.Equals(Player.Player2);

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

        private void OnBasketClicked(object sender, MouseButtonEventArgs clickEventArgs)
        {
            if (sender is not GuiBasket clickedBasket)
            {
                return;
            }

            if (!board.IsMoveAllowed(clickedBasket.BoardIndex))
            {
                return;
            }

            board.Move(clickedBasket.BoardIndex);
        }
    }
}
