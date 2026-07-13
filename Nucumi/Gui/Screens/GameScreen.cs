using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using NuciXNA.Gui;
using NuciXNA.Gui.Screens;
using NuciXNA.Input;
using NuciXNA.Primitives;

using Nucumi.GameLogic;
using Nucumi.Gui.Controls;
using Nucumi.Model;

namespace Nucumi.Gui.Screens
{
    internal sealed class GameScreen : Screen
    {
        private static int ButtonBarHeight => 64;
        private static double AiMoveDelayMilliseconds => 1000;

        private readonly Board board;
        private readonly BoardAi boardAi;
        private TimeSpan aiMoveScheduledAt;
        private GuiButton undoButton;
        private GuiButton restartButton;
        private GuiButton infoButton;
        private GuiButton settingsButton;
        private GuiGameBoard gameBoard;
        private GuiStatusBar statusBar;

        public GameScreen()
        {
            BackgroundColour = new Colour(20, 15, 10);
            board = new Board();
            boardAi = new BoardAi();
        }

        protected override void DoLoadContent()
        {
            undoButton = new GuiButton { ButtonType = ButtonType.Undo };
            restartButton = new GuiButton { ButtonType = ButtonType.Restart };
            infoButton = new GuiButton { ButtonType = ButtonType.Info };
            settingsButton = new GuiButton { ButtonType = ButtonType.Settings };
            gameBoard = new GuiGameBoard(board);
            statusBar = new GuiStatusBar();

            GuiManager.Instance.RegisterControls(gameBoard);
            GuiManager.Instance.RegisterControls(undoButton, restartButton, infoButton, settingsButton, statusBar);
            RegisterEvents();
            SetChildrenProperties();
        }

        protected override void DoUnloadContent() => UnregisterEvents();

        protected override void DoUpdate(GameTime gameTime)
        {
            SetChildrenProperties();

            if (Equals(board.Phase, GamePhase.InProgress) && Equals(board.CurrentPlayer, Player.Player2))
            {
                if (aiMoveScheduledAt == TimeSpan.Zero)
                {
                    aiMoveScheduledAt = gameTime.TotalGameTime + TimeSpan.FromMilliseconds(AiMoveDelayMilliseconds);
                }
                else if (gameTime.TotalGameTime >= aiMoveScheduledAt)
                {
                    int aiMove = boardAi.ChooseMove(board);

                    if (board.IsMoveAllowed(aiMove))
                    {
                        board.Move(aiMove);
                    }

                    aiMoveScheduledAt = TimeSpan.Zero;
                }
            }
            else
            {
                aiMoveScheduledAt = TimeSpan.Zero;
            }
        }

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

            statusBar.Location = new Point2D(2 * ButtonBarHeight, 0);
            statusBar.Size = new Size2D(infoButtonX - 2 * ButtonBarHeight, ButtonBarHeight);
            statusBar.Text = BuildStatusText();

            gameBoard.Location = new Point2D(0, ButtonBarHeight);
            gameBoard.Size = new Size2D(
                ScreenManager.Instance.Size.Width,
                ScreenManager.Instance.Size.Height - ButtonBarHeight);
        }

        private string BuildStatusText()
        {
            if (!Equals(board.Phase, GamePhase.GameOver))
            {
                string currentPlayerName = "You";

                if (Equals(board.CurrentPlayer, Player.Player2))
                {
                    currentPlayerName = "Computer";
                }

                return $"{currentPlayerName}'s turn";
            }

            int player1Score = board.GetWalnuts(Board.Player1StoreIndex);
            int player2Score = board.GetWalnuts(Board.Player2StoreIndex);

            if (player1Score > player2Score)
            {
                return $"Game over — You win! ({player1Score} vs {player2Score})  |  Press R to restart";
            }

            if (player2Score > player1Score)
            {
                return $"Game over — Computer wins! ({player2Score} vs {player1Score})  |  Press R to restart";
            }

            return $"Game over — Draw! ({player1Score} each)  |  Press R to restart";
        }

        private void OnRestartButtonClicked(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            board.Reset();
            aiMoveScheduledAt = TimeSpan.Zero;
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
