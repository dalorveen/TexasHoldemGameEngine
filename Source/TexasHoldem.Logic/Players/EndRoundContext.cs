namespace TexasHoldem.Logic.Players
{
    using System;
    using System.Collections.Generic;

    public class EndRoundContext : IEndRoundContext
    {
        public EndRoundContext(IReadOnlyCollection<PlayerActionAndName> roundActions, GameRoundType completedRoundType)
        {
            this.RoundActions = roundActions;
            this.CompletedRoundType = completedRoundType;
        }

        public IReadOnlyCollection<PlayerActionAndName> RoundActions { get; }

        public GameRoundType CompletedRoundType { get; }
    }
}
