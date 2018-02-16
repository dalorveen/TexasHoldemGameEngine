namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class CBet : BaseIndicator
    {
        private readonly string playerName;

        private readonly int[] totalTimes;

        private readonly int[] totalOpportunities;

        private bool isPreflopRaiser;

        public CBet(string playerName, int hands = 0)
            : base(hands)
        {
            this.playerName = playerName;
            this.totalTimes = new int[4];
            this.totalOpportunities = new int[4];
        }

        public CBet(string playerName, int hands, StreetStorage totalTimes, StreetStorage totalOpportunities)
            : this(playerName, hands)
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
        /// The percentage of time a player followed their [pre-flop raise by betting the flop]
        /// </summary>
        /// <param name="street">Street</param>
        /// <returns>Percentages of continuation bet on the street</returns>
        public double Percentage(GameRoundType street)
        {
            return this.totalOpportunities[(int)street] == 0
                ? 0 : ((double)this.totalTimes[(int)street] / (double)this.totalOpportunities[(int)street]) * 100.0;
        }

        public override void GetTurnExtract(IGetTurnContext context)
        {
            if (context.RoundType != GameRoundType.PreFlop && context.MoneyToCall == 0)
            {
                if (this.isPreflopRaiser)
                {
                    for (int i = 1; i < (int)context.RoundType; i++)
                    {
                        var previousRoundFirstRaiser = context.PreviousRoundActions
                            .Where(p => p.Round == (GameRoundType)i)
                            .Cast<PlayerActionAndName?>()
                            .LastOrDefault(p => p.Value.Action.Type == PlayerActionType.Raise);

                        if (!previousRoundFirstRaiser.HasValue)
                        {
                            return;
                        }
                        else if (previousRoundFirstRaiser.Value.PlayerName != this.playerName)
                        {
                            return;
                        }
                    }

                    this.totalOpportunities[(int)context.RoundType]++;
                    this.IsOpportunity = true;
                }
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

        public override void EndRoundExtract(IEndRoundContext context)
        {
            if (context.CompletedRoundType == GameRoundType.PreFlop)
            {
                var preflopRaiser = context.RoundActions
                    .Cast<PlayerActionAndName?>()
                    .LastOrDefault(p => p.Value.Action.Type == PlayerActionType.Raise);

                if (preflopRaiser.HasValue && preflopRaiser.Value.PlayerName == this.playerName)
                {
                    this.isPreflopRaiser = true;
                }
                else
                {
                    this.isPreflopRaiser = false;
                }
            }
        }

        public override string ToString()
        {
            return $"CBet:[F|{this.Percentage(GameRoundType.Flop):0.00}%] " +
                $"[T|{this.Percentage(GameRoundType.Turn):0.00}%] " +
                $"[R|{this.Percentage(GameRoundType.River):0.00}%]";
        }

        public override BaseIndicator DeepClone()
        {
            var copy = new CBet(this.playerName, this.Hands, this.TotalTimes, this.TotalOpportunities);
            copy.isPreflopRaiser = this.isPreflopRaiser;
            copy.IsOpportunity = this.IsOpportunity;
            return copy;
        }
    }
}
