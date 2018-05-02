namespace TexasHoldem.Statistics
{
    using TexasHoldem.Statistics.Indicators;

    public interface IStats
    {
        SingleStreet<VPIP> VPIP();
        SingleStreet<PFR> PFR();
        SingleStreet<RFI> RFI();
        SingleStreet<BBper100> BBper100();
        SingleStreet<WTSD> WTSD();
        SingleStreet<WSD> WSD();
        SingleStreet<WWSF> WWSF();
        SeveralStreets<ThreeBet> ThreeBet();
        SeveralStreets<FourBet> FourBet();
        SeveralStreets<CBet> CBet();
        SeveralStreets<AFq> AFq();
    }
}
