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

namespace Nucumi.Gui.Screens
{
    internal sealed class GameScreen : Screen
    {
        private static int ButtonBarHeight => 64;

        private readonly Board board;
        private GuiButton undoButton;
        private GuiButton restartButton;
        private GuiButton infoButton;
        private GuiButton settingsButton;
        private GuiGameBoard gameBoard;
        private GuiText statusText;

        public GameScreen()
        {
            BackgroundColour = new Colour(20, 15, 10);
            board = new Board();
        }

        protected override void DoLoadContent()
        {
            undoButton = new GuiButton { ButtonType = ButtonType.Undo };
            restartButton = new GuiButton { ButtonType = ButtonType.Restart };
            infoButton = new GuiButton { ButtonType = ButtonType.Info };
            settingsButton = new GuiButton { ButtonType = ButtonType.Settings };
            gameBoard = new GuiGameBoard(board);
            statusText = new GuiText { FontName = "DefaultFont" };

            GuiManager.Instance.RegisterControls(undoButton, restartButton, infoButton, settingsButton, gameBoard, statusText);
            RegisterEvents();
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() => UnregisterEvents();

        protected override void DoUpdate(GameTime gameTime) => SetChildrenProperties();

        protected override void DoDraw(SpriteBatch spriteBatch) { }

        private void RegisterEvents()
        {
            InputManager.Instance.KeyboardKeyPressed += OnKeyboardKeyPressed;
            restartButton.Clicked += OnRestartButtonClicked;
        }

        private void UnregisterEvents()
        {
            InputManager.Instance.KeyboardKeyPressed -= OnKeyboardKeyPressed;
            restartButton.Clicked -= OnRestartButtonClicked;
        }

        private void SetChildrenProperties()
        {
            int screenWidth = ScreenManager.Instance.Size.Width;
            int settingsButtonX = screenWidth - ButtonBarHeight;
            int infoButtonX = settingsButtonX - ButtonBarHeight;

            undoButton.Location = new Point2D(0, 0);
            undoButton.Size = new Size2D(ButtonBarHeight, ButtonBarHeight);

            restartButton.Location = new Point2D(ButtonBarHeight, 0);
            restartButton.Size = new Size2D(ButtonBarHeight, ButtonBarHeight);

            infoButton.Location = new Point2D(infoButtonX, 0);
            infoButton.Size = new Size2D(ButtonBarHeight, ButtonBarHeight);

            settingsButton.Location = new Point2D(settingsButtonX, 0);
            settingsButton.Size = new Size2D(ButtonBarHeight, ButtonBarHeight);

            gameBoard.Location = new Point2D(0, ButtonBarHeight);
            gameBoard.Size = new Size2D(
                ScreenManager.Instance.Size.Width,
                ScreenManager.Instance.Size.Height - ButtonBarHeight);

            statusText.Location = new Point2D(2 * ButtonBarHeight, 0);
            statusText.Size = new Size2D(infoButtonX - 2 * ButtonBarHeight, ButtonBarHeight);
            statusText.HorizontalAlignment = Alignment.Middle;
            statusText.VerticalAlignment = Alignment.Middle;
            statusText.ForegroundColour = Colour.White;
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

        private void OnRestartButtonClicked(object sender, MouseButtonEventArgs mouseButtonEventArgs)
            => board.Reset();

        private void OnKeyboardKeyPressed(object sender, KeyboardKeyEventArgs keyEventArgs)
        {
            if (Equals(keyEventArgs.Key, Keys.R))
            {
                board.Reset();
            }
        }
    }
}
