namespace TexasHoldem.Statistics
{
    using TexasHoldem.Statistics.Indicators;

    public interface IStats
    {
        VPIP VPIP();

        PFR PFR();

        RFI RFI();

        BBper100 BBper100();

        WTSD WTSD();

        WSD WSD();

        WWSF WWSF();

        ThreeBet ThreeBet();

        FoldThreeBet FoldThreeBet();

        CallThreeBet CallThreeBet();

        FourBet FourBet();

        FoldFourBet FoldFourBet();

        CBet CBet();

        FoldToCBet FoldToCBet();

        AFq AFq();

        CheckRaise CheckRaise();
    }
}
