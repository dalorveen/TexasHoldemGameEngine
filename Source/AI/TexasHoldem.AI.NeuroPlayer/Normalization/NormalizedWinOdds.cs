namespace TexasHoldem.AI.NeuroPlayer.Normalization
{
    using System.Collections.Generic;

    using HandEvaluatorExtension;
    using HoldemHand;

    public class NormalizedWinOdds
    {
        public NormalizedWinOdds()
        {
            this.Initialize(new double[9], new double[9]);
        }

        public NormalizedWinOdds(ICardAdapter pocket, ICardAdapter communityCards)
        {
            double[] heroWins;
            double[] opponentWins;

            HoldemHand.Hand.HandWinOdds(pocket.Mask, communityCards.Mask, out heroWins, out opponentWins);

            this.Initialize(heroWins, opponentWins);
        }

        public IReadOnlyDictionary<Hand.HandTypes, double> HeroWins { get; private set; }

        public IReadOnlyDictionary<Hand.HandTypes, double> OpponentWins { get; private set; }

        private void Initialize(double[] heroWins, double[] opponentWins)
        {
            var tempHeroWins = new Dictionary<Hand.HandTypes, double>();
            var tempOpponentWins = new Dictionary<Hand.HandTypes, double>();

            for (int i = 0; i < 9; i++)
            {
                tempHeroWins.Add((Hand.HandTypes)i, heroWins[i]);
                tempOpponentWins.Add((Hand.HandTypes)i, opponentWins[i]);
            }

            this.HeroWins = new Dictionary<Hand.HandTypes, double>(tempHeroWins);
            this.OpponentWins = new Dictionary<Hand.HandTypes, double>(tempOpponentWins);
        }
    }
}