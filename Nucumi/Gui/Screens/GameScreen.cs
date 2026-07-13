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
        // Pixel measurements within the board/background.png reference texture (1536×1024).
        private static int ReferenceBackgroundWidth => 1536;
        private static int ReferenceBackgroundHeight => 1024;
        private static int BackgroundInnerFieldLeft => 136;
        private static int BackgroundInnerFieldWidth => 1263;
        private static int BackgroundInnerFieldHeight => 763;
        private static int BackgroundLabelBarHeight => 36;

        // Layout constants.
        private static int ButtonBarHeight => 64;
        private static int LabelBoardGap => 8;
        private static int StatusBarBottomMargin => 10;

        private readonly Board board;
        private GuiButton undoButton;
        private GuiButton restartButton;
        private GuiButton infoButton;
        private GuiButton settingsButton;
        private GuiGameBoard gameBoard;
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
            undoButton = new GuiButton { ButtonType = ButtonType.Undo };
            restartButton = new GuiButton { ButtonType = ButtonType.Restart };
            infoButton = new GuiButton { ButtonType = ButtonType.Info };
            settingsButton = new GuiButton { ButtonType = ButtonType.Settings };
            gameBoard = new GuiGameBoard(board);
            player1Label = new GuiText { FontName = "DefaultFont" };
            player2Label = new GuiText { FontName = "DefaultFont" };
            statusText = new GuiText { FontName = "DefaultFont" };

            GuiManager.Instance.RegisterControls(undoButton, restartButton, infoButton, settingsButton, gameBoard, player1Label, player2Label, statusText);
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
            int screenHeight = ScreenManager.Instance.Size.Height;

            // Layout proportions from the carpet border positions in the reference image.
            int boardX = screenWidth * BackgroundInnerFieldLeft / ReferenceBackgroundWidth;
            int boardY = ButtonBarHeight;
            int boardWidth = screenWidth * BackgroundInnerFieldWidth / ReferenceBackgroundWidth;
            int boardHeight = screenHeight * BackgroundInnerFieldHeight / ReferenceBackgroundHeight;
            int labelHeight = screenHeight * BackgroundLabelBarHeight / ReferenceBackgroundHeight;
            int statusHeight = screenHeight * BackgroundLabelBarHeight / ReferenceBackgroundHeight;
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

            player2Label.Location = new Point2D(boardX, boardY - labelHeight - LabelBoardGap);
            player2Label.Size = new Size2D(boardWidth, labelHeight);
            player2Label.HorizontalAlignment = Alignment.Middle;
            player2Label.VerticalAlignment = Alignment.Middle;
            player2Label.ForegroundColour = Colour.Black;
            player2Label.Text = "Player 2";

            player1Label.Location = new Point2D(boardX, boardY + boardHeight + LabelBoardGap);
            player1Label.Size = new Size2D(boardWidth, labelHeight);
            player1Label.HorizontalAlignment = Alignment.Middle;
            player1Label.VerticalAlignment = Alignment.Middle;
            player1Label.ForegroundColour = Colour.Black;
            player1Label.Text = "Player 1";

            statusText.Location = new Point2D(0, screenHeight - statusHeight - StatusBarBottomMargin);
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
