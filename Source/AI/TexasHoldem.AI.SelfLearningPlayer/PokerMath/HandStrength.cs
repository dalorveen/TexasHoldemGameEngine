namespace TexasHoldem.AI.Champion.PokerMath
{
    using TexasHoldem.AI.Champion.Helpers;

    public struct HandStrength
    {
        public HandStrength(IPocket pocket, double equity)
        {
            this.Pocket = pocket;
            this.Equity = equity;
        }

        public IPocket Pocket { get; }

        public double Equity { get; }
    }
}
