using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CribExplorer.Model;

namespace CribExplorerTests
{
    [TestClass]
    public class DeckTests
    {
        private bool AllCardsPresent(Deck deck, out string error)
        {
            error = string.Empty;

            IDictionary<CardSuit, int> suitCounts = new Dictionary<CardSuit, int>();
            IDictionary<CardFace, int> faceCounts = new Dictionary<CardFace, int>();

            Card nextCard = deck.GetNextCard();

            while (nextCard != null)
            {
                if (!suitCounts.ContainsKey(nextCard.Suit))
                    suitCounts.Add(nextCard.Suit, 1);
                else
                    suitCounts[nextCard.Suit]++;

                if (!faceCounts.ContainsKey(nextCard.Face))
                    faceCounts.Add(nextCard.Face, 1);
                else
                    faceCounts[nextCard.Face]++;

                nextCard = deck.GetNextCard();
            }

            foreach (KeyValuePair<CardSuit, int> suitCount in suitCounts)
            {
                if (suitCount.Value != 13)
                {
                    error = string.Format("Invalid count of {0} for {1}.", suitCount.Value, suitCount.Key.ToString());
                    return false;
                }
            }

            foreach (KeyValuePair<CardFace, int> faceCount in faceCounts)
            {
                if (faceCount.Value != 4)
                {
                    error = string.Format("Invalid count of {0} for {1}.", faceCount.Value, faceCount.Key.ToString());
                    return false;
                }
            }

            return true;
        }
        
        [TestMethod]
        public void Deck_GetNextCard()
        {
            Deck deck = new Deck();

            string error;

            Assert.IsTrue(AllCardsPresent(deck, out error), error);
        }

        [TestMethod]
        public void Deck_Shuffle()
        {
            // Initialize objects
            Deck deck = new Deck();
            List<HashSet<Card>> uniqueCounts = new List<HashSet<Card>>();

            for (int i = 0; i < 52; i++)
                uniqueCounts.Add(new HashSet<Card>());

            // Run test
            int numTests = 10;

            for (int i = 0; i < numTests; i++)
            {
                deck.Shuffle();

                Card nextCard = deck.GetNextCard();

                int j = 0;
                while (nextCard != null)
                {
                    uniqueCounts[j].Add(nextCard);
                    j++;

                    nextCard = deck.GetNextCard();
                }

                Assert.AreEqual(52, j, string.Format("Incorrect number of cards drawn in iteration {0}", j));
            }

            // Summarize results
            int uniqueCount3orMore = 0;
            int maxUniqueCount = 0;

            for (int i = 0; i < 52; i++)
            {
                Assert.IsTrue(uniqueCounts[i].Count >= 1, string.Format("Drawn card {0} must have at least one unique value.", i));

                if (uniqueCounts[i].Count >= 3)
                    uniqueCount3orMore++;

                if (uniqueCounts[i].Count > maxUniqueCount)
                    maxUniqueCount = uniqueCounts[i].Count;
            }

            // Test
            // TODO: It is possible for this test to fail (could have the same card 
            // show up in the same position numTest times in a row). Should figure 
            // out a way to set up the test so that it never fails.
            Assert.IsTrue(uniqueCount3orMore == 52);
            Assert.IsTrue(maxUniqueCount > numTests / 2);
        }
    }
}
