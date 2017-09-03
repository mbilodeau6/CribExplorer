using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CribExplorer.Model;

namespace CribExplorerTests
{
    [TestClass]
    public class DeckTests
    {
        [TestMethod]
        public void Deck_GetNextCard()
        {
            // Set up
            Deck deck = new Deck();
            IDictionary<CardSuit, int> suitCounts = new Dictionary<CardSuit, int>();
            IDictionary<CardFace, int> faceCounts = new Dictionary<CardFace, int>();

            // Test
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

            // Verify
            foreach(KeyValuePair<CardSuit, int> suitCount in suitCounts)
            {
                Assert.AreEqual(13, suitCount.Value, string.Format("Count of {0}", suitCount.Key.ToString()));
            }

            foreach (KeyValuePair<CardFace, int> faceCount in faceCounts)
            {
                Assert.AreEqual(4, faceCount.Value, string.Format("Count of {0}", faceCount.Key.ToString()));
            }
        }
    }
}
