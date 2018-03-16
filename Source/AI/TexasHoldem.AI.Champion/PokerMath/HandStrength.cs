namespace TexasHoldem.AI.Champion.PokerMath
{
    using HandEvaluatorExtension;

    public struct HandStrength
    {
        public HandStrength(ICardAdapter pocket, double equity)
        {
            this.Pocket = pocket;
            this.Equity = equity;
        }

        public ICardAdapter Pocket { get; }

        public double Equity { get; }
    }
}
