using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CribExplorer.Model;

namespace CribExplorer
{
    public class PointCalculator
    {
        private IList<Card> allCards;
        private IDictionary<CardFace, int> faceCounts;
        private IList<Card> cardsInHand;
        private Card starterCard;

        public PointCalculator(Hand hand, Card starter)
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

            faceCounts = new Dictionary<CardFace, int>();

            foreach (Card card in allCards)
            {
                if (faceCounts.ContainsKey(card.Face))
                    faceCounts[card.Face]++;
                else
                    faceCounts.Add(card.Face, 1);
            }
        }

        public int GetStraightPoints()
        {
            int straightSize = 0;
            int multiplier = 1;

            foreach(CardFace face in Enum.GetValues(typeof(CardFace)))
            {
                if (!faceCounts.ContainsKey(face) || faceCounts[face] == 0)
                {
                    if (straightSize > 2)
                        break;
                    else
                        straightSize = 0;

                    multiplier = 1;
                }
                else
                {
                    straightSize++;
                    multiplier *= faceCounts[face];
                }
            }

            if (straightSize > 2)
                return straightSize * multiplier;
            else
                return 0;
        }

        public int GetFifteenPoints()
        {
            return CountFifteens(0, allCards) * 2;
        }

        private int CountFifteens(int sumSoFar, IList<Card> cards)
        {
            int countOf15 = 0;

            if (sumSoFar > 15)
                return 0;

            IList<Card> cardsLeft = new List<Card>(cards);

            while (cardsLeft.Count > 0)
            {
                int newSum = sumSoFar + cardsLeft[0].Value;
                cardsLeft.Remove(cardsLeft[0]);

                if (newSum == 15)
                    countOf15++;

                else if (newSum < 15)
                    countOf15 += CountFifteens(newSum, cardsLeft);
            }

            return countOf15;
        }

        public int GetMatchingJackPoints()
        {
            foreach (Card card in cardsInHand)
                if (card.Face == CardFace.Jack && card.Suit == starterCard.Suit)
                {
                    return 1;
                }

            return 0;
        }

        public int GetPairPoints()
        {
            int points = 0;

            foreach (KeyValuePair<CardFace, int> pair in faceCounts.Where(x => x.Value > 1))
            {
                switch (pair.Value)
                {
                    case 2:
                        points += 2;
                        break;
                    case 3:
                        points += 6;
                        break;
                    case 4:
                        points += 12;
                        break;
                    default:
                        throw new ApplicationException(string.Format("Unexpected count of pairs: {0}", pair.Value));
                }
            }

            return points;
        }

        public int GetFlushPoints()
        {
            if (cardsInHand[0].Suit == cardsInHand[1].Suit &&
                cardsInHand[0].Suit == cardsInHand[2].Suit &&
                cardsInHand[0].Suit == cardsInHand[3].Suit)
            {
                if (starterCard.Suit == cardsInHand[0].Suit)
                    return 5;
                else
                    return 4;
            }

            return 0;
        }

        public int GetAllPoints()
        {
            int points = GetMatchingJackPoints();

            points += GetPairPoints();
            points += GetFlushPoints();
            points += GetFifteenPoints();
            points += GetStraightPoints();

            return points;
        }
    }
}
