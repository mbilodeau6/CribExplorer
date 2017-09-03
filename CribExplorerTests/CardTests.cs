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
                Assert.AreEqual(test.Item2, (new Card() { Suit = test.Item1 }).Color, string.Format("Testing {0}", test.Item1.ToString()));
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
                Assert.AreEqual(test.Item2, (new Card() { Face = test.Item1 }).Value, string.Format("Testing {0}", test.Item1.ToString()));
            }
        }
    }
}
