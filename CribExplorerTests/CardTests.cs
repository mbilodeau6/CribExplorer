using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CribExplorer.Model;

namespace CribExplorerTests
{
    [TestClass]
    public class CardTests
    {
        [TestMethod]
        public void Card_Color()
        {
            IEnumerable<Tuple<CardSuit, CardColor>> tests = new List<Tuple<CardSuit, CardColor>>()
            {
                new Tuple<CardSuit, CardColor>(CardSuit.Club, CardColor.Black),
                new Tuple<CardSuit, CardColor>(CardSuit.Diamond, CardColor.Red),
                new Tuple<CardSuit, CardColor>(CardSuit.Heart, CardColor.Red),
                new Tuple<CardSuit, CardColor>(CardSuit.Spade, CardColor.Black),
            };
            
            foreach(Tuple<CardSuit, CardColor> test in tests)
            {
                Assert.AreEqual(test.Item2, (new Card(test.Item1, CardFace.Ace)).Color, string.Format("Testing {0}", test.Item1.ToString()));
            }
        }

        [TestMethod]
        public void Card_Value()
        {
            IEnumerable<Tuple<CardFace, int>> tests = new List<Tuple<CardFace, int>>()
            {
                new Tuple<CardFace, int>(CardFace.Ace, 1),
                new Tuple<CardFace, int>(CardFace.Two, 2),
                new Tuple<CardFace, int>(CardFace.Three, 3),
                new Tuple<CardFace, int>(CardFace.Four, 4),
                new Tuple<CardFace, int>(CardFace.Five, 5),
                new Tuple<CardFace, int>(CardFace.Six, 6),
                new Tuple<CardFace, int>(CardFace.Seven, 7),
                new Tuple<CardFace, int>(CardFace.Eight, 8),
                new Tuple<CardFace, int>(CardFace.Nine, 9),
                new Tuple<CardFace, int>(CardFace.Ten, 10),
                new Tuple<CardFace, int>(CardFace.Jack, 10),
                new Tuple<CardFace, int>(CardFace.Queen, 10),
                new Tuple<CardFace, int>(CardFace.King, 10),
            };

            foreach (Tuple<CardFace, int> test in tests)
            {
                Assert.AreEqual(test.Item2, (new Card(CardSuit.Heart, test.Item1)).Value, string.Format("Testing {0}", test.Item1.ToString()));
            }
        }

        [TestMethod]
        public void Card_GetHashCode()
        {
            HashSet<int> hashCodes = new HashSet<int>();

            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardFace face in Enum.GetValues(typeof(CardFace)))
                {
                    hashCodes.Add((new Card(suit, face)).GetHashCode());
                }
            }

            int minValue = int.MaxValue;
            int maxValue = int.MinValue;

            foreach(int hashCode in hashCodes)
            {
                minValue = Math.Min(hashCode, minValue);
                maxValue = Math.Max(hashCode, maxValue);
            }

            Assert.AreEqual(52, hashCodes.Count, "Count of unique hash codes");
            Assert.AreEqual(1, minValue, "Min hash code");
            Assert.AreEqual(52, maxValue, "Max hash code");
        }

        [TestMethod]
        public void Card_Equals()
        {
            Card card1 = new Card(CardSuit.Heart, CardFace.Ace);
            Card card2 = new Card(CardSuit.Heart, CardFace.Two);
            Card card3 = new Card(CardSuit.Diamond, CardFace.Ace);
            Card card4 = new Card(CardSuit.Heart, CardFace.Ace);

            Assert.IsTrue(card1.Equals(card4), "HA == HA");
            Assert.IsFalse(card1.Equals(card2), "HA != H2");
            Assert.IsFalse(card1.Equals(card3), "HA != DA");
        }

        [TestMethod]
        public void Card_EqualityForKey()
        {
            HashSet<Card> uniqueCards = new HashSet<Card>();

            Assert.AreEqual(0, uniqueCards.Count, "Before anything added.");

            uniqueCards.Add(new Card(CardSuit.Heart, CardFace.Ace));
            uniqueCards.Add(new Card(CardSuit.Heart, CardFace.Two));

            Assert.AreEqual(2, uniqueCards.Count, "After adding two unique.");

            uniqueCards.Add(new Card(CardSuit.Heart, CardFace.Ace));
            Assert.AreEqual(2, uniqueCards.Count, "After adding card that already exists.");

            uniqueCards.Add(new Card(CardSuit.Diamond, CardFace.Ace));
            Assert.AreEqual(3, uniqueCards.Count, "After adding card of same face but different suit.");
        }
    }
}
