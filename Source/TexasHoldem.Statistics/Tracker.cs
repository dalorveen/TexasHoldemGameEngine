namespace TexasHoldem.Statistics
{
    using System.Linq;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public class Tracker : ITracker
    {
        private readonly IGetTurnContext context;

        public Tracker(IGetTurnContext context)
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

        public bool ThreeBetOpportunity(GameRoundType roundType)
        {
            return this.context.PreviousRoundActions.Count(x => x.Action.Type == PlayerActionType.Raise)
                == (roundType == GameRoundType.PreFlop ? 1 : 2);
        }

        public bool FourBetAndMoreOpportunity(GameRoundType roundType)
        {
            var count = this.context.PreviousRoundActions.Count(x => x.Action.Type == PlayerActionType.Raise);
            if (roundType == GameRoundType.PreFlop)
            {
                return count > 1;
            }
            else
            {
                return count > 2;
            }
        }
    }
}
