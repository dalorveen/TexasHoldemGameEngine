namespace TexasHoldem.Statistics
{
    using TexasHoldem.Statistics.Indicators;

    public interface IStats
    {
        PositionalCollection<VPIP> VPIP();
        PositionalCollection<PFR> PFR();
        PositionalCollection<RFI> RFI();
        PositionalCollection<BBper100> BBper100();
        PositionalCollection<WTSD> WTSD();
        PositionalCollection<WSD> WSD();
        PositionalCollection<WWSF> WWSF();
        StreetCollection<ThreeBet> ThreeBet();
        StreetCollection<FourBet> FourBet();
        StreetCollection<CBet> CBet();
        StreetCollection<AFq> AFq();
    }
}
