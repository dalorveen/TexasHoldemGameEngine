namespace TexasHoldem.AI.NeuroPlayer
{
    using System;

    using SharpNeat.Phenomes;
    using TexasHoldem.AI.NeuroPlayer.NeuralNetwork;
    using TexasHoldem.Logic.Players;
    using TexasHoldem.Statistics;

    public class NeuroPlayer : BasePlayer
    {
        private readonly Signal signal;

        public NeuroPlayer(string xmlPopulationFile)
        {
            var parser = new Helpers.PopulationFileParser(xmlPopulationFile);
            this.Stats = new Stats(this.Name);
            this.InputData = new InputData(this.Name);
            this.signal = new Signal(parser.BestPhenome());
        }

        protected NeuroPlayer(IBlackBox phenome)
        {
            this.Stats = new Stats(this.Name);
            this.InputData = new InputData(this.Name);
            this.signal = new Signal(phenome);
        }

        public Stats Stats { get; private set; }

        public InputData InputData { get; private set; }

        public override string Name { get; } = "NeuroPlayer_" + Guid.NewGuid();

        public override int BuyIn { get; } = -1;

        public override void StartGame(IStartGameContext context)
        {
            this.Stats.Update(context);
            this.InputData.StartGameContext = context;
        }

        public override void StartHand(IStartHandContext context)
        {
            base.StartHand(context);
            this.Stats.Update(context);
            this.InputData.StartHandContext = context;
        }

        public override void StartRound(IStartRoundContext context)
        {
            base.StartRound(context);
            this.Stats.Update(context);
            this.InputData.StartRoundContext = context;
        }

        public override PlayerAction PostingBlind(IPostingBlindContext context)
        {
            return context.BlindAction;
        }

        public override PlayerAction GetTurn(IGetTurnContext context)
        {
            this.Stats.Update(context);
            this.InputData.TurnContext = context;

            var networkResponse = this.signal.GetOutput(this.InputData);
            PlayerAction playerAction;

            if (networkResponse[0] > 0.75 && networkResponse[1] <= 0.25)
            {
                // A signal to raise
                var moneyToRaise = context.MinRaise + (int)(context.MoneyLeft * networkResponse[2]);

                if (context.CanRaise)
                {
                    if (moneyToRaise >= context.MoneyLeft - context.MoneyToCall)
                    {
                        // All-In
                        playerAction = PlayerAction.Raise(context.MoneyLeft - context.MoneyToCall);
                    }
                    else
                    {
                        playerAction = PlayerAction.Raise(moneyToRaise);
                    }
                }
                else
                {
                    playerAction = PlayerAction.CheckOrCall();
                }
            }
            else if (networkResponse[0] <= 0.25 && networkResponse[1] > 0.75)
            {
                // A signal to check or call
                playerAction = PlayerAction.CheckOrCall();
            }
            else
            {
                if (context.CanCheck)
                {
                    playerAction = PlayerAction.CheckOrCall();
                }
                else
                {
                    playerAction = PlayerAction.Fold();
                }
            }

            this.Stats.Update(context, playerAction);

            return playerAction;
        }

        public override void EndRound(IEndRoundContext context)
        {
            this.Stats.Update(context);
        }

        public override void EndHand(IEndHandContext context)
        {
            this.Stats.Update(context);
        }

        public override void EndGame(IEndGameContext context)
        {
            this.Stats.Update(context);
        }
    }
}