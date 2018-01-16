using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CribExplorer;
using CribExplorer.Model;

namespace CribExplorerTests
{
    [TestClass]
    public class PegPointCalculatorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PegPointCalculator_Constructor_NullHand()
        {
            PegPointCalculator pointCalc = new PegPointCalculator(null, 0, false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PegPointCalculator_Constructor_NullCards()
        {
            Hand hand = new Hand();

            PegPointCalculator pointCalc = new PegPointCalculator(hand, 0, false);
        }

        [TestMethod]
        public void PegPointCalculator_Constructor()
        {
            Hand hand = new Hand();
            hand.Cards.Add(new Card(CardSuit.Club, CardFace.Ace));

            PegPointCalculator pointCalc = new PegPointCalculator(hand, 0, false);
        }
    }
}
