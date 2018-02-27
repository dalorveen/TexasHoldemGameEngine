namespace TexasHoldem.AI.Champion.Strategy
{
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.AI.Champion.Helpers;
    using TexasHoldem.AI.Champion.PokerMath;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class StartingHand
    {
        public static readonly IReadOnlyDictionary<string, HoldemHand.PocketHands> PocketsFromStrongToWeak;

        static StartingHand()
        {
            var pockets = new Dictionary<string, HoldemHand.PocketHands>();
            for (int i = 0; i < 9; i++)
            {
                foreach (var item in HoldemHand.PocketHands.Group((HoldemHand.PocketHands.GroupTypeEnum)i))
                {
                    var pocketHand169Str = HoldemHand.Hand.PocketHand169Type(item).ToString().Replace("Pocket", string.Empty);
                    HoldemHand.PocketHands amount;
                    if (pockets.TryGetValue(pocketHand169Str, out amount))
                    {
                        continue;
                    }
                    else
                    {
                        pockets[pocketHand169Str] = HoldemHand.PocketHands.PocketCards169(pocketHand169Str);
                    }
                }
            }

            PocketsFromStrongToWeak = new Dictionary<string, HoldemHand.PocketHands>(pockets);
        }

        public StartingHand(IPocket pocket)
        {
            this.Pocket = pocket;
        }

        public IPocket Pocket { get; }

        public bool IsPremiumHand
        {
            get
            {
                var premiumHands = HoldemHand.PocketHands.PocketCards169("AA")
                    + HoldemHand.PocketHands.PocketCards169("KK")
                    + HoldemHand.PocketHands.PocketCards169("AKs");
                return premiumHands.Contains(this.Pocket.Mask);
            }
        }

        public int HeroRelativePosition(IGetTurnExtendedContext context)
        {
            return context.Opponents.Where(x => x.InHand && x.ActionPriority < 0).Count();
        }

        public HoldemHand.PocketHands PlayablePocketsForTheCurrentPosition(
            double rangeInPercent, double slope, IGetTurnExtendedContext context)
        {
            /*
             * The number of playable pockets depends on the position of the hero.
             * From the early position of playable pockets is less than from a late
             * position. In this case, the dependence of playable cards on the position
             * is quadratic.
            */
            var numberOfOpponentsInHand = context.Opponents.Where(x => x.InHand).Count();

            var frequency = Distribution.FrequencyOfActionFromASpecificPosition(
                this.HeroRelativePosition(context), numberOfOpponentsInHand, rangeInPercent / 100.0, slope);

            var numberOfplayablePockets = (int)(1326.0 * (numberOfOpponentsInHand + 1) * (frequency > 1.0 ? 1.0 : frequency));
            return new HoldemHand.PocketHands(
                PocketsFromStrongToWeak.Select(s => s.Value).SelectMany(s => s).Take(numberOfplayablePockets).ToList());
        }

        public HoldemHand.PocketHands PlayablePockets(double rangeInPercent)
        {
            rangeInPercent = rangeInPercent > 100 ? 100 : rangeInPercent;
            var numberOfplayablePockets = (int)(1326.0 * rangeInPercent / 100.0);
            return new HoldemHand.PocketHands(
                PocketsFromStrongToWeak.Select(s => s.Value).SelectMany(s => s).Take(numberOfplayablePockets).ToList());
        }

        public bool IsPlayablePocket(double rangeInPercent, double slope, IGetTurnExtendedContext context)
        {
            var numberOfPlayablePockets = this.PlayablePocketsForTheCurrentPosition(rangeInPercent, slope, context);
            return numberOfPlayablePockets.Contains(this.Pocket.Mask);
        }

        public bool IsPlayablePocket(double rangeInPercent)
        {
            var numberOfPlayablePockets = this.PlayablePockets(rangeInPercent);
            return numberOfPlayablePockets.Contains(this.Pocket.Mask);
        }
    }
}
