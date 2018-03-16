namespace TexasHoldem.AI.NeuroPlayer.Normalization
{
    using System.Collections.Generic;
    using System.Linq;

    using HandEvaluatorExtension;
    using HoldemHand;
    using TexasHoldem.Logic.Players;

    public class Normalization
    {
        private readonly IStartGameContext startGameContext;

        private ICardAdapter pocket;

        private ICardAdapter communityCards;

        private IGetTurnContext getTurnContext;

        private int maxMoneyAtTheTable;

        private bool isUpdated;

        public Normalization(IStartGameContext context)
        {
            this.startGameContext = context;
            this.isUpdated = false;
        }

        public void Update(ICardAdapter pocket, ICardAdapter communityCards, IGetTurnContext context)
        {
            var allChips = new List<int>();
            var activeOpponents = context.Opponents.Where(p => p.InHand);

            allChips.AddRange(activeOpponents.Select(s => s.Money));
            allChips.AddRange(activeOpponents.Select(s => s.CurrentRoundBet));
            allChips.AddRange(new[] { context.CurrentPot, context.MoneyLeft, context.MyMoneyInTheRound });

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
                for (int i = 1; i < this.startGameContext.PlayerNames.Count; i++)
                {
                    normalizedOpponents.Add(new NormalizedOpponent(0, 0, 0));
                }
            }
            else
            {
                foreach (var item in this.getTurnContext.Opponents)
                {
                    if (item.InHand)
                    {
                        normalizedOpponents.Add(new NormalizedOpponent(
                        this.Normalize(0, this.maxMoneyAtTheTable, item.Money),
                        this.Normalize(0, this.maxMoneyAtTheTable, item.CurrentRoundBet),
                        1));
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
                return new NormalizedHero(
                    this.Normalize(0, this.maxMoneyAtTheTable, this.getTurnContext.MoneyLeft),
                    this.Normalize(0, this.maxMoneyAtTheTable, this.getTurnContext.MyMoneyInTheRound),
                    this.Normalize(-1, this.getTurnContext.Opponents.Count, this.getTurnContext.Opponents.Last().ActionPriority));
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
                var result = this.Normalize(0, this.maxMoneyAtTheTable, this.getTurnContext.CurrentPot);

                return result;
            }
        }

        public double NormalizedStreet()
        {
            if (!this.isUpdated)
            {
                return 0;
            }
            else
            {
                return (int)this.getTurnContext.RoundType / 3.0;
            }
        }

        public double HandStrength()
        {
            if (!this.isUpdated)
            {
                return 0;
            }
            else if (this.getTurnContext != null && this.getTurnContext.RoundType == Logic.GameRoundType.PreFlop)
            {
                double[] hero;
                double[] opponent;
                Hand.HandWinOdds(this.pocket.Mask, 0UL, out hero, out opponent);

                return hero.Sum();
            }
            else
            {
                return Hand.HandStrength(this.pocket.Mask, this.communityCards.Mask);
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
            return (double)(targetValue - minimumValue) / (double)(maximumValue - minimumValue);
        }
    }
}