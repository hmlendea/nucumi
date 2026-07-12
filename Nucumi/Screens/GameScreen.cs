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
        private readonly Board board;
        private GuiImage backgroundImage;
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
            backgroundImage = new GuiImage { ContentFile = "board/background" };
            boardControl = new GuiGameBoard(board);
            player1Label = new GuiText { FontName = "DefaultFont" };
            player2Label = new GuiText { FontName = "DefaultFont" };
            statusText = new GuiText { FontName = "DefaultFont" };

            GuiManager.Instance.RegisterControls(backgroundImage, boardControl, player1Label, player2Label, statusText);
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

            // Proportions derived from the carpet border positions in the reference 1536×1024 image:
            // inner field left=136, top=130, width=1263, height=763.
            int boardX = screenWidth * 136 / 1536;
            int boardY = screenHeight * 130 / 1024;
            int boardWidth = screenWidth * 1263 / 1536;
            int boardHeight = screenHeight * 763 / 1024;
            int labelHeight = screenHeight * 36 / 1024;
            int statusHeight = screenHeight * 36 / 1024;

            backgroundImage.Location = Point2D.Empty;
            backgroundImage.Size = new Size2D(screenWidth, screenHeight);

            boardControl.Location = new Point2D(boardX, boardY);
            boardControl.Size = new Size2D(boardWidth, boardHeight);

            player2Label.Location = new Point2D(boardX, boardY - labelHeight - 8);
            player2Label.Size = new Size2D(boardWidth, labelHeight);
            player2Label.HorizontalAlignment = Alignment.Middle;
            player2Label.VerticalAlignment = Alignment.Middle;
            player2Label.ForegroundColour = Colour.Black;
            player2Label.Text = "Player 2";

            player1Label.Location = new Point2D(boardX, boardY + boardHeight + 8);
            player1Label.Size = new Size2D(boardWidth, labelHeight);
            player1Label.HorizontalAlignment = Alignment.Middle;
            player1Label.VerticalAlignment = Alignment.Middle;
            player1Label.ForegroundColour = Colour.Black;
            player1Label.Text = "Player 1";

            statusText.Location = new Point2D(0, screenHeight - statusHeight - 10);
            statusText.Size = new Size2D(screenWidth, statusHeight);
            statusText.HorizontalAlignment = Alignment.Middle;
            statusText.VerticalAlignment = Alignment.Middle;
            statusText.ForegroundColour = Colour.Black;
            statusText.Text = BuildStatusText();
        }

        private string BuildStatusText()
        {
            if (!Equals(board.Phase, GamePhase.GameOver))
            {
                string currentPlayerName = "Player 1";

                if (Equals(board.CurrentPlayer, Player.Player2))
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
            if (Equals(keyEventArgs.Key, Keys.R))
            {
                board.Reset();
            }
        }
    }
}
