namespace TexasHoldem.AI.Champion.Strategy
{
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.AI.Champion.Helpers;
    using TexasHoldem.AI.Champion.PokerMath;
    using TexasHoldem.Logic.Cards;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public abstract class BaseBehavior
    {
        private readonly IStats playingStyle;

        public BaseBehavior(IStats playingStyle)
        {
            this.playingStyle = playingStyle;
        }

        public IStats PlayingStyle
        {
            get
            {
                return this.playingStyle;
            }
        }

        public abstract PlayerAction OptimalAction(
            IPocket pocket, IGetTurnExtendedContext context, IReadOnlyCollection<Card> communityCards);

        public PlayerAction RaiseOrAllIn(int moneyToRaise, IGetTurnExtendedContext context)
        {
            if (moneyToRaise >= context.MoneyLeft - context.MoneyToCall)
            {
                // All-In
                return PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall);
            }
            else
            {
                return PlayerAction.Raise(moneyToRaise);
            }
        }

        public PlayerEconomy PlayerEconomy(
            IPocket pocket, IGetTurnExtendedContext context, IReadOnlyCollection<Card> communityCards)
        {
            var calculator = this.Calculator(pocket, context, communityCards);
            var handEconomy = new HandEconomy(calculator);
            return handEconomy.First(p => p.Hero.Pocket.Mask == pocket.Mask);
        }

        public bool IsPush(int moneyToRaise, IGetTurnExtendedContext context)
        {
            return (double)(context.MoneyLeft - moneyToRaise) / (double)(moneyToRaise + context.CurrentPot) <= 0.5;
        }

        private ICalculator Calculator(
            IPocket pocket, IGetTurnExtendedContext context, IReadOnlyCollection<Card> communityCards)
        {
            var holeCardsOfOpponentsWhoAreInHand = new List<IPocket>();
            holeCardsOfOpponentsWhoAreInHand.Add(pocket);
            var deadCards = new List<Card>();
            foreach (var item in context.Opponents)
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
                communityCards.ToList());
        }
    }
}
