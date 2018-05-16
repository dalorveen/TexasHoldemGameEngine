namespace TexasHoldem.Statistics.Indicators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class FoldToCBet : BaseIndicator<FoldToCBet>
    {
        private Dictionary<GameRoundType, int> totalTimesFoldedToCBetByStreet;
        private Dictionary<GameRoundType, int> totalTimesFacedCBetByStreet;

        public FoldToCBet()
            : base(0)
        {
            this.totalTimesFoldedToCBetByStreet = new Dictionary<GameRoundType, int>();
            this.totalTimesFacedCBetByStreet = new Dictionary<GameRoundType, int>();

            foreach (var item in Enum.GetValues(typeof(GameRoundType)).Cast<GameRoundType>())
            {
                this.totalTimesFoldedToCBetByStreet.Add(item, 0);
                this.totalTimesFacedCBetByStreet.Add(item, 0);
            }
        }

        public bool IsOpportunity { get; private set; }

        public int TotalTimesFoldedToCBet
        {
            get
            {
                return this.totalTimesFoldedToCBetByStreet.Sum(s => s.Value);
            }
        }

        public int TotalTimesFacedCBet
        {
            get
            {
                return this.totalTimesFacedCBetByStreet.Sum(s => s.Value);
            }
        }

        /// <summary>
        /// Gets the percentage of times the player folded to a continuation bet
        /// </summary>
        /// <value>Percentages of folded to continuation bet</value>
        public override double Amount
        {
            get
            {
                return this.TotalTimesFacedCBet == 0
                    ? 0
                    : ((double)this.TotalTimesFoldedToCBet / (double)this.TotalTimesFacedCBet) * 100.0;
            }
        }

        public double AmountByStreet(GameRoundType street)
        {
            return this.totalTimesFacedCBetByStreet[street] == 0
                    ? 0
                    : ((double)this.totalTimesFoldedToCBetByStreet[street]
                        / (double)this.totalTimesFacedCBetByStreet[street]) * 100.0;
        }

        public override void Update(IGetTurnContext context, IStatsContext statsContext)
        {
            if (context.RoundType != GameRoundType.PreFlop
                && context.PreviousRoundActions.Count(p => p.Round == context.RoundType
                    && p.Action.Type == PlayerActionType.Raise) == 1)
            {
                var preflopRaiser = context.PreviousRoundActions
                    .Where(p => p.Round == GameRoundType.PreFlop)
                    .Cast<PlayerActionAndName?>()
                    .LastOrDefault(p => p.Value.Action.Type == PlayerActionType.Raise);

                if (preflopRaiser.HasValue && preflopRaiser.Value.PlayerName != statsContext.PlayerName)
                {
                    for (int i = 1; i <= (int)context.RoundType; i++)
                    {
                        var playerWhoMadeOpeningBet = context.PreviousRoundActions
                            .Where(p => p.Round == (GameRoundType)i)
                            .Cast<PlayerActionAndName?>()
                            .FirstOrDefault(p => p.Value.Action.Type == PlayerActionType.Raise);

                        if (!playerWhoMadeOpeningBet.HasValue)
                        {
                            return;
                        }
                        else if (playerWhoMadeOpeningBet.Value.PlayerName != preflopRaiser.Value.PlayerName)
                        {
                            return;
                        }
                    }

                    this.totalTimesFacedCBetByStreet[context.RoundType]++;
                    this.IsOpportunity = true;
                }
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, IStatsContext statsContext)
        {
            if (this.IsOpportunity && madeAction.Type == PlayerActionType.Fold)
            {
                this.totalTimesFoldedToCBetByStreet[context.RoundType]++;
            }

            this.IsOpportunity = false;
        }

        public override string ToString()
        {
            return this.ToStreetFormat(
                this.AmountByStreet(GameRoundType.Flop),
                this.AmountByStreet(GameRoundType.Turn),
                this.AmountByStreet(GameRoundType.River));
        }
    }
}