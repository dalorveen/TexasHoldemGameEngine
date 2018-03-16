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
    }
}
