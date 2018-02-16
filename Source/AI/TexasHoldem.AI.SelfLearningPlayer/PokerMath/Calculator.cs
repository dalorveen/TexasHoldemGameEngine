namespace TexasHoldem.AI.Champion.PokerMath
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.AI.Champion.Helpers;
    using TexasHoldem.Logic.Cards;

    public class Calculator : ICalculator
    {
        private readonly ICollection<IPocket> pockets;

        private readonly ulong dead;

        private readonly ulong communityCards;

        public Calculator(ICollection<IPocket> pockets, ICollection<Card> dead, ICollection<Card> communityCards)
        {
            if (pockets == null)
            {
                throw new ArgumentNullException(nameof(pockets));
            }
            else if (pockets.Count < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(pockets), "Requires at least two pockets");
            }

            if (dead == null)
            {
                throw new ArgumentNullException(nameof(dead));
            }

            if (communityCards == null)
            {
                throw new ArgumentNullException(nameof(communityCards));
            }

            this.pockets = pockets;
            this.dead = new CardAdapter(dead).Mask;
            this.communityCards = new CardAdapter(communityCards).Mask;
        }

        public ICollection<HandStrength> Equity()
        {
            var potsWon = new Dictionary<ulong, double>();
            var dead = this.dead;

            foreach (var item in this.pockets)
            {
                potsWon.Add(item.Mask, 0);
                dead |= item.Mask;
            }

            var games = 0.0;
            var winners = new List<ulong>();

            foreach (var nextCommunityCards in HoldemHand.Hand.Hands(this.communityCards, dead, 5))
            {
                var winningMaskValue = 0U;
                foreach (var item in this.pockets)
                {
                    var maskValue = HoldemHand.Hand.Evaluate(item.Mask | nextCommunityCards, 7);
                    if (maskValue > winningMaskValue)
                    {
                        winningMaskValue = maskValue;
                        winners.Clear();
                        winners.Add(item.Mask);
                    }
                    else if (maskValue == winningMaskValue)
                    {
                        winners.Add(item.Mask);
                    }
                }

                foreach (var item in winners)
                {
                    potsWon[item] += 1.0 / winners.Count();
                }

                games++;
            }

            var result = new List<HandStrength>();
            foreach (var item in potsWon)
            {
                result.Add(new HandStrength(this.pockets.First(x => x.Mask == item.Key), item.Value / games));
            }

            return result;
        }
    }
}