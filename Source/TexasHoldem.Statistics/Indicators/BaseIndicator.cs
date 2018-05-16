namespace TexasHoldem.Statistics.Indicators
{
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public abstract class BaseIndicator<T> : IUpdate, IAmount
    {
        public BaseIndicator(int hands = 0)
        {
            this.Hands = hands;
        }

        /// <summary>
        /// Gets the total number of hands
        /// </summary>
        /// <value>
        /// Played hands
        /// </value>
        public int Hands { get; private set; }

        public abstract double Amount { get; }

        public virtual void Update(IStartGameContext context)
        {
        }

        public virtual void Update(IStartHandContext context)
        {
            this.Hands++;
        }

        public virtual void Update(IStartRoundContext context)
        {
        }

        public virtual void Update(IGetTurnContext context, IStatsContext statsContext)
        {
        }

        public virtual void Update(IGetTurnContext context, PlayerAction playerAction, IStatsContext statsContext)
        {
        }

        public virtual void Update(IEndRoundContext context)
        {
        }

        public virtual void Update(IEndHandContext context, IStatsContext statsContext)
        {
        }

        public virtual void Update(IEndGameContext context)
        {
        }

        public string ToStreetFormat(double preflop, double flop, double turn, double river)
        {
            return $"PF[{preflop:0.0}%] | " +
                $"F[{flop:0.0}%] | " +
                $"T[{turn:0.0}%] | " +
                $"R[{river:0.0}%]";
        }

        public string ToStreetFormat(double flop, double turn, double river)
        {
            return $"F[{flop:0.0}%] | " +
                $"T[{turn:0.0}%] | " +
                $"R[{river:0.0}%]";
        }

        public string ToPositionFormat(double sb, double bb, double ep, double mp, double co, double btn)
        {
            return $"SB[{sb:0.0}%] | " +
                $"BB[{bb:0.0}%] | " +
                $"EP[{ep:0.0}%] | " +
                $"MP[{mp:0.0}%] | " +
                $"CO[{co:0.0}%] | " +
                $"BTN[{btn:0.0}%]";
        }
    }
}