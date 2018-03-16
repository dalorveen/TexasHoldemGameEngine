namespace TexasHoldem.Statistics
{
    using TexasHoldem.Statistics.Indicators;

    public interface IStats
    {
        VPIP VPIP { get; }

        PFR PFR { get; }

        PositionStorage<RFI> RFI { get; }

        StreetStorage<ThreeBet> ThreeBet { get; }

        StreetStorage<FourBet> FourBet { get; }

        StreetStorage<CBet> CBet { get; }

        StreetStorage<AFq> AFq { get; }

        BBper100 BBper100 { get; }

        WTSD WTSD { get; }

        WSD WSD { get; }

        WWSF WWSF { get; }
    }
}
