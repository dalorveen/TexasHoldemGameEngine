namespace TexasHoldem.Statistics.Tests.Indicators
{
    using System.Collections.Generic;

    using Moq;
    using NUnit.Framework;
    using TexasHoldem.Logic;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics.Indicators;

    [TestFixture]
    public class CBetTests
    {
        [Test]
        public void ShouldReturnTrueIfThePlayerWasPreflopAggressorAndTheFlopDidNotYetHaveABet()
        {
            var cbet = new CBet("hero");

            var mockedEndRoundContext = new Mock<IEndRoundContext>();
            mockedEndRoundContext.SetupGet(x => x.RoundActions).Returns(
                new List<PlayerActionAndName>
                {
                    new PlayerActionAndName("opponent", PlayerAction.Post(1), GameRoundType.PreFlop),
                    new PlayerActionAndName("hero", PlayerAction.Post(2), GameRoundType.PreFlop),
                    new PlayerActionAndName("opponent", PlayerAction.CheckOrCall(), GameRoundType.PreFlop),
                    new PlayerActionAndName("hero", PlayerAction.Raise(10), GameRoundType.PreFlop),
                    new PlayerActionAndName("opponent", PlayerAction.CheckOrCall(), GameRoundType.PreFlop)
                });
            mockedEndRoundContext.SetupGet(x => x.CompletedRoundType).Returns(GameRoundType.PreFlop);
            cbet.EndRoundExtract(mockedEndRoundContext.Object);

            var mockedGetTurnContext = new Mock<IGetTurnContext>();
            mockedGetTurnContext.SetupGet(x => x.PreviousRoundActions).Returns(
                new List<PlayerActionAndName>
                {
                    new PlayerActionAndName("opponent", PlayerAction.CheckOrCall(), GameRoundType.Flop)
                });
            mockedGetTurnContext.SetupGet(x => x.RoundType).Returns(GameRoundType.Flop);
            cbet.GetTurnExtract(mockedGetTurnContext.Object);

            Assert.IsTrue(cbet.TotalOpportunities.F == 1);
        }
    }
}
