using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CribExplorer;
using CribExplorer.Model;

namespace CribExplorer
{
    public class HandPointCalculator:PointCalculator
    {
        public HandPointCalculator(Hand hand, Card starter)
        {
            if (hand == null)
                throw new ArgumentNullException("hand");

            if (starter == null)
                throw new ArgumentNullException("starter");

            if (hand.Cards == null || hand.Cards.Count != 4)
                throw new ArgumentException("Hands must have 4 cards to be scored.");

            cardsInHand = hand.Cards;
            starterCard = starter;

            allCards = new List<Card>(hand.Cards);
            allCards.Add(starter);

            base.CalcFaceCounts();
        }

    }
}
