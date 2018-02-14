namespace TexasHoldem.Statistics
{
    using TexasHoldem.Logic.Players;

    public interface IGetTurnExtendedContext : IGetTurnContext
    {
        IStats CurrentStats { get; }
    }
}
