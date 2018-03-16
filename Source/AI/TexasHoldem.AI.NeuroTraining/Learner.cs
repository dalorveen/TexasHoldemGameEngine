namespace TexasHoldem.AI.NeuroTraining
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Xml;

    using HandEvaluatorExtension;
    using SharpNeat.Core;
    using SharpNeat.EvolutionAlgorithms;
    using SharpNeat.Genomes.Neat;
    using SharpNeat.Phenomes;
    using TexasHoldem.AI.NeuroPlayer;
    using TexasHoldem.AI.NeuroPlayer.NeuralNetwork;
    using TexasHoldem.Logic.Players;

    public class Learner : NeuroPlayer
    {
        private Thread sharpNeatThread;

        private NeatEvolutionAlgorithm<NeatGenome> ea;

        private SharpNeatPoker.Evaluator evaluator;

        public override string Name { get; } = "Learner_" + Guid.NewGuid();

        public override int BuyIn { get; } = -1;

        public override void StartGame(IStartGameContext context)
        {
            base.StartGame(context);

            var signal = new Signal(context);
            var populationSize = 1000;

            //var experiment = new SharpNeatPoker.Experiment(@"..\..\PopulationFiles\allAgents.xml");

            var experiment = new SharpNeatPoker.Experiment(
                signal.InputSignals().Count, signal.OutputSignals().Count, populationSize);

            this.evaluator = experiment.Evaluator;
            this.evaluator.StartGame(populationSize);

            this.sharpNeatThread = new Thread(() => this.SharpNeatThreadStart(experiment));
            this.sharpNeatThread.Start();
        }

        public override void StartHand(IStartHandContext context)
        {
            base.StartHand(context);
            this.evaluator.StartHand(context, this.StrengthIndex);
        }

        public override PlayerAction GetTurn(IGetTurnContext context)
        {
            var reaction = base.GetTurn(context);
            this.evaluator.Turn(context, reaction, this.PocketMask, this.CommunityCardsMask);
            return reaction;
        }

        public override void EndHand(IEndHandContext context)
        {
            base.EndHand(context);
            this.evaluator.EndHand(context);

            if (this.ea != null && this.ea.RunState == RunState.Paused)
            {
                var xwSettings = new XmlWriterSettings();

                xwSettings.Indent = true;

                var pathToBestAgent = @"..\..\PopulationFiles\bestAgent.xml";
                var pathToAllAgents = @"..\..\PopulationFiles\allAgents.xml";

                using (XmlWriter xw = XmlWriter.Create(@"..\..\PopulationFiles\bestAgent.xml", xwSettings))
                {
                    NeatGenomeXmlIO.WriteComplete(xw, this.ea.CurrentChampGenome, false);
                    Console.Write($"Player's training is interrupted.\nThe best agent is saved in {pathToBestAgent}\n");
                }

                using (XmlWriter xw = XmlWriter.Create(@"..\..\PopulationFiles\allAgents.xml", xwSettings))
                {
                    NeatGenomeXmlIO.WriteComplete(xw, this.ea.GenomeList, false);
                    Console.Write($"The list of all agents is saved to {pathToAllAgents}\n");
                }

                this.ea.Dispose();
                this.sharpNeatThread.Abort();
                this.ea = null;
            }
        }

        public override IBlackBox Phenome()
        {
            return this.evaluator.GetCurrentPhenome();
        }

        private void SharpNeatThreadStart(SharpNeatPoker.Experiment experiment)
        {
            // Create evolution algorithm and attach update event.
            this.ea = experiment.CreateEvolutionAlgorithm(experiment.GenomeFactory, experiment.GenomeList);
            this.ea.UpdateEvent += new EventHandler(this.EvolutionAlgorithmUpdateEvent);

            // Start algorithm (it will run on a background thread).
            this.ea.StartContinue();
        }

        private void EvolutionAlgorithmUpdateEvent(object sender, EventArgs e)
        {
            var temp = sender as NeatEvolutionAlgorithm<NeatGenome>;

            if (temp.RunState == RunState.Paused)
            {
                Console.WriteLine("Please wait...");
            }
            else
            {
                var str = $"{temp.RunState} [generation {temp.Statistics._generation}]\n" +
                        $"\t[bestFitness {temp.Statistics._maxFitness:N6}] [meanFitness {temp.Statistics._meanFitness:N6}]\n" +
                        $"\t[maxComplexity {temp.Statistics._maxComplexity:N6}] [meanComplexity {temp.Statistics._meanComplexity:N6}]\n";

                Console.Write(str);
            }
        }
    }
}
