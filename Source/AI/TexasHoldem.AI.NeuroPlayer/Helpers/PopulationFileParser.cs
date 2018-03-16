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

        private readonly int inputCount;

        private readonly int outputCount;

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
                            this.inputCount++;
                        }
                        else if (xr.GetAttribute("type") == "out")
                        {
                            this.outputCount++;
                        }
                    }
                    else if (xr.NodeType == XmlNodeType.EndElement && xr.Name == "Nodes")
                    {
                        break;
                    }
                }
            }
        }

        public IBlackBox BestPhenome()
        {
            var genomeFactory = new NeatGenomeFactory(this.inputCount, this.outputCount);
            List<NeatGenome> genomeList;

            using (XmlReader xr = XmlReader.Create(this.xmlPopulationFile))
            {
                genomeList = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, genomeFactory);
            }

            var decoder = new NeatGenomeDecoder(NetworkActivationScheme.CreateCyclicFixedTimestepsScheme(1));

            return decoder.Decode(genomeList[0]);
        }
    }
}
