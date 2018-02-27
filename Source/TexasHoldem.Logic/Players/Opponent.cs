namespace TexasHoldem.Logic.Players
{
    using System.Collections.Generic;

    using TexasHoldem.Logic.Cards;

    public struct Opponent
    {
        internal Opponent(string name, ICollection<Card> holeCards, int actionPriority, int money, int currentRoundBet, bool inHand)
        {
            this.Name = name;
            this.HoleCards = holeCards;
            this.ActionPriority = actionPriority;
            this.Money = money;
            this.CurrentRoundBet = currentRoundBet;
            this.InHand = inHand;
        }

        public string Name { get; }

        public ICollection<Card> HoleCards { get; }

        public int ActionPriority { get; }

        public int Money { get; }

        public int CurrentRoundBet { get; }

        public bool InHand { get; }
    }
}
