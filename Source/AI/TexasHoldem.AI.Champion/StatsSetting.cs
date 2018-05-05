namespace TexasHoldem.AI.Champion
{
    using HoldemHand;
    using TexasHoldem.Statistics.Indicators;

    public class StatsSetting<TIndicator>
    {
        public StatsSetting(TIndicator indicator, PocketHands possibleRange)
        {
            this.Indicator = indicator;
            this.PossibleRange = possibleRange;
        }

        public TIndicator Indicator { get; }

        public PocketHands PossibleRange { get; }
    }
}
