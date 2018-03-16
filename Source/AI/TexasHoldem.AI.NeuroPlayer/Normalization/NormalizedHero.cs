namespace TexasHoldem.AI.NeuroPlayer.Normalization
{
    public struct NormalizedHero
    {
        public NormalizedHero(double money, double currentRoundBet, double position)
        {
            this.Money = money;
            this.CurrentRoundBet = currentRoundBet;
            this.Position = position;
        }

        public double Money { get; }

        public double CurrentRoundBet { get; }

        public double Position { get; }
    }
}