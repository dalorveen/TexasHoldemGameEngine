namespace TexasHoldem.Statistics.Indicators
{
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public abstract class BaseIndicator<T> : IDeepCloneable<T>, ISum<T>, IUpdate, IAmount
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

        public virtual void Update(IGetTurnContext context, string playerName)
        {
        }

        public virtual void Update(IGetTurnContext context, PlayerAction playerAction, string playerName)
        {
        }

        public virtual void Update(IEndRoundContext context)
        {
        }

        public virtual void Update(IEndHandContext context, string playerName)
        {
        }

        public virtual void Update(IEndGameContext context)
        {
        }

        public abstract T DeepClone();

        public abstract T Sum(T other);
    }
}