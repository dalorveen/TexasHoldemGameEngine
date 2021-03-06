﻿namespace TexasHoldem.AI.NeuroPlayer
{
    using System;
    using System.Linq;

    using HandEvaluatorExtension;
    using SharpNeat.Phenomes;
    using TexasHoldem.Logic.Players;

    public class NeuroPlayer : BasePlayer
    {
        private ICardAdapter pocket;

        private ICardAdapter communityCards;

        private NeuralNetwork.Reaction reaction;

        public NeuroPlayer(string xmlPopulationFile)
        {
            var parser = new Helpers.PopulationFileParser(xmlPopulationFile);

            this.Phenome = parser.BestPhenome();
        }

        protected NeuroPlayer(IBlackBox phenome)
        {
            this.Phenome = phenome;
        }

        public override string Name { get; } = "NeuroPlayer_" + Guid.NewGuid();

        public override int BuyIn { get; } = -1;

        public ulong PocketMask
        {
            get
            {
                return this.pocket.Mask;
            }
        }

        public ulong CommunityCardsMask
        {
            get
            {
                return this.communityCards.Mask;
            }
        }

        public IBlackBox Phenome { get; }

        public override void StartGame(IStartGameContext context)
        {
            base.StartGame(context);

            this.reaction = new NeuralNetwork.Reaction(context);
        }

        public override void StartHand(IStartHandContext context)
        {
            base.StartHand(context);

            this.pocket = new CardAdapter(new[] { context.FirstCard, context.SecondCard });
        }

        public override void StartRound(IStartRoundContext context)
        {
            base.StartRound(context);

            this.communityCards = new CardAdapter(context.CommunityCards.ToList());
        }

        public override PlayerAction PostingBlind(IPostingBlindContext context)
        {
            return context.BlindAction;
        }

        public override PlayerAction GetTurn(IGetTurnContext context)
        {
            this.reaction.Update(this.pocket, this.communityCards, context, this.Phenome);

            return this.reaction.React();
        }
    }
}