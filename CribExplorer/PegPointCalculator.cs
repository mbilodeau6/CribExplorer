using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CribExplorer;
using CribExplorer.Model;

namespace CribExplorer
{
    public class PegPointCalculator:PointCalculator
    {
        public PegPointCalculator(Hand hand, int sumOfPlayedCards, bool cardsPlayable)
        {
            if (hand == null)
                throw new ArgumentNullException("hand");

            if (hand.Cards == null || hand.Cards.Count == 0)
                throw new ArgumentException("No cards found in hand.");

            cardsInHand = hand.Cards;

            allCards = new List<Card>(hand.Cards);

            base.CalcFaceCounts();
        }
    }
}
