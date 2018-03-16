namespace HandEvaluatorExtension
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic.Cards;

    public class Pocket : ICardAdapter
    {
        private readonly ICollection<Card> cards;

        public Pocket(ICollection<Card> cards)
        {
            if (cards.Count != 2)
            {
                throw new ArgumentOutOfRangeException(nameof(cards), "Two hole cards are required");
            }

            this.cards = cards;
            this.Mask = new CardAdapter(cards).Mask;
        }

        public ICollection<Card> NativeType
        {
            get
            {
                return this.cards.ToList();
            }
        }

        public ulong Mask { get; }

        public string Text
        {
            get
            {
                return HoldemHand.Hand.MaskToString(this.Mask);
            }
        }
    }
}