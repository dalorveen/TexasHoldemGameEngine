namespace TexasHoldem.AI.NeuroPlayer.Helpers
{
    using System.Collections.Generic;
    using System.Xml;

    using SharpNeat.Decoders;
    using SharpNeat.Decoders.Neat;
    using SharpNeat.Genomes.Neat;
    using SharpNeat.Phenomes;

    public class PopulationFileParser
    {
        private readonly string xmlPopulationFile;

        public PopulationFileParser(string xmlPopulationFile)
        {
            this.xmlPopulationFile = xmlPopulationFile;

            using (XmlReader xr = XmlReader.Create(this.xmlPopulationFile))
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
        }

        public int InputCount { get; }

        public int OutputCount { get; }

        public IBlackBox BestPhenome()
        {
            var genomeFactory = new NeatGenomeFactory(this.InputCount, this.OutputCount, new NeatGenomeParameters());

            //var genomeFactory = new SharpNeat.Genomes.HyperNeat.CppnGenomeFactory(
            //    this.InputCount,
            //    this.OutputCount,
            //    SharpNeat.Network.DefaultActivationFunctionLibrary.CreateLibraryCppn(),
            //    new NeatGenomeParameters());

            List<NeatGenome> genomeList;

            using (XmlReader xr = XmlReader.Create(this.xmlPopulationFile))
            {
                genomeList = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, genomeFactory);
            }

            //var decoder = new NeatGenomeDecoder(NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(1));
            var decoder = new NeatGenomeDecoder(NetworkActivationScheme.CreateAcyclicScheme());

            return decoder.Decode(genomeList[0]);
        }
    }
}
