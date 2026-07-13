using System.Collections.Generic;

using Nucumi.Model;

namespace Nucumi.GameLogic
{
    internal sealed class BoardAi
    {
        private static int SearchDepth => 6;

        // Positional bonus for having an extra-turn move available.
        // An extra turn is worth roughly 3–4 seeds of positional advantage.
        private static int ExtraTurnPositionalBonus => 3;

        public int ChooseMove(Board board)
        {
            int bestMove = Board.Player2BasketStartIndex;
            int bestScore = int.MinValue;

            foreach (int basketIndex in GetOrderedMoves(board, isMaximisingPlayer: true))
            {
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

                foreach (int basketIndex in GetOrderedMoves(board, isMaximisingPlayer: true))
                {
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

                foreach (int basketIndex in GetOrderedMoves(board, isMaximisingPlayer: false))
                {
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

        // Returns moves ordered: extra-turn moves first, then captures, then all others.
        // Better ordering allows alpha-beta pruning to cut far more branches.
        private IEnumerable<int> GetOrderedMoves(Board board, bool isMaximisingPlayer)
        {
            int startIndex = isMaximisingPlayer ? Board.Player2BasketStartIndex : 0;
            int endIndex = isMaximisingPlayer ? Board.Player2LastBasketIndex : Board.BasketsPerPlayer - 1;

            List<int> extraTurnMoves = [];
            List<int> captureMoves = [];
            List<int> otherMoves = [];

            for (int basketIndex = startIndex; basketIndex <= endIndex; basketIndex++)
            {
                if (!board.IsMoveAllowed(basketIndex))
                {
                    continue;
                }

                if (board.IsExtraTurnMove(basketIndex))
                {
                    extraTurnMoves.Add(basketIndex);
                }
                else if (board.GetImmediateCaptureValue(basketIndex) > 0)
                {
                    captureMoves.Add(basketIndex);
                }
                else
                {
                    otherMoves.Add(basketIndex);
                }
            }

            List<int> orderedMoves = [.. extraTurnMoves, .. captureMoves, .. otherMoves];

            return orderedMoves;
        }

        private int Evaluate(Board board)
        {
            int score = board.GetWalnuts(Board.Player2StoreIndex) - board.GetWalnuts(Board.Player1StoreIndex);

            for (int basketIndex = Board.Player2BasketStartIndex; basketIndex <= Board.Player2LastBasketIndex; basketIndex++)
            {
                if (board.IsExtraTurnMove(basketIndex))
                {
                    score += ExtraTurnPositionalBonus;
                }

                score += board.GetImmediateCaptureValue(basketIndex);
            }

            for (int basketIndex = 0; basketIndex < Board.BasketsPerPlayer; basketIndex++)
            {
                if (board.IsExtraTurnMove(basketIndex))
                {
                    score -= ExtraTurnPositionalBonus;
                }

                score -= board.GetImmediateCaptureValue(basketIndex);
            }

            return score;
        }
    }
}
