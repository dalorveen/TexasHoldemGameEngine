namespace HandEvaluatorExtension
{
    using HoldemHand;

    public class HandEx
    {
        public static double HandWinOdds(ulong hero, ulong[] opponents, ulong dead, ulong board)
        {
            var skip = hero | dead;

            foreach (var item in opponents)
            {
                skip |= item;
            }

            var potsWon = 0.0;
            var games = 0.0;

            foreach (var nextBoard in HoldemHand.Hand.Hands(board, skip, 5))
            {
                var heroMaskValue = Hand.Evaluate(hero | nextBoard, 7);
                var greaterthan = true;
                var greaterthanequal = true;
                var tiesCount = 0.0;

                foreach (var nextOpponent in opponents)
                {
                    var opponentMaskValue = Hand.Evaluate(nextOpponent | nextBoard, 7);

                    if (heroMaskValue < opponentMaskValue)
                    {
                        greaterthan = greaterthanequal = false;
                        break;
                    }
                    else if (heroMaskValue == opponentMaskValue)
                    {
                        greaterthan = false;
                        tiesCount += 1.0;
                    }
                }

                if (greaterthan)
                {
                    potsWon += 1.0;
                }
                else if (greaterthanequal)
                {
                    potsWon += 1.0 / (tiesCount + 1.0);
                }

                games++;
            }

            return potsWon / games;
        }

        public static double HandStrengthInTheCurrentRound(ulong hero, ulong[] opponents, ulong board)
        {
            var heroMaskValue = Hand.Evaluate(hero | board);
            var win = 0;
            var tie = 0;
            var lose = 0;

            foreach (var opponent in opponents)
            {
                var opponentMaskValue = Hand.Evaluate(opponent | board);

                if (heroMaskValue > opponentMaskValue)
                {
                    win++;
                }
                else if (heroMaskValue == opponentMaskValue)
                {
                    tie++;
                }
                else
                {
                    lose++;
                }
            }

            return lose > 0 ? 0 : (tie > 0 ? 1.0 / (1 + tie) : 1.0);
        }

        public static void BalanceHandStrength(ulong hero, ulong board, out double[] wins, out double[] losses)
        {
            var count = 0.0;
            var heroHandValue = Hand.Evaluate(hero | board);

            wins = new double[9];
            losses = new double[9];

            foreach (var opponent in Hand.Hands(0UL, hero | board, 2))
            {
                var opponentHandValue = Hand.Evaluate(opponent | board);

                if (heroHandValue > opponentHandValue)
                {
                    wins[Hand.HandType(heroHandValue)] += 1.0;
                }
                else if (heroHandValue == opponentHandValue)
                {
                    wins[Hand.HandType(heroHandValue)] += 0.5;
                    losses[Hand.HandType(opponentHandValue)] += 0.5;
                }
                else
                {
                    losses[Hand.HandType(opponentHandValue)] += 1.0;
                }

                count++;
            }

            for (int i = 0; i < 9; i++)
            {
                wins[i] = wins[i] / count;
                losses[i] = losses[i] / count;
            }
        }
    }
}
