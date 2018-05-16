namespace TexasHoldem.Statistics
{
    using TexasHoldem.Logic.Players;

    public interface IUpdate
    {
        void Update(IStartGameContext context);

        void Update(IStartHandContext context);

        void Update(IStartRoundContext context);

        void Update(IGetTurnContext context, IStatsContext statsContext);

        void Update(IGetTurnContext context, PlayerAction playerAction, IStatsContext statsContext);

        void Update(IEndRoundContext context);

        void Update(IEndHandContext context, IStatsContext statsContext);

        void Update(IEndGameContext context);
    }
}