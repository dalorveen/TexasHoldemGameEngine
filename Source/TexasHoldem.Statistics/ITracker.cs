namespace TexasHoldem.Statistics
{
    using TexasHoldem.Logic;

    public interface ITracker
    {
        int Callers { get; }

        bool OpenRaiseOpportunity { get; }

        bool InPosition { get; }

        bool OutOfPosition { get; }

        bool ThreeBetOpportunity(GameRoundType roundType);

        bool FourBetAndMoreOpportunity(GameRoundType roundType);
    }
}
