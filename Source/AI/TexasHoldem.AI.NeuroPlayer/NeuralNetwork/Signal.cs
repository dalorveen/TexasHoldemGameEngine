namespace TexasHoldem.AI.NeuroPlayer.NeuralNetwork
{
    using System;
    using System.Collections.Generic;

    using SharpNeat.Phenomes;

    public class Signal
    {
        public Signal(IBlackBox blackBox)
        {
            this.BlackBox = blackBox ?? throw new ArgumentNullException();
        }

        public IBlackBox BlackBox { get; }

        public double[] GetInput(InputData data)
        {
            var input = new List<double>();

            input.Add(data.ImmediatePotOdds());
            input.Add(data.BetRatio());
            input.Add(data.HasPutMoneyInThePotThisRound() ? 1 : 0);
            input.Add(data.OneBetToCall() ? 1 : 0);
            input.Add(data.TwoOrMoreBetsToCall() ? 1 : 0);
            input.AddRange(data.Street());
            input.Add(data.LastBetsCalledByPlayer() ? 1 : 0);
            input.Add(data.LastActionOfPlayerWasBetOrRaise() ? 1 : 0);
            input.Add(data.IsHeadsUp() ? 1 : 0);
            input.Add(data.IsFirstToAct() ? 1 : 0);
            input.Add(data.IsLastToAct() ? 1 : 0);
            input.AddRange(data.Position());
            input.AddRange(data.MatrixOfOdds());
            input.Add(data.IsStraightDraw() ? 1 : 0);
            input.Add(data.IsFlushDraw() ? 1 : 0);

            if (input.Count != this.BlackBox.InputSignalArray.Length)
            {
                throw new Exception("The number of input signals does not match the number of inputs in the network.");
            }

            return input.ToArray();
        }

        // Out1 = raise; Out2 = check or call; Out3 = size of raise (0..1)
        public ISignalArray GetOutput(InputData data)
        {
            var inputSourceOfSignals = this.GetInput(data);
            var inputSignalArray = this.BlackBox.InputSignalArray;
            var outputSignalArray = this.BlackBox.OutputSignalArray;

            this.BlackBox.ResetState();

            for (int i = 0; i < inputSourceOfSignals.Length; i++)
            {
                inputSignalArray[i] = inputSourceOfSignals[i];
            }

            this.BlackBox.Activate();

            if (!this.BlackBox.IsStateValid)
            {
                // Any black box that gets itself into an invalid state is unlikely to be any good, so lets
                // just bail out here
                // TODO: how to return a zero fitness?
                throw new System.Exception();
            }

            return outputSignalArray;
        }
    }
}
