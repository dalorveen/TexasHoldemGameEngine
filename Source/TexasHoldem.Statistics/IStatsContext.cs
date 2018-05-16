namespace TexasHoldem.Statistics
{
    public interface IStatsContext
    {
        string PlayerName { get; }

       TablePosition Position { get;}
    }
}
