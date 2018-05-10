namespace TexasHoldem.Statistics.Indicators
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class FoldToCBet : BaseIndicator<FoldToCBet>
    {
        public FoldToCBet()
            : base(0)
        {
        }

        public FoldToCBet(int hands, int totalTimesFoldedToCBet, int totalTimesFacedCBet)
            : base(hands)
        {
            this.TotalTimesFoldedToCBet = totalTimesFoldedToCBet;
            this.TotalTimesFacedCBet = totalTimesFacedCBet;
        }

        public bool IsOpportunity { get; private set; }

        public int TotalTimesFoldedToCBet { get; private set; }

        public int TotalTimesFacedCBet { get; private set; }

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

        public override void Update(IGetTurnContext context, string playerName)
        {
            if (context.RoundType != GameRoundType.PreFlop
                && context.PreviousRoundActions.Count(p => p.Round == context.RoundType
                    && p.Action.Type == PlayerActionType.Raise) == 1)
            {
                var preflopRaiser = context.PreviousRoundActions
                    .Where(p => p.Round == GameRoundType.PreFlop)
                    .Cast<PlayerActionAndName?>()
                    .LastOrDefault(p => p.Value.Action.Type == PlayerActionType.Raise);

                if (preflopRaiser.HasValue && preflopRaiser.Value.PlayerName != playerName)
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

                    this.TotalTimesFacedCBet++;
                    this.IsOpportunity = true;
                }
            }
        }

        public override void Update(IGetTurnContext context, PlayerAction madeAction, string playerName)
        {
            if (this.IsOpportunity && madeAction.Type == PlayerActionType.Fold)
            {
                this.TotalTimesFoldedToCBet++;
            }

            this.IsOpportunity = false;
        }

        public override string ToString()
        {
            return $"{this.Amount:0.00}%";
        }

        public override FoldToCBet DeepClone()
        {
            var copy = new FoldToCBet(this.Hands, this.TotalTimesFoldedToCBet, this.TotalTimesFacedCBet);
            copy.IsOpportunity = this.IsOpportunity;
            return copy;
        }

        public override FoldToCBet Sum(FoldToCBet other)
        {
            return new FoldToCBet(
                this.Hands + other.Hands,
                this.TotalTimesFoldedToCBet + other.TotalTimesFoldedToCBet,
                this.TotalTimesFacedCBet + other.TotalTimesFacedCBet);
        }
    }
}