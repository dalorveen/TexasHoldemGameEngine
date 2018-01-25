namespace TexasHoldem.AI.SelfLearningPlayer.Statistics
{
    public interface IStats
    {
        int Callers { get; }

        bool OpenRaiseOpportunity { get; }

        bool PreflopThreeBetOpportunity { get; }

        bool PreflopFourBetAndMoreOpportunity { get; }

        bool InPosition { get; }

        bool OutOfPosition { get; }
    }
}
