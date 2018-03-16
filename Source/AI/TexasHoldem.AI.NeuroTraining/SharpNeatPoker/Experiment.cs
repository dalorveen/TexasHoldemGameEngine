namespace TexasHoldem.AI.NeuroTraining.SharpNeatPoker
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Xml;

    using SharpNeat.Core;
    using SharpNeat.Decoders;
    using SharpNeat.Decoders.Neat;
    using SharpNeat.DistanceMetrics;
    using SharpNeat.EvolutionAlgorithms;
    using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
    using SharpNeat.Genomes.Neat;
    using SharpNeat.Phenomes;
    using SharpNeat.SpeciationStrategies;

    public class Experiment
    {
        private NetworkActivationScheme activationScheme;

        private ParallelOptions parallelOptions;

        private NeatEvolutionAlgorithmParameters eaParams;

        private NeatGenomeParameters neatGenomeParams;

        public Experiment(int inputCount, int outputCount, int populationSize)
        {
            this.InputCount = inputCount;
            this.OutputCount = outputCount;

            this.Initialize();

            // Create a genome factory with our neat genome parameters object and
            // the appropriate number of input and output neuron genes.
            this.GenomeFactory = this.CreateGenomeFactory();

            // Create an initial population of randomly generated genomes.
            this.GenomeList = this.GenomeFactory.CreateGenomeList(populationSize, 0);
        }

        public Experiment(string populationFile)
        {
            this.Initialize();

            using (XmlReader xr = XmlReader.Create(populationFile))
            {
                while (xr.Read())
                {
                    if (xr.NodeType == XmlNodeType.Element && xr.Name == "Node")
                    {
                        if (xr.GetAttribute("type") == "in")
                        {
                            this.InputCount++;
                        }
                        else if (xr.GetAttribute("type") == "out")
                        {
                            this.OutputCount++;
                        }
                    }
                    else if (xr.NodeType == XmlNodeType.EndElement && xr.Name == "Nodes")
                    {
                        break;
                    }
                }
            }

            using (XmlReader xr = XmlReader.Create(populationFile))
            {
                this.GenomeList = this.LoadPopulation(xr);
            }

            this.GenomeFactory = this.GenomeList[0].GenomeFactory;
        }

        public int InputCount { get; private set; }

        public int OutputCount { get; private set; }

        public Evaluator Evaluator { get; private set; }

        public IGenomeFactory<NeatGenome> GenomeFactory { get; private set; }

        public List<NeatGenome> GenomeList { get; private set; }

        public List<NeatGenome> LoadPopulation(XmlReader xr)
        {
            NeatGenomeFactory genomeFactory = (NeatGenomeFactory)this.CreateGenomeFactory();
            return NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, genomeFactory);
        }

        public IGenomeDecoder<NeatGenome, IBlackBox> CreateGenomeDecoder()
        {
            return new NeatGenomeDecoder(this.activationScheme);
        }

        public IGenomeFactory<NeatGenome> CreateGenomeFactory()
        {
            return new NeatGenomeFactory(this.InputCount, this.OutputCount, this.neatGenomeParams);
        }

        public NeatEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(
            IGenomeFactory<NeatGenome> genomeFactory, List<NeatGenome> genomeList)
        {
            // Create distance metric. Mismatched genes have a fixed distance of 10;
            // for matched genes the distance is their weight difference.
            IDistanceMetric distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 10.0);
            ISpeciationStrategy<NeatGenome> speciationStrategy = new KMeansClusteringStrategy<NeatGenome>(distanceMetric);

            // Create complexity regulation strategy.
            IComplexityRegulationStrategy complexityRegulationStrategy = new NullComplexityRegulationStrategy();

            // Create the evolution algorithm.
            NeatEvolutionAlgorithm<NeatGenome> ea =
                new NeatEvolutionAlgorithm<NeatGenome>(this.eaParams, speciationStrategy, complexityRegulationStrategy);

            // Create genome decoder.
            IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder = this.CreateGenomeDecoder();

            // Create a genome list evaluator. This packages up the genome decoder with the genome evaluator.
            IGenomeListEvaluator<NeatGenome> innerEvaluator =
                new SerialGenomeListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, this.Evaluator);

            // Wrap the list evaluator in a 'selective' evaluator that will only evaluate new genomes.
            // That is, we skip re-evaluating any genomes that were in the population in previous
            // generations (elite genomes). This is determined by examining each genome's evaluation info object.
            IGenomeListEvaluator<NeatGenome> selectiveEvaluator =
                new SelectiveGenomeListEvaluator<NeatGenome>(
                    innerEvaluator,
                    SelectiveGenomeListEvaluator<NeatGenome>.CreatePredicate_OnceOnly());

            // Initialize the evolution algorithm.
            ea.Initialize(selectiveEvaluator, genomeFactory, genomeList);
            ea.UpdateScheme = new UpdateScheme(1);
            var ttt = ea.CurrentChampGenome;

            // Finished. Return the evolution algorithm
            return ea;
        }

        private void Initialize()
        {
            this.activationScheme = NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(1);
            this.parallelOptions = new ParallelOptions();

            this.eaParams = new NeatEvolutionAlgorithmParameters();
            this.eaParams.SpecieCount = 10;                     // default: 10
            this.eaParams.ElitismProportion = 0.1;              // default: 0.2
            this.eaParams.SelectionProportion = 0.3;            // default: 0.2
            this.eaParams.OffspringAsexualProportion = 0.40;    // default:0.5
            this.eaParams.OffspringSexualProportion = 0.60;     // default:0.5
            this.eaParams.InterspeciesMatingProportion = 0.01;  // default: 0.01

            this.neatGenomeParams = new NeatGenomeParameters();
            this.neatGenomeParams.ConnectionWeightRange = 7.0;                 // default: 5
            this.neatGenomeParams.ConnectionWeightMutationProbability = 0.90;   // default: 0.94;
            this.neatGenomeParams.AddNodeMutationProbability = 0.01;           // default: 0.01;
            this.neatGenomeParams.AddConnectionMutationProbability = 0.050;    // default: 0.025;
            this.neatGenomeParams.DeleteConnectionMutationProbability = 0.025;  // default: 0.025;

            this.neatGenomeParams.FeedforwardOnly = this.activationScheme.AcyclicNetwork;

            this.Evaluator = new Evaluator();
        }
    }
}
