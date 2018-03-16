namespace TexasHoldem.AI.NeuroPlayer.NeuralNetwork
{
    using System.Collections.Generic;
    using System.Diagnostics;

    using HandEvaluatorExtension;
    using SharpNeat.Phenomes;
    using TexasHoldem.Logic.Players;

    public class Signal
    {
        private readonly Normalization.Normalization normalization;

        public Signal(IStartGameContext context)
        {
            this.normalization = new Normalization.Normalization(context);
        }

        protected IGetTurnContext GetTurnContext { get; private set; }

        protected IBlackBox BlackBox { get; private set; }

        public void Update(ICardAdapter pocket, ICardAdapter communityCards, IGetTurnContext context, IBlackBox box)
        {
            this.normalization.Update(pocket, communityCards, context);
            this.GetTurnContext = context;
            this.BlackBox = box;
        }

        public IList<double> InputSignals()
        {
            var input = new List<double>();
            var hero = this.normalization.Hero();
            var opponents = this.normalization.Opponents();

            input.Add(this.normalization.HandStrength());
            input.Add(this.normalization.NormalizedPot());
            input.Add(this.normalization.NormalizedStreet());
            input.Add(hero.Money);
            input.Add(hero.CurrentRoundBet);
            input.Add(hero.Position);

            foreach (var item in opponents)
            {
                input.Add(item.Money);
                input.Add(item.CurrentRoundBet);
                input.Add(item.InHand);
            }

            return input;
        }

        public IList<double> OutputSignals()
        {
            if (this.BlackBox == null)
            {
                // Out1 = raise
                // Out2 = check or call
                // Out3 = size of raise
                return new List<double> { 0, 0, 0 };
            }
            else
            {
                ISignalArray inputSignalArray = this.BlackBox.InputSignalArray;
                ISignalArray outputSignalArray = this.BlackBox.OutputSignalArray;

                this.BlackBox.ResetState();

                var inputSignals = this.InputSignals();

                for (int i = 0; i < inputSignals.Count; i++)
                {
                    Debug.Assert(inputSignals[i] >= -1.0 && inputSignals[i] <= 1, "Outside the range of acceptable values");
                    inputSignalArray[i] = inputSignals[i];
                }

                this.BlackBox.Activate();

                if (!this.BlackBox.IsStateValid)
                {
                    // Any black box that gets itself into an invalid state is unlikely to be any good, so lets just bail out here
                    // TODO: how to return a zero fitness
                    throw new System.Exception();
                }

                return new List<double> { outputSignalArray[0], outputSignalArray[1], outputSignalArray[2] };
            }
        }
    }
}
