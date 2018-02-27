namespace TexasHoldem.Statistics.Indicators
{
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;

    public abstract class BaseIndicator : IDeepCloneable<BaseIndicator>
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

        public virtual void StartGameExtract(IStartHandContext context)
        {
        }

        public virtual void StartHandExtract(IStartHandContext context)
        {
            this.Hands++;
        }

        public virtual void StartRoundExtract(IStartRoundContext context)
        {
        }

        public virtual void GetTurnExtract(IGetTurnContext context)
        {
        }

        public virtual void MadeActionExtract(IGetTurnContext context, PlayerAction madeAction)
        {
        }

        public virtual void EndRoundExtract(IEndRoundContext context)
        {
        }

        public virtual void EndHandExtract(IEndHandContext context)
        {
        }

        public virtual void EndGameExtract(IStartHandContext context)
        {
        }

        public abstract BaseIndicator DeepClone();
    }
}
