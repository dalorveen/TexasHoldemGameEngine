namespace TexasHoldem.AI.Champion.Strategy
{
    using System.Collections.Generic;
    using System.Linq;

    using HandEvaluatorExtension;
    using TexasHoldem.AI.Champion.PokerMath;
    using TexasHoldem.Logic.Cards;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public abstract class BaseBehavior
    {
        public BaseBehavior(PlayingStyle playingStyle)
        {
            this.PlayingStyle = playingStyle;
        }

        public PlayingStyle PlayingStyle { get; }

        public abstract PlayerAction OptimalAction(
            ICardAdapter pocket, IReadOnlyCollection<Card> communityCards, IGetTurnContext context, Stats stats);

        public PlayerAction RaiseOrAllIn(int moneyToRaise, IGetTurnContext context)
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
            ICardAdapter pocket, IReadOnlyCollection<Card> communityCards, IGetTurnContext context)
        {
            var calculator = this.Calculator(pocket, communityCards, context);
            var handEconomy = new HandEconomy(calculator);
            return handEconomy.First(p => p.Hero.Pocket.Mask == pocket.Mask);
        }

        public bool IsPush(int moneyToRaise, IGetTurnContext context)
        {
            return (double)(context.MoneyLeft - moneyToRaise) / (double)(moneyToRaise + context.CurrentPot) <= 0.5;
        }

        public PlayerAction ToRaise(int moneyToRaise, IGetTurnContext context)
        {
            if (this.IsPush(moneyToRaise, context))
            {
                return this.RaiseOrAllIn(int.MaxValue, context);
            }
            else
            {
                return this.RaiseOrAllIn(moneyToRaise, context);
            }
        }

        //public bool IsInPosition(IGetTurnContext context)
        //{
        //    
        //    return !context.Opponents.Any(x => x.InHand && x.ActionPriority > 0);
        //}

        private ICalculator Calculator(
            ICardAdapter pocket, IReadOnlyCollection<Card> communityCards, IGetTurnContext context)
        {
            var holeCardsOfOpponentsWhoAreInHand = new List<ICardAdapter>();
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
