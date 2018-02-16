namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class FourBet : BaseIndicator
    {
        private readonly int[] totalTimes;

        private readonly int[] totalOpportunities;

        public FourBet(int hands = 0)
            : base(hands)
        {
            this.totalTimes = new int[4];
            this.totalOpportunities = new int[4];
        }

        public FourBet(int hands, StreetStorage totalTimes, StreetStorage totalOpportunities)
            : this(hands)
        {
            this.totalTimes[0] = totalTimes.PF;
            this.totalTimes[1] = totalTimes.F;
            this.totalTimes[2] = totalTimes.T;
            this.totalTimes[3] = totalTimes.R;

            this.totalOpportunities[0] = totalOpportunities.PF;
            this.totalOpportunities[1] = totalOpportunities.F;
            this.totalOpportunities[2] = totalOpportunities.T;
            this.totalOpportunities[3] = totalOpportunities.R;
        }

        public bool IsOpportunity { get; private set; }

        public StreetStorage TotalTimes
        {
            get
            {
                return new StreetStorage(
                    this.totalTimes[0],
                    this.totalTimes[1],
                    this.totalTimes[2],
                    this.totalTimes[3]);
            }
        }

        public StreetStorage TotalOpportunities
        {
            get
            {
                return new StreetStorage(
                    this.totalOpportunities[0],
                    this.totalOpportunities[1],
                    this.totalOpportunities[2],
                    this.totalOpportunities[3]);
            }
        }

        /// <summary>
        /// The percentage of time a player re-raised a 3Bet on the (pre-flop/flop/turn/river)
        /// </summary>
        /// <param name="street">Street</param>
        /// <returns>Percentages of 4Bet on the street</returns>
        public double Percentage(GameRoundType street)
        {
            return this.totalOpportunities[(int)street] == 0
                ? 0 : ((double)this.totalTimes[(int)street] / (double)this.totalOpportunities[(int)street]) * 100.0;
        }

        public override void GetTurnExtract(IGetTurnContext context)
        {
            var raises = context.PreviousRoundActions.Count(x => x.Action.Type == PlayerActionType.Raise);

            if (raises == (context.RoundType == GameRoundType.PreFlop ? 2 : 3))
            {
                this.totalOpportunities[(int)context.RoundType]++;
                this.IsOpportunity = true;
            }
        }

        public override void MadeActionExtract(IGetTurnContext context, PlayerAction madeAction)
        {
            if (this.IsOpportunity && madeAction.Type == PlayerActionType.Raise)
            {
                this.totalTimes[(int)context.RoundType]++;
            }

            this.IsOpportunity = false;
        }

        public override string ToString()
        {
            return $"FourBet:[PF|{this.Percentage(GameRoundType.PreFlop):0.00}%] " +
                $"[F|{this.Percentage(GameRoundType.Flop):0.00}%] " +
                $"[T|{this.Percentage(GameRoundType.Turn):0.00}%] " +
                $"[R|{this.Percentage(GameRoundType.River):0.00}%]";
        }

        public override BaseIndicator DeepClone()
        {
            var copy = new FourBet(this.Hands, this.TotalTimes, this.TotalOpportunities);
            copy.IsOpportunity = this.IsOpportunity;
            return copy;
        }
    }
}
