namespace TexasHoldem.AI.Champion
{
    using HoldemHand;
    using TexasHoldem.Statistics.Indicators;

    public class StatsSetting : IAmount
    {
        public StatsSetting(double amount, PocketHands possibleRange)
        {
            this.Amount = amount;
            this.PlayableRange = possibleRange;
        }

        public double Amount { get; }

        public PocketHands PlayableRange { get; }
    }
}
