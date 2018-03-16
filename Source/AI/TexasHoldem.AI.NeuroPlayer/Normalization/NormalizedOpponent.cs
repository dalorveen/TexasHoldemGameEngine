namespace TexasHoldem.AI.NeuroPlayer.Normalization
{
    public struct NormalizedOpponent
    {
        public NormalizedOpponent(double money, double currentRoundBet, double inHand)
        {
            this.Money = money;
            this.CurrentRoundBet = currentRoundBet;
            this.InHand = inHand;
        }

        public double Money { get; }

        public double CurrentRoundBet { get; }

        public double InHand { get; }
    }
}