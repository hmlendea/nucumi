using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NuciXNA.Graphics.Drawing;
using NuciXNA.Gui;
using NuciXNA.Gui.Controls;
using NuciXNA.Gui.Screens;
using NuciXNA.Input;
using NuciXNA.Primitives;
using Nucumi.Gui.Controls;
using Nucumi.Model;

namespace Nucumi.Screens
{
    internal sealed class GameScreen : Screen
    {
        private static Colour Player1Colour => new(220, 120, 40);
        private static Colour Player2Colour => new(70, 130, 220);

        private readonly Board board;
        private GuiGameBoard boardControl;
        private GuiText player1Label;
        private GuiText player2Label;
        private GuiText statusText;

        public GameScreen()
        {
            BackgroundColour = new Colour(20, 15, 10);
            board = new Board();
        }

        protected override void DoLoadContent()
        {
            boardControl = new GuiGameBoard(board);
            player1Label = new GuiText { FontName = "DefaultFont" };
            player2Label = new GuiText { FontName = "DefaultFont" };
            statusText = new GuiText { FontName = "DefaultFont" };

            GuiManager.Instance.RegisterControls(boardControl, player1Label, player2Label, statusText);
            RegisterEvents();
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() => UnregisterEvents();

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void RegisterEvents() => InputManager.Instance.KeyboardKeyPressed += OnKeyboardKeyPressed;

        private void UnregisterEvents() => InputManager.Instance.KeyboardKeyPressed -= OnKeyboardKeyPressed;

        private void SetChildrenProperties()
        {
            int screenWidth = ScreenManager.Instance.Size.Width;
            int screenHeight = ScreenManager.Instance.Size.Height;
            int boardWidth = GuiGameBoard.TotalWidth;
            int boardHeight = GuiGameBoard.TotalHeight;
            int boardX = (screenWidth - boardWidth) / 2;
            int boardY = (screenHeight - boardHeight) / 2;
            int labelHeight = 36;
            int statusHeight = 36;

            boardControl.Location = new Point2D(boardX, boardY);
            boardControl.Size = new Size2D(boardWidth, boardHeight);

            player2Label.Location = new Point2D(boardX, boardY - labelHeight - 8);
            player2Label.Size = new Size2D(boardWidth, labelHeight);
            player2Label.HorizontalAlignment = Alignment.Middle;
            player2Label.VerticalAlignment = Alignment.Middle;
            player2Label.ForegroundColour = Player2Colour;
            player2Label.Text = "Player 2";

            player1Label.Location = new Point2D(boardX, boardY + boardHeight + 8);
            player1Label.Size = new Size2D(boardWidth, labelHeight);
            player1Label.HorizontalAlignment = Alignment.Middle;
            player1Label.VerticalAlignment = Alignment.Middle;
            player1Label.ForegroundColour = Player1Colour;
            player1Label.Text = "Player 1";

            statusText.Location = new Point2D(0, screenHeight - statusHeight - 10);
            statusText.Size = new Size2D(screenWidth, statusHeight);
            statusText.HorizontalAlignment = Alignment.Middle;
            statusText.VerticalAlignment = Alignment.Middle;
            statusText.ForegroundColour = Colour.White;
            statusText.Text = BuildStatusText();
        }

        private string BuildStatusText()
        {
            if (!board.Phase.Equals(GamePhase.GameOver))
            {
                string currentPlayerName = "Player 1";

                if (board.CurrentPlayer.Equals(Player.Player2))
                {
                    currentPlayerName = "Player 2";
                }

                return $"{currentPlayerName}'s turn";
            }

            int player1Score = board.GetWalnuts(Board.Player1StoreIndex);
            int player2Score = board.GetWalnuts(Board.Player2StoreIndex);

            if (player1Score > player2Score)
            {
                return $"Game over — Player 1 wins! ({player1Score} vs {player2Score})  |  Press R to restart";
            }

            if (player2Score > player1Score)
            {
                return $"Game over — Player 2 wins! ({player2Score} vs {player1Score})  |  Press R to restart";
            }

            return $"Game over — Draw! ({player1Score} each)  |  Press R to restart";
        }

        private void OnKeyboardKeyPressed(object sender, KeyboardKeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.Key.Equals(Keys.R))
            {
                board.Reset();
            }
        }
    }
}
