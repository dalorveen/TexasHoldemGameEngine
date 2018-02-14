namespace TexasHoldem.Statistics
{
    using TexasHoldem.Statistics.Indicators;

    public interface IStats
    {
        VPIP VPIP { get; }

        PFR PFR { get; }

        ThreeBet ThreeBet { get; }

        FourBet FourBet { get; }

        CBet CBet { get; }

        AFq AFq { get; }

        BBper100 BBper100 { get; }

        WTSD WTSD { get; }
    }
}
