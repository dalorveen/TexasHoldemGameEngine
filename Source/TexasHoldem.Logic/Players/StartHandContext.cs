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
            int actionPriority,
            string dealerName)
        {
            this.FirstCard = firstCard;
            this.SecondCard = secondCard;
            this.HandNumber = handNumber;
            this.MoneyLeft = moneyLeft;
            this.SmallBlind = smallBlind;
            this.ActionPriority = actionPriority;
            this.DealerName = dealerName;
        }

        public Card FirstCard { get; }

        public Card SecondCard { get; }

        public int HandNumber { get; }

        public int MoneyLeft { get; }

        public int SmallBlind { get; }

        /// <summary>
        /// Gets the sequence number of the player in the turn queue.
        /// Small blind = 0, big blind = 1, ..., dealer 9.
        /// </summary>
        /// <value>
        /// The value is from zero to 10.
        /// </value>
        public int ActionPriority { get; }

        public string DealerName { get; }
    }
}
