namespace TexasHoldem.AI.Champion.Strategy
{
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.AI.Champion.PokerMath;

    public class PlayerEconomy : IPlayerEconomy
    {
        private readonly IReadOnlyList<HandStrength> opponents;

        private readonly HandStrength hero;

        public PlayerEconomy(HandStrength hero, ICollection<HandStrength> opponents)
        {
            this.hero = hero;
            this.opponents = opponents.ToList().AsReadOnly();
        }

        public HandStrength Hero
        {
            get
            {
                return this.hero;
            }
        }

        public IReadOnlyList<HandStrength> Opponents
        {
            get
            {
                return this.opponents;
            }
        }

        public bool NutHand
        {
            get
            {
                return this.hero.Equity == 1.0;
            }
        }

        public bool BestHand
        {
            get
            {
                return this.hero.Equity >= this.opponents.Max(s => s.Equity);
            }
        }

        public int TiedHandsWithHero
        {
            get
            {
                return this.opponents.Count(p => p.Equity == this.hero.Equity);
            }
        }

        public ICollection<HandStrength> HandsThatLoseToTheHero
        {
            get
            {
                return this.opponents.Where(p => p.Equity < this.hero.Equity).ToList();
            }
        }

        public double OptimalInvestment(int pot)
        {
            if (this.BestHand)
            {
                // investment for +EV and reasonable for a caller
                var maxEquity = this.hero.Equity;
                var secondEquity = this.opponents.Where(p => p.Equity != maxEquity).Max(s => s.Equity);
                return (pot * secondEquity) / ((1.0 - secondEquity) - secondEquity);
            }
            else
            {
                // investment for a neutral EV
                return this.NeutralEVInvestment(pot);
            }
        }

        public double NeutralEVInvestment(int pot)
        {
            return (pot * this.hero.Equity) / (1.0 - this.hero.Equity);
        }
    }
}
