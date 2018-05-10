namespace TexasHoldem.AI.NeuroPlayer.NeuralNetwork
{
    using System;
    using System.Linq;

    using HandEvaluatorExtension;
    using HoldemHand;
    using TexasHoldem.Logic.Players;

    // Some materials are taken from here
    // Opponent Modeling in Poker: Learning and Acting in a Hostile and Uncertain Environment (2002)
    // by Aaron Davidson
    public class InputData
    {
        private static object locker = new object();

        public InputData(string playerName)
        {
            this.PlayerName = playerName;
        }

        public string PlayerName { get; }

        public IStartGameContext StartGameContext { get; set; }

        public IStartHandContext StartHandContext { get; set; }

        public IStartRoundContext StartRoundContext { get; set; }

        public IGetTurnContext TurnContext { get; set; }

        public CardAdapter PocketCards()
        {
            return new CardAdapter(new[] { this.StartHandContext.FirstCard, this.StartHandContext.SecondCard });
        }

        public CardAdapter CommunityCards()
        {
            return new CardAdapter(this.StartRoundContext.CommunityCards.ToList());
        }

        public double ImmediatePotOdds()
        {
            return (double)this.TurnContext.MoneyToCall
                / (double)(this.TurnContext.CurrentPot + this.TurnContext.MoneyToCall);
        }

        public double BetRatio()
        {
            var query = this.TurnContext.PreviousRoundActions.Where(p => p.Round == this.TurnContext.RoundType);
            var bets = query.Count(p => p.PlayerName == this.PlayerName && p.Action.Type == PlayerActionType.Raise);
            var calls = query.SkipWhile(p => p.Action.Type != PlayerActionType.Raise)
                .Count(p => p.PlayerName == this.PlayerName && p.Action.Type == PlayerActionType.CheckCall);
            return (double)bets / (double)(bets + calls);
        }

        public bool HasPutMoneyInThePotThisRound()
        {
            return this.TurnContext.MyMoneyInTheRound > 0;
        }

        public int AmountBetToCall()
        {
            return this.TurnContext.PreviousRoundActions
                .Where(p => p.Round == this.TurnContext.RoundType)
                .Count(p => p.Action.Type == PlayerActionType.Raise);
        }

        public bool OneBetToCall()
        {
            return this.AmountBetToCall() == 1;
        }

        public bool TwoOrMoreBetsToCall()
        {
            return this.AmountBetToCall() > 1;
        }

        public double[] Street()
        {
            switch (this.TurnContext.RoundType)
            {
                case Logic.GameRoundType.PreFlop:
                    return new double[] { 0, 0, 0 };
                case Logic.GameRoundType.Flop:
                    return new double[] { 1, 0, 0 };
                case Logic.GameRoundType.Turn:
                    return new double[] { 0, 1, 0 };
                case Logic.GameRoundType.River:
                    return new double[] { 0, 0, 1 };
                default:
                    throw new NotImplementedException();
            }
        }

        public bool LastBetsCalledByPlayer()
        {
            var query = this.TurnContext.PreviousRoundActions
                .Where(p => p.Round == this.TurnContext.RoundType)
                .Reverse();
            var lastActIsCall = false;

            foreach (var item in query)
            {
                if (!lastActIsCall)
                {
                    if (item.PlayerName == this.PlayerName && item.Action.Type == PlayerActionType.CheckCall)
                    {
                        lastActIsCall = true;
                    }
                }
                else
                {
                    if (item.PlayerName != this.PlayerName && item.Action.Type == PlayerActionType.Raise)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool LastActionOfPlayerWasBetOrRaise()
        {
            var query = this.TurnContext.PreviousRoundActions
                .Where(p => p.Round == this.TurnContext.RoundType && p.PlayerName == this.PlayerName)
                .Select(s => s.Action)
                .LastOrDefault();

            return query == null ? false : query.Type == PlayerActionType.Raise;
        }

        public bool IsHeadsUp()
        {
            return this.TurnContext.Opponents.Count(p => p.InHand) == 1;
        }

        public bool IsFirstToAct()
        {
            return this.TurnContext.Opponents
                .Where(p => p.InHand)
                .All(p => p.ActionPriority > this.StartHandContext.ActionPriority);
        }

        public bool IsLastToAct()
        {
            return this.TurnContext.Opponents
                .Where(p => p.InHand)
                .All(p => p.ActionPriority < this.StartHandContext.ActionPriority);
        }

        public double[] Position()
        {
            var vector = new double[this.StartGameContext.PlayerNames.Count];
            vector[this.StartHandContext.ActionPriority] = 1.0;
            return vector;
        }

        public double[] MatrixOfOdds()
        {
            double[] wins;
            double[] losses;

            if (this.TurnContext.RoundType == Logic.GameRoundType.PreFlop)
            {
                lock (locker)
                {
                    // We use the lock since in the third-party library "HandEvaluator"
                    // the method to which this method refers uses the common field
                    Hand.HandWinOdds(this.PocketCards().Mask, 0UL, out wins, out losses);
                }
            }
            else
            {
                HandEx.BalanceHandStrength(this.PocketCards().Mask, this.CommunityCards().Mask, out wins, out losses);
            }

            var combinedArray = new double[18];

            wins.CopyTo(combinedArray, 0);
            losses.CopyTo(combinedArray, 9);

            return combinedArray;
        }

        public bool IsStraightDraw()
        {
            return Hand.IsStraightDraw(this.PocketCards().Mask, this.CommunityCards().Mask, 0UL);
        }

        public bool IsFlushDraw()
        {
            return Hand.IsStraightDraw(this.PocketCards().Mask, this.CommunityCards().Mask, 0UL);
        }

        public double[] IntToBinaryCode(int number, int arrayLength)
        {
            string convertedString = Convert.ToString(number, 2);

            double[] bitArray = convertedString.PadLeft(arrayLength, '0')
                .Select(c => double.Parse(c.ToString()))
                .ToArray();

            return bitArray;
        }
    }
}
