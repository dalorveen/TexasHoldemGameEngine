namespace TexasHoldem.AI.NeuroPlayer.Normalization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using HandEvaluatorExtension;
    using HoldemHand;
    using TexasHoldem.Logic.Players;

    public class Normalization
    {
        private static object locker;

        private readonly int numberOfPlayers;

        private ICardAdapter pocket;

        private ICardAdapter communityCards;

        private IGetTurnContext getTurnContext;

        private int secondStack;

        private int excessInThePot;

        private int maxMoneyAtTheTable;

        private bool isUpdated;

        public Normalization(int numberOfPlayers)
        {
            this.numberOfPlayers = numberOfPlayers;
            this.isUpdated = false;
            locker = new object();
        }

        public void Update(ICardAdapter pocket, ICardAdapter communityCards, IGetTurnContext context)
        {
            var allChips = new List<int>();
            var activeOpponents = context.Opponents.Where(p => p.InHand);

            System.Diagnostics.Debug.Assert(activeOpponents.Count() > 0, "There are no active players.");

            var activeStacks = activeOpponents
                .Select(s => s.Money + s.CurrentRoundBet)
                .Concat(new int[] { context.MoneyLeft + context.MyMoneyInTheRound });
            var sortedStacks = activeStacks.OrderByDescending(k => k).Distinct().ToArray();

            this.excessInThePot = 0;

            if (sortedStacks.Count() == 1)
            {
                this.secondStack = sortedStacks[0];

                allChips.AddRange(activeOpponents.Select(s => s.Money));
                allChips.AddRange(activeOpponents.Select(s => s.CurrentRoundBet));
                allChips.AddRange(new[] { context.CurrentPot, context.MoneyLeft, context.MyMoneyInTheRound });
            }
            else
            {
                this.secondStack = sortedStacks[1];
                var currentPot = context.CurrentPot;

                if (context.MyMoneyInTheRound > this.secondStack)
                {
                    this.excessInThePot = context.MyMoneyInTheRound - this.secondStack;
                    currentPot = context.CurrentPot - this.excessInThePot;
                }
                else
                {
                    foreach (var item in activeOpponents)
                    {
                        if (item.CurrentRoundBet > this.secondStack)
                        {
                            this.excessInThePot = item.CurrentRoundBet - this.secondStack;
                            currentPot = context.CurrentPot - this.excessInThePot;

                            break;
                        }
                    }
                }

                allChips.Add(currentPot);
                allChips.Add(this.secondStack);
            }

            this.maxMoneyAtTheTable = allChips.Max(s => s);
            this.pocket = pocket;
            this.communityCards = communityCards;
            this.getTurnContext = context;
            this.isUpdated = true;
        }

        public ICollection<NormalizedOpponent> Opponents()
        {
            var normalizedOpponents = new List<NormalizedOpponent>();

            if (!this.isUpdated)
            {
                for (int i = 1; i < this.numberOfPlayers; i++)
                {
                    normalizedOpponents.Add(new NormalizedOpponent(0, 0, 0));
                }
            }
            else
            {
                var playersAfterMe = this.getTurnContext.Opponents.Where(p => p.ActionPriority > 0);
                var playersInFrontOfMe = this.getTurnContext.Opponents.Where(p => p.ActionPriority < 0);
                var opponents = playersAfterMe.Concat(playersInFrontOfMe);

                foreach (var item in opponents)
                {
                    if (item.InHand)
                    {
                        if (item.CurrentRoundBet > this.secondStack)
                        {
                            normalizedOpponents.Add(new NormalizedOpponent(
                                this.Normalize(0, this.maxMoneyAtTheTable, 0),
                                this.Normalize(0, this.maxMoneyAtTheTable, this.secondStack),
                                1));
                        }
                        else if (item.Money > this.secondStack)
                        {
                            normalizedOpponents.Add(new NormalizedOpponent(
                                this.Normalize(0, this.maxMoneyAtTheTable, this.secondStack - item.CurrentRoundBet),
                                this.Normalize(0, this.maxMoneyAtTheTable, item.CurrentRoundBet),
                                1));
                        }
                        else
                        {
                            normalizedOpponents.Add(new NormalizedOpponent(
                                this.Normalize(0, this.maxMoneyAtTheTable, item.Money),
                                this.Normalize(0, this.maxMoneyAtTheTable, item.CurrentRoundBet),
                                1));
                        }
                    }
                    else
                    {
                        normalizedOpponents.Add(new NormalizedOpponent(0, 0, 0));
                    }
                }
            }

            return normalizedOpponents;
        }

        public NormalizedHero Hero()
        {
            if (!this.isUpdated)
            {
                return new NormalizedHero(0, 0, 0);
            }
            else
            {
                if (this.getTurnContext.MyMoneyInTheRound > this.secondStack)
                {
                    return new NormalizedHero(
                        this.Normalize(0, this.maxMoneyAtTheTable, 0),
                        this.Normalize(0, this.maxMoneyAtTheTable, this.secondStack),
                        this.Normalize(-1, this.getTurnContext.Opponents.Count, this.getTurnContext.Opponents.Last().ActionPriority));
                }
                else if (this.getTurnContext.MoneyLeft > this.secondStack)
                {
                    return new NormalizedHero(
                        this.Normalize(0, this.maxMoneyAtTheTable, this.secondStack - this.getTurnContext.MyMoneyInTheRound),
                        this.Normalize(0, this.maxMoneyAtTheTable, this.getTurnContext.MyMoneyInTheRound),
                        this.Normalize(-1, this.getTurnContext.Opponents.Count, this.getTurnContext.Opponents.Last().ActionPriority));
                }
                else
                {
                    return new NormalizedHero(
                        this.Normalize(0, this.maxMoneyAtTheTable, this.getTurnContext.MoneyLeft),
                        this.Normalize(0, this.maxMoneyAtTheTable, this.getTurnContext.MyMoneyInTheRound),
                        this.Normalize(-1, this.getTurnContext.Opponents.Count, this.getTurnContext.Opponents.Last().ActionPriority));
                }
            }
        }

        public double NormalizedPot()
        {
            if (!this.isUpdated)
            {
                return 0;
            }
            else
            {
                var result = this.Normalize(0, this.maxMoneyAtTheTable, this.getTurnContext.CurrentPot - this.excessInThePot);

                return result;
            }
        }

        public double[] HandStrength()
        {
            if (!this.isUpdated)
            {
                return new double[18];
            }
            else
            {
                double[] wins;
                double[] losses;

                if (this.getTurnContext != null && this.getTurnContext.RoundType == Logic.GameRoundType.PreFlop)
                {
                    lock (locker)
                    {
                        // We use the lock since in the third-party library "HandEvaluator"
                        // the method to which this method refers uses the common field
                        Hand.HandWinOdds(this.pocket.Mask, 0UL, out wins, out losses);
                    }
                }
                else
                {
                    HandEx.BalanceHandStrength(this.pocket.Mask, this.communityCards.Mask, out wins, out losses);
                }

                var combinedArray = new double[18];
                wins.CopyTo(combinedArray, 0);
                losses.CopyTo(combinedArray, 9);

                return combinedArray;
            }
        }

        public double[] Draw()
        {
            if (!this.isUpdated)
            {
                return new double[2];
            }
            else
            {
                return new double[]
                {
                    Hand.IsStraightDraw(this.pocket.Mask, this.communityCards.Mask, 0UL) ? 1 : 0,
                    Hand.IsFlushDraw(this.pocket.Mask, this.communityCards.Mask, 0UL) ? 1 : 0
                };
            }
        }

        // Too expensive method.
        public NormalizedWinOdds WinOdds()
        {
            if (this.getTurnContext == null)
            {
                return new NormalizedWinOdds();
            }
            else
            {
                return new NormalizedWinOdds(this.pocket, this.communityCards);
            }
        }

        public double Normalize(int minimumValue, int maximumValue, int targetValue)
        {
            System.Diagnostics.Debug.Assert(
                targetValue >= minimumValue && targetValue <= maximumValue, "Overstepping the limits of normalization");
            return (double)(targetValue - minimumValue) / (double)(maximumValue - minimumValue);
        }
    }
}