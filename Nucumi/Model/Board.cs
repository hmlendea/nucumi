using System;

namespace Nucumi.Model
{
    public sealed class Board
    {
        public static int TotalPositionCount => 14;

        public static int Player1StoreIndex => 6;

        public static int Player2StoreIndex => 13;

        public static int BasketsPerPlayer => 6;

        public static int InitialWalnutsPerBasket => 4;

        public static int Player2BasketStartIndex => Player1StoreIndex + 1;

        public static int Player2LastBasketIndex => Player2StoreIndex - 1;

        // Counter-clockwise distribution path for Player 1 (skips Player 2's store at 13).
        private static readonly int[] Player1DistributionSequence = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];

        // Counter-clockwise distribution path for Player 2 (skips Player 1's store at 6).
        private static readonly int[] Player2DistributionSequence = [0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12, 13];

        private readonly int[] walnutsAtPosition;

        public GamePhase Phase { get; private set; }

        public Player CurrentPlayer { get; private set; }

        public Board()
        {
            walnutsAtPosition = new int[TotalPositionCount];
            Reset();
        }

        private Board(int[] existingWalnutsAtPosition, GamePhase phase, Player currentPlayer)
        {
            walnutsAtPosition = (int[])existingWalnutsAtPosition.Clone();
            Phase = phase;
            CurrentPlayer = currentPlayer;
        }

        public Board Clone() => new(walnutsAtPosition, Phase, CurrentPlayer);

        public int GetWalnuts(int positionIndex) => walnutsAtPosition[positionIndex];

        public int GetImmediateCaptureValue(int basketIndex)
        {
            int walnuts = walnutsAtPosition[basketIndex];

            if (walnuts == 0)
            {
                return 0;
            }

            bool isPlayer1Basket = basketIndex < BasketsPerPlayer;
            int[] sequence = isPlayer1Basket ? Player1DistributionSequence : Player2DistributionSequence;
            int startIndexInSequence = Array.IndexOf(sequence, basketIndex);
            int landingPosition = sequence[(startIndexInSequence + walnuts) % sequence.Length];

            bool isOwnEmptyBasket;

            if (isPlayer1Basket)
            {
                isOwnEmptyBasket = landingPosition < BasketsPerPlayer && walnutsAtPosition[landingPosition] == 0;
            }
            else
            {
                isOwnEmptyBasket = landingPosition >= Player2BasketStartIndex
                    && landingPosition <= Player2LastBasketIndex
                    && walnutsAtPosition[landingPosition] == 0;
            }

            if (!isOwnEmptyBasket)
            {
                return 0;
            }

            int oppositePosition = Player2LastBasketIndex - landingPosition;
            int oppositeWalnuts = walnutsAtPosition[oppositePosition];

            if (oppositeWalnuts == 0)
            {
                return 0;
            }

            return oppositeWalnuts + 1;
        }

        public bool IsMoveAllowed(int basketIndex)
        {
            if (!Equals(Phase, GamePhase.InProgress))
            {
                return false;
            }

            if (Equals(CurrentPlayer, Player.Player1))
            {
                return basketIndex >= 0 && basketIndex < BasketsPerPlayer && walnutsAtPosition[basketIndex] > 0;
            }

            return basketIndex >= Player2BasketStartIndex && basketIndex <= Player2LastBasketIndex && walnutsAtPosition[basketIndex] > 0;
        }

        public void Reset()
        {
            for (int positionIndex = 0; positionIndex < TotalPositionCount; positionIndex++)
            {
                walnutsAtPosition[positionIndex] = 0;
            }

            for (int basketIndex = 0; basketIndex < BasketsPerPlayer; basketIndex++)
            {
                walnutsAtPosition[basketIndex] = InitialWalnutsPerBasket;
                walnutsAtPosition[Player2BasketStartIndex + basketIndex] = InitialWalnutsPerBasket;
            }

            Phase = GamePhase.InProgress;
            CurrentPlayer = Player.Player1;
        }

        public void Move(int basketIndex)
        {
            if (!IsMoveAllowed(basketIndex))
            {
                throw new InvalidOperationException($"Cannot move from position {basketIndex}.");
            }

            int[] distributionSequence = Player1DistributionSequence;

            if (Equals(CurrentPlayer, Player.Player2))
            {
                distributionSequence = Player2DistributionSequence;
            }

            int walnutsToDistribute = walnutsAtPosition[basketIndex];
            walnutsAtPosition[basketIndex] = 0;
            int startIndexInSequence = Array.IndexOf(distributionSequence, basketIndex);
            int lastLandedPosition = basketIndex;

            for (int distributionStep = 0; distributionStep < walnutsToDistribute; distributionStep++)
            {
                int sequencePosition = (startIndexInSequence + 1 + distributionStep) % distributionSequence.Length;
                int targetPosition = distributionSequence[sequencePosition];
                walnutsAtPosition[targetPosition] += 1;
                lastLandedPosition = targetPosition;
            }

            ProcessTurnEnd(lastLandedPosition);
        }

        private void ProcessTurnEnd(int lastLandedPosition)
        {
            bool hasLandedInOwnStore = Equals(lastLandedPosition, Player1StoreIndex);

            if (Equals(CurrentPlayer, Player.Player2))
            {
                hasLandedInOwnStore = Equals(lastLandedPosition, Player2StoreIndex);
            }

            if (hasLandedInOwnStore)
            {
                CheckGameOver();

                return;
            }

            bool hasLandedInOwnEmptyBasket = lastLandedPosition >= 0
                && lastLandedPosition < BasketsPerPlayer
                && Equals(walnutsAtPosition[lastLandedPosition], 1);

            if (Equals(CurrentPlayer, Player.Player2))
            {
                hasLandedInOwnEmptyBasket = lastLandedPosition >= Player2BasketStartIndex
                    && lastLandedPosition <= Player2LastBasketIndex
                    && Equals(walnutsAtPosition[lastLandedPosition], 1);
            }

            if (hasLandedInOwnEmptyBasket)
            {
                int oppositePosition = Player2LastBasketIndex - lastLandedPosition;

                if (walnutsAtPosition[oppositePosition] > 0)
                {
                    int capturedWalnuts = walnutsAtPosition[oppositePosition] + 1;
                    walnutsAtPosition[oppositePosition] = 0;
                    walnutsAtPosition[lastLandedPosition] = 0;

                    int playerStoreIndex = Player1StoreIndex;

                    if (Equals(CurrentPlayer, Player.Player2))
                    {
                        playerStoreIndex = Player2StoreIndex;
                    }

                    walnutsAtPosition[playerStoreIndex] += capturedWalnuts;
                }
            }

            CheckGameOver();

            if (!Equals(Phase, GamePhase.GameOver))
            {
                Player nextPlayer = Player.Player2;

                if (Equals(CurrentPlayer, Player.Player2))
                {
                    nextPlayer = Player.Player1;
                }

                CurrentPlayer = nextPlayer;
            }
        }

        private void CheckGameOver()
        {
            bool isPlayer1SideEmpty = true;
            bool isPlayer2SideEmpty = true;

            for (int basketIndex = 0; basketIndex < BasketsPerPlayer; basketIndex++)
            {
                if (walnutsAtPosition[basketIndex] > 0)
                {
                    isPlayer1SideEmpty = false;
                }

                if (walnutsAtPosition[Player2BasketStartIndex + basketIndex] > 0)
                {
                    isPlayer2SideEmpty = false;
                }
            }

            if (!isPlayer1SideEmpty && !isPlayer2SideEmpty)
            {
                return;
            }

            for (int basketIndex = 0; basketIndex < BasketsPerPlayer; basketIndex++)
            {
                walnutsAtPosition[Player1StoreIndex] += walnutsAtPosition[basketIndex];
                walnutsAtPosition[basketIndex] = 0;
                walnutsAtPosition[Player2StoreIndex] += walnutsAtPosition[Player2BasketStartIndex + basketIndex];
                walnutsAtPosition[Player2BasketStartIndex + basketIndex] = 0;
            }

            Phase = GamePhase.GameOver;
        }
    }
}
