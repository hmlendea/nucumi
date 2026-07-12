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
        private static int BasketSize => 143;
        private static int HorizontalGap => 17;
        private static int RowGap => 477;
        private static int ColumnWidth => BasketSize + HorizontalGap;
        private static int StoreVerticalOffset => BasketSize + (RowGap - BasketSize) / 2;

        public static int TotalWidth => 2 * BasketSize + HorizontalGap + Board.BasketsPerPlayer * ColumnWidth;
        public static int TotalHeight => 2 * BasketSize + RowGap;

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
                Location = new Point2D(0, StoreVerticalOffset),
                Size = new Size2D(BasketSize, BasketSize)
            };

            player1Store = new GuiStore
            {
                Location = new Point2D(BasketSize + HorizontalGap + Board.BasketsPerPlayer * ColumnWidth, StoreVerticalOffset),
                Size = new Size2D(BasketSize, BasketSize)
            };

            for (int columnIndex = 0; columnIndex < Board.BasketsPerPlayer; columnIndex++)
            {
                int basketX = BasketSize + HorizontalGap + columnIndex * ColumnWidth;

                player2Baskets[columnIndex] = new GuiBasket
                {
                    BoardIndex = 12 - columnIndex,
                    Location = new Point2D(basketX, 0),
                    Size = new Size2D(BasketSize, BasketSize)
                };

                player1Baskets[columnIndex] = new GuiBasket
                {
                    BoardIndex = columnIndex,
                    Location = new Point2D(basketX, BasketSize + RowGap),
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
            InputManager.Instance.MouseButtonPressed += OnMouseButtonPressed;
            UpdateControls();
        }

        protected override void DoUnloadContent()
            => InputManager.Instance.MouseButtonPressed -= OnMouseButtonPressed;

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

        private void OnMouseButtonPressed(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (!mouseButtonEventArgs.Button.Equals(MouseButton.Left))
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
