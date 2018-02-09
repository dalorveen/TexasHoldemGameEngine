namespace TexasHoldem.AI.SelfLearningPlayer.Strategy
{
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.AI.SelfLearningPlayer.PokerMath;
    using TexasHoldem.Logic.Cards;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public abstract class BaseBehavior
    {
        private readonly IPocket pocket;

        private readonly IStats playingStyle;

        private readonly IGetTurnContext context;

        private readonly IReadOnlyCollection<Card> communityCards;

        private readonly Tracker tracker;

        public BaseBehavior(
            IPocket pocket, IStats playingStyle, IGetTurnContext context, IReadOnlyCollection<Card> communityCards)
        {
            this.playingStyle = playingStyle;
            this.pocket = pocket;
            this.context = context;
            this.communityCards = communityCards;
            this.tracker = new Tracker(context);
        }

        public IStats PlayingStyle
        {
            get
            {
                return this.playingStyle;
            }
        }

        public IGetTurnContext Context
        {
            get
            {
                return this.context;
            }
        }

        public Tracker Tracker
        {
            get
            {
                return this.tracker;
            }
        }

        public abstract PlayerAction OptimalAction();

        public PlayerAction RaiseOrAllIn(int moneyToRaise)
        {
            if (moneyToRaise >= this.Context.MoneyLeft - this.Context.MoneyToCall)
            {
                // All-In
                return PlayerAction.Raise(this.Context.MoneyLeft - this.Context.MoneyToCall);
            }
            else
            {
                return PlayerAction.Raise(moneyToRaise);
            }
        }

        public PlayerEconomy PlayerEconomy()
        {
            var calculator = this.Calculator();
            var handEconomy = new HandEconomy(calculator);
            return handEconomy.First(p => p.Hero.Pocket.Mask == this.pocket.Mask);
        }

        public bool IsPush(int moneyToRaise)
        {
            return (double)(this.context.MoneyLeft - moneyToRaise) / (double)(moneyToRaise + this.context.CurrentPot) <= 0.5;
        }

        private ICalculator Calculator()
        {
            var holeCardsOfOpponentsWhoAreInHand = new List<IPocket>();
            holeCardsOfOpponentsWhoAreInHand.Add(this.pocket);
            var deadCards = new List<Card>();
            foreach (var item in this.Context.Opponents)
            {
                if (item.InHand)
                {
                    holeCardsOfOpponentsWhoAreInHand.Add(new Pocket(item.HoleCards));
                }
                else
                {
                    deadCards.AddRange(item.HoleCards);
                }
            }

            return new Calculator(
                holeCardsOfOpponentsWhoAreInHand,
                deadCards,
                this.communityCards.ToList());
        }
    }
}
