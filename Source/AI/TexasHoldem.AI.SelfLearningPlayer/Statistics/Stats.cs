namespace TexasHoldem.AI.SelfLearningPlayer.Statistics
{
    using System.Linq;

    using TexasHoldem.Logic.Players;

    public class Stats : IStats
    {
        private readonly IGetTurnContext context;

        public Stats(IGetTurnContext context)
        {
            this.context = context;
        }

        public int Callers
        {
            get
            {
                var reverse = this.context.PreviousRoundActions.Reverse();
                return reverse.Count(x => x.Action.Type == PlayerActionType.CheckCall && x.Action.Type != PlayerActionType.Raise);
            }
        }

        public bool OpenRaiseOpportunity
        {
            get
            {
                return this.context.PreviousRoundActions.Count(x => x.Action.Type == PlayerActionType.Raise) == 0;
            }
        }

        public bool PreflopThreeBetOpportunity
        {
            get
            {
                return this.context.PreviousRoundActions.Count(x => x.Action.Type == PlayerActionType.Raise) == 1;
            }
        }

        public bool PreflopFourBetAndMoreOpportunity
        {
            get
            {
                return this.context.PreviousRoundActions.Count(x => x.Action.Type == PlayerActionType.Raise) > 1;
            }
        }

        public bool InPosition
        {
            get
            {
                return this.context.Position > this.context.Opponents.Where(x => x.InHand).Max(s => s.Position);
            }
        }

        public bool OutOfPosition
        {
            get
            {
                return this.context.Position < this.context.Opponents.Where(x => x.InHand).Max(s => s.Position);
            }
        }
    }
}
