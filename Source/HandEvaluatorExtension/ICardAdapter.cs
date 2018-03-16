namespace HandEvaluatorExtension
{
    using System.Collections.Generic;

    using TexasHoldem.Logic.Cards;

    public interface ICardAdapter
    {
        ICollection<Card> NativeType { get; }

        ulong Mask { get; }

        string Text { get; }
    }
}