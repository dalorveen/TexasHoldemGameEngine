namespace TexasHoldem.Logic.Players
{
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic.Cards;

    public class StartHandContext : IStartHandContext
    {
        public StartHandContext(
            Card firstCard,
            Card secondCard,
            int handNumber,
            int moneyLeft,
            int smallBlind,
            List<string> actionPriority)
        {
            this.FirstCard = firstCard;
            this.SecondCard = secondCard;
            this.HandNumber = handNumber;
            this.MoneyLeft = moneyLeft;
            this.SmallBlind = smallBlind;
            this.ActionPriority = actionPriority;
        }

        public Card FirstCard { get; }

        public Card SecondCard { get; }

        public int HandNumber { get; }

        public int MoneyLeft { get; }

        public int SmallBlind { get; }

        public IReadOnlyList<string> ActionPriority { get; }

        public string DealerName
        {
            get
            {
                return this.ActionPriority[this.ActionPriority.Count - 1];
            }
        }
    }
}
