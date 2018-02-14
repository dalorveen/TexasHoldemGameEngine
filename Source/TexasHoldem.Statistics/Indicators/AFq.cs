namespace TexasHoldem.Statistics.Indicators
{
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class AFq : BaseIndicator
    {
        private readonly int[] totalTimesRaised;

        private readonly int[] totalTimesCalled;

        private readonly int[] totalTimesFolded;

        public AFq(int hands = 0)
            : base(hands)
        {
            this.totalTimesRaised = new int[4];
            this.totalTimesCalled = new int[4];
            this.totalTimesFolded = new int[4];
        }

        public AFq(int hands, StreetStorage totalTimesRaised, StreetStorage totalTimesCalled, StreetStorage totalTimesFolded)
            : this(hands)
        {
            this.totalTimesRaised[0] = totalTimesRaised.PF;
            this.totalTimesRaised[1] = totalTimesRaised.F;
            this.totalTimesRaised[2] = totalTimesRaised.T;
            this.totalTimesRaised[3] = totalTimesRaised.R;

            this.totalTimesCalled[0] = totalTimesCalled.PF;
            this.totalTimesCalled[1] = totalTimesCalled.F;
            this.totalTimesCalled[2] = totalTimesCalled.T;
            this.totalTimesCalled[3] = totalTimesCalled.R;

            this.totalTimesFolded[0] = totalTimesFolded.PF;
            this.totalTimesFolded[1] = totalTimesFolded.F;
            this.totalTimesFolded[2] = totalTimesFolded.T;
            this.totalTimesFolded[3] = totalTimesFolded.R;
        }

        public StreetStorage TotalTimesRaised
        {
            get
            {
                return new StreetStorage(
                    this.totalTimesRaised[0],
                    this.totalTimesRaised[1],
                    this.totalTimesRaised[2],
                    this.totalTimesRaised[3]);
            }
        }

        public StreetStorage TotalTimesCalled
        {
            get
            {
                return new StreetStorage(
                    this.totalTimesCalled[0],
                    this.totalTimesCalled[1],
                    this.totalTimesCalled[2],
                    this.totalTimesCalled[3]);
            }
        }

        public StreetStorage TotalTimesFolded
        {
            get
            {
                return new StreetStorage(
                    this.totalTimesFolded[0],
                    this.totalTimesFolded[1],
                    this.totalTimesFolded[2],
                    this.totalTimesFolded[3]);
            }
        }

        /// <summary>
        /// The measure of how frequently a player is aggressive on the (pre-flop/flop/turn/river)
        /// </summary>
        /// <param name="street">Street</param>
        /// <returns>Percentages of aggression frequency on the street</returns>
        public double Percentage(GameRoundType street)
        {
            return this.totalTimesRaised[(int)street] == 0 ? 0 : ((double)this.totalTimesRaised[(int)street]
                / (double)(this.totalTimesRaised[(int)street]
                    + this.totalTimesCalled[(int)street]
                    + this.totalTimesFolded[(int)street])) * 100.0;
        }

        public override void MadeActionExtract(IGetTurnContext context, PlayerAction madeAction)
        {
            if (madeAction.Type == PlayerActionType.Raise)
            {
                this.totalTimesRaised[(int)context.RoundType]++;
            }
            else if (madeAction.Type == PlayerActionType.CheckCall)
            {
                this.totalTimesCalled[(int)context.RoundType]++;
            }
            else if (madeAction.Type == PlayerActionType.Fold)
            {
                this.totalTimesFolded[(int)context.RoundType]++;
            }
        }

        public override string ToString()
        {
            return $"AFq:[PF|{this.Percentage(GameRoundType.PreFlop):0.00}%] " +
                $"[F|{this.Percentage(GameRoundType.Flop):0.00}%] " +
                $"[T|{this.Percentage(GameRoundType.Turn):0.00}%] " +
                $"[R|{this.Percentage(GameRoundType.River):0.00}%]";
        }

        public override BaseIndicator DeepClone()
        {
            return new AFq(this.Hands, this.TotalTimesRaised, this.TotalTimesCalled, this.TotalTimesFolded);
        }
    }
}
