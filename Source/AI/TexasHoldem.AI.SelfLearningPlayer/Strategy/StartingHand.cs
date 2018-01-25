namespace TexasHoldem.AI.SelfLearningPlayer.Strategy
{
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.AI.SelfLearningPlayer.Helpers;
    using TexasHoldem.AI.SelfLearningPlayer.PokerMath;
    using TexasHoldem.Logic.Players;

    public class StartingHand
    {
        public static readonly IReadOnlyDictionary<string, HoldemHand.PocketHands> PocketsFromStrongToWeak;

        private readonly IPocket pocket;

        private readonly IGetTurnContext context;

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

        public StartingHand(IPocket pocket, IGetTurnContext context)
        {
            this.pocket = pocket;
            this.context = context;
        }

        public int RelativePosition
        {
            get
            {
                var playersInHand = this.context.Opponents.Where(x => x.InHand)
                    .Select(x => x.Position)
                    .Union(new[] { this.context.Position })
                    .OrderBy(k => k);
                return playersInHand.TakeWhile(x => x != this.context.Position).Count();
            }
        }

        public HoldemHand.PocketHands PlayablePocketsForTheCurrentPosition(double range, double slope)
        {
            /*
             * The number of playable pockets depends on the position of the hero.
             * From the early position of playable pockets is less than from a late
             * position. In this case, the dependence of playable cards on the position
             * is quadratic.
            */
            var numberOfOpponentsInHand = this.context.Opponents.Where(x => x.InHand).Count();
            var frequency = Distribution.FrequencyOfActionFromASpecificPosition(
                this.RelativePosition, numberOfOpponentsInHand, range, slope);
            var numberOfplayablePockets = (int)(1326.0 * (numberOfOpponentsInHand + 1) * frequency);
            return new HoldemHand.PocketHands(
                PocketsFromStrongToWeak.Select(s => s.Value).SelectMany(s => s).Take(numberOfplayablePockets).ToList());
        }

        public HoldemHand.PocketHands PlayablePockets(double range)
        {
            var numberOfplayablePockets = (int)(1326.0 * range);
            return new HoldemHand.PocketHands(
                PocketsFromStrongToWeak.Select(s => s.Value).SelectMany(s => s).Take(numberOfplayablePockets).ToList());
        }

        public bool IsPlayablePocket(double range, double slope)
        {
            var numberOfPlayablePockets = this.PlayablePocketsForTheCurrentPosition(range, slope);
            return numberOfPlayablePockets.Contains(this.pocket.Mask);
        }

        public bool IsPlayablePocket(double range)
        {
            var numberOfPlayablePockets = this.PlayablePockets(range);
            return numberOfPlayablePockets.Contains(this.pocket.Mask);
        }
    }
}
