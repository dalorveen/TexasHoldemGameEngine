namespace TexasHoldem.Logic.Players
{
    using System.Collections.Generic;
    using TexasHoldem.Logic.Cards;

    public class EndHandContext : IEndHandContext
    {
        public EndHandContext(
            Dictionary<string, ICollection<Card>> showdownCards,
            int moneyLeft,
            GameRoundType lastGameRoundType,
            List<string> playersWhoWonMoney)
        {
            this.ShowdownCards = showdownCards;
            this.MoneyLeft = moneyLeft;
            this.LastGameRoundType = lastGameRoundType;
            this.PlayersWhoWonMoney = playersWhoWonMoney;
        }

        public Dictionary<string, ICollection<Card>> ShowdownCards { get; private set; }

        public int MoneyLeft { get; private set; }

        public GameRoundType LastGameRoundType { get; }

        public IReadOnlyList<string> PlayersWhoWonMoney { get; }
    }
}