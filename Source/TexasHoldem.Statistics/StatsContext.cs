namespace TexasHoldem.Statistics
{
    public class StatsContext : IStatsContext
    {
        public StatsContext(string playerName, TablePosition position)
        {
            this.PlayerName = playerName;
            this.Position = position;
        }

        public string PlayerName { get; }

        public TablePosition Position { get; }
    }
}
