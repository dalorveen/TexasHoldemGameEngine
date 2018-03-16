namespace TexasHoldem.AI.Champion.Strategy
{
    using System.Linq;

    using HandEvaluatorExtension;

    public class StartingHand
    {
        public StartingHand(ICardAdapter pocket)
        {
            this.Pocket = pocket;
        }

        public ICardAdapter Pocket { get; }

        public bool IsPremiumHand
        {
            get
            {
                var premiumHands = HoldemHand.PocketHands.PocketCards169("AA")
                    + HoldemHand.PocketHands.PocketCards169("KK")
                    + HoldemHand.PocketHands.PocketCards169("QQ")
                    + HoldemHand.PocketHands.PocketCards169("JJ")
                    + HoldemHand.PocketHands.PocketCards169("AKs");
                return premiumHands.Contains(this.Pocket.Mask);
            }
        }

        public HoldemHand.PocketHands PlayablePockets(double rangeInPercent)
        {
            rangeInPercent = rangeInPercent > 100 ? 100 : rangeInPercent;
            var numberOfplayablePockets = (int)(169.0 * rangeInPercent / 100.0);
            return new HoldemHand.PocketHands(StartingHandStrength.SortedFromStrongToWeak
                .Take(numberOfplayablePockets)
                .SelectMany(s => s.Value)
                .ToList());
        }

        public bool IsPlayablePocket(double rangeInPercent)
        {
            var numberOfPlayablePockets = this.PlayablePockets(rangeInPercent);
            return numberOfPlayablePockets.Contains(this.Pocket.Mask);
        }
    }
}
