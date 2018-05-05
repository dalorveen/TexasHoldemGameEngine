namespace HandEvaluatorExtension
{
    using System;
    using System.Linq;

    using HoldemHand;

    public class Range
    {
        /// <summary>
        /// Parses the input string to a range of hole cards
        /// </summary>
        /// <param name="range">The same format as in the PokerStove or Equilab (77+, 44, 95s-92s, AJo-A9o)</param>
        /// <returns>Returns the range of pocket cards</returns>
        public static PocketHands Parse(string range)
        {
            var strRanks = new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K", "A" };
            var pockets = new PocketHands();
            var pile = range.Replace(" ", string.Empty).Split(',');

            for (int i = 0; i < pile.Length; i++)
            {
                if (pile[i].Length == 7)
                {
                    // e.g. AQo-A9o
                    pockets += PocketHands.PocketCards169Range(pile[i].Substring(0, 3), pile[i].Substring(4, 3));
                }
                else if (pile[i].Length == 5)
                {
                    // e.g. TT-88
                    pockets += PocketHands.PocketCards169Range(pile[i].Substring(0, 2), pile[i].Substring(3, 2));
                }
                else if (pile[i].Length == 4)
                {
                    // e.g. AJs+ A9o+
                    int[] rankValues = new int[]
                    {
                        Hand.CardRank(Hand.ParseCard(pile[i][0].ToString() + "s")),
                        Hand.CardRank(Hand.ParseCard(pile[i][1].ToString() + "s"))
                    };
                    int highRank = rankValues.Max();
                    int lowRank = rankValues.Min();
                    pockets += PocketHands.PocketCards169Range(
                        strRanks[highRank] + strRanks[highRank - 1] + pile[i][2].ToString(),
                        strRanks[highRank] + strRanks[lowRank] + pile[i][2].ToString());
                }
                else if (pile[i].Length == 3)
                {
                    // e.g. KQs JJ+
                    if (pile[i].EndsWith("+"))
                    {
                        pockets += PocketHands.PocketCards169Range("AA", pile[i].Replace("+", string.Empty));
                    }
                    else
                    {
                        pockets += PocketHands.PocketCards169(pile[i]);
                    }
                }
                else if (pile[i].Length == 2)
                {
                    // e.g. QQ
                    pockets += PocketHands.PocketCards169(pile[i]);
                }
                else
                {
                    throw new Exception("Can not parse the range.");
                }
            }

            return pockets;
        }
    }
}
