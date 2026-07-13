using Nucumi.Model;

namespace Nucumi.GameLogic
{
    internal sealed class BoardAi
    {
        private static int SearchDepth => 5;

        public int ChooseMove(Board board)
        {
            int bestMove = Board.Player2BasketStartIndex;
            int bestScore = int.MinValue;

            for (int basketIndex = Board.Player2BasketStartIndex; basketIndex <= Board.Player2LastBasketIndex; basketIndex++)
            {
                if (!board.IsMoveAllowed(basketIndex))
                {
                    continue;
                }

                Board simulatedBoard = board.Clone();
                simulatedBoard.Move(basketIndex);

                bool isNextMoveMaximising = Equals(simulatedBoard.CurrentPlayer, Player.Player2);
                int score = Minimax(simulatedBoard, SearchDepth - 1, isNextMoveMaximising, int.MinValue, int.MaxValue);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = basketIndex;
                }
            }

            return bestMove;
        }

        private int Minimax(Board board, int depth, bool isMaximisingPlayer, int alpha, int beta)
        {
            if (depth == 0 || Equals(board.Phase, GamePhase.GameOver))
            {
                return Evaluate(board);
            }

            if (isMaximisingPlayer)
            {
                int maximumScore = int.MinValue;

                for (int basketIndex = Board.Player2BasketStartIndex; basketIndex <= Board.Player2LastBasketIndex; basketIndex++)
                {
                    if (!board.IsMoveAllowed(basketIndex))
                    {
                        continue;
                    }

                    Board simulatedBoard = board.Clone();
                    simulatedBoard.Move(basketIndex);

                    bool isNextMoveMaximising = Equals(simulatedBoard.CurrentPlayer, Player.Player2);
                    int score = Minimax(simulatedBoard, depth - 1, isNextMoveMaximising, alpha, beta);

                    if (score > maximumScore)
                    {
                        maximumScore = score;
                    }

                    if (maximumScore > alpha)
                    {
                        alpha = maximumScore;
                    }

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return maximumScore;
            }
            else
            {
                int minimumScore = int.MaxValue;

                for (int basketIndex = 0; basketIndex < Board.BasketsPerPlayer; basketIndex++)
                {
                    if (!board.IsMoveAllowed(basketIndex))
                    {
                        continue;
                    }

                    Board simulatedBoard = board.Clone();
                    simulatedBoard.Move(basketIndex);

                    bool isNextMoveMaximising = Equals(simulatedBoard.CurrentPlayer, Player.Player2);
                    int score = Minimax(simulatedBoard, depth - 1, isNextMoveMaximising, alpha, beta);

                    if (score < minimumScore)
                    {
                        minimumScore = score;
                    }

                    if (minimumScore < beta)
                    {
                        beta = minimumScore;
                    }

                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return minimumScore;
            }
        }

        private int Evaluate(Board board)
        {
            int score = board.GetWalnuts(Board.Player2StoreIndex) - board.GetWalnuts(Board.Player1StoreIndex);

            for (int basketIndex = Board.Player2BasketStartIndex; basketIndex <= Board.Player2LastBasketIndex; basketIndex++)
            {
                score += board.GetImmediateCaptureValue(basketIndex) / 2;
            }

            for (int basketIndex = 0; basketIndex < Board.BasketsPerPlayer; basketIndex++)
            {
                score -= board.GetImmediateCaptureValue(basketIndex) / 2;
            }

            return score;
        }
    }
}
