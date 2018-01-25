namespace TexasHoldem.AI.SelfLearningPlayer.Strategy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.AI.SelfLearningPlayer.PokerMath;

    public class HandEconomy : IEnumerable<PlayerEconomy>
    {
        private readonly IList<PlayerEconomy> playerEconomy;

        public HandEconomy(ICalculator calculator)
        {
            this.playerEconomy = new List<PlayerEconomy>();
            var listOfhandStrength = calculator.Equity();
            foreach (var item in listOfhandStrength)
            {
                this.playerEconomy.Add(new PlayerEconomy(item, listOfhandStrength.Where(p => p.Pocket.Mask != item.Pocket.Mask).ToList()));
            }
        }

        public IEnumerator<PlayerEconomy> GetEnumerator()
        {
            return this.playerEconomy.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
