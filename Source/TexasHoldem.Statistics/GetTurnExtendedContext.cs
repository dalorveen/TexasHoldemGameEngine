namespace TexasHoldem.Statistics
{
    using System.Collections.Generic;

    using TexasHoldem.Logic;
    using TexasHoldem.Logic.GameMechanics;
    using TexasHoldem.Logic.Players;

    public class GetTurnExtendedContext : IGetTurnExtendedContext
    {
        private readonly IGetTurnContext getTurnContext;

        public GetTurnExtendedContext(IGetTurnContext getTurnContext, IStats stats)
        {
            this.getTurnContext = getTurnContext;
            this.CurrentStats = stats;
        }

        public ICollection<PlayerActionType> AvailablePlayerOptions
        {
            get
            {
                return this.getTurnContext.AvailablePlayerOptions;
            }
        }

        public bool CanCheck
        {
            get
            {
                return this.getTurnContext.CanCheck;
            }
        }

        public int CurrentMaxBet
        {
            get
            {
                return this.getTurnContext.CurrentMaxBet;
            }
        }

        public int CurrentPot
        {
            get
            {
                return this.getTurnContext.CurrentPot;
            }
        }

        public bool IsAllIn
        {
            get
            {
                return this.getTurnContext.IsAllIn;
            }
        }

        public Pot MainPot
        {
            get
            {
                return this.getTurnContext.MainPot;
            }
        }

        public int MinRaise
        {
            get
            {
                return this.getTurnContext.MinRaise;
            }
        }

        public int MoneyLeft
        {
            get
            {
                return this.getTurnContext.MoneyLeft;
            }
        }

        public int MoneyToCall
        {
            get
            {
                return this.getTurnContext.MoneyToCall;
            }
        }

        public int MyMoneyInTheRound
        {
            get
            {
                return this.getTurnContext.MyMoneyInTheRound;
            }
        }

        public ICollection<Opponent> Opponents
        {
            get
            {
                return this.getTurnContext.Opponents;
            }
        }

        public IReadOnlyCollection<PlayerActionAndName> PreviousRoundActions
        {
            get
            {
                return this.getTurnContext.PreviousRoundActions;
            }
        }

        public GameRoundType RoundType
        {
            get
            {
                return this.getTurnContext.RoundType;
            }
        }

        public IReadOnlyCollection<Pot> SidePots
        {
            get
            {
                return this.getTurnContext.SidePots;
            }
        }

        public int SmallBlind
        {
            get
            {
                return this.getTurnContext.SmallBlind;
            }
        }

        public IStats CurrentStats { get; }
    }
}
