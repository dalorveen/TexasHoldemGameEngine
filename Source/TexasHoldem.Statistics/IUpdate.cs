namespace TexasHoldem.Statistics
{
    using TexasHoldem.Logic.Players;

    public interface IUpdate
    {
        void Update(IStartGameContext context);

        void Update(IStartHandContext context);

        void Update(IStartRoundContext context);

        void Update(IGetTurnContext context, string playerName);

        void Update(IGetTurnContext context, PlayerAction playerAction, string playerName);

        void Update(IEndRoundContext context);

        void Update(IEndHandContext context, string playerName);

        void Update(IEndGameContext context);
    }
}