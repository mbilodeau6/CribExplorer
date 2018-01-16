using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CribExplorer;
using CribExplorer.Model;

namespace CribExplorerTests
{
    [TestClass]
    public class HandPointCalculatorTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HandPointCalculator_Constructor_NullHand()
        {
            HandPointCalculator pointCalc = new HandPointCalculator(null, new Card(CardSuit.Club, CardFace.Ace));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HandPointCalculator_Constructor_NullStarter()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>() 
            {
                new Card(CardSuit.Club, CardFace.Ace)
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HandPointCalculator_Constructor_NullCards()
        {
            Hand hand = new Hand();

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Club, CardFace.Ace));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HandPointCalculator_Constructor_EmptyHand()
        {
            Hand hand = new Hand();
            hand.Cards = new List<Card>();

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Club, CardFace.Ace));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HandPointCalculator_Constructor_HandTooLarge()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Club, CardFace.Ace),
                new Card(CardSuit.Diamond, CardFace.Ace),
                new Card(CardSuit.Heart, CardFace.Ace),
                new Card(CardSuit.Spade, CardFace.Ace),
                new Card(CardSuit.Spade, CardFace.Two)
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Club, CardFace.Two));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HandPointCalculator_Constructor_HandTooSmall()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Ace),
                new Card(CardSuit.Heart, CardFace.Ace),
                new Card(CardSuit.Spade, CardFace.Ace)
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Club, CardFace.Ace));
        }

        [TestMethod]
        public void HandPointCalculator_GetAllPoints_Zero()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Two),
                new Card(CardSuit.Heart, CardFace.Four),
                new Card(CardSuit.Spade, CardFace.Six),
                new Card(CardSuit.Spade, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Club, CardFace.Ten));

            Assert.AreEqual(0, pointCalc.GetAllPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetAllPoints_ComboWithStraights()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Six),
                new Card(CardSuit.Heart, CardFace.Jack),
                new Card(CardSuit.Spade, CardFace.Nine),
                new Card(CardSuit.Spade, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Heart, CardFace.Seven));

            Assert.AreEqual(9, pointCalc.GetAllPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetAllPoints_ComboWithoutStraights()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Spade, CardFace.Two),
                new Card(CardSuit.Diamond, CardFace.Three),
                new Card(CardSuit.Spade, CardFace.Queen),
                new Card(CardSuit.Spade, CardFace.Three),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Diamond, CardFace.Two));

            Assert.AreEqual(12, pointCalc.GetAllPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetMatchingJackPoints_Match()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Two),
                new Card(CardSuit.Heart, CardFace.Jack),
                new Card(CardSuit.Spade, CardFace.Six),
                new Card(CardSuit.Spade, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Heart, CardFace.Ten));

            Assert.AreEqual(1, pointCalc.GetMatchingJackPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetMatchingJackPoints_NoMatch()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Two),
                new Card(CardSuit.Heart, CardFace.Jack),
                new Card(CardSuit.Spade, CardFace.Six),
                new Card(CardSuit.Spade, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Club, CardFace.Ten));

            Assert.AreEqual(0, pointCalc.GetMatchingJackPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetPairPoints_Pair()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Two),
                new Card(CardSuit.Heart, CardFace.Three),
                new Card(CardSuit.Spade, CardFace.Eight),
                new Card(CardSuit.Club, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Heart, CardFace.Nine));

            Assert.AreEqual(2, pointCalc.GetPairPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetPairPoints_Triple()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Two),
                new Card(CardSuit.Heart, CardFace.Three),
                new Card(CardSuit.Spade, CardFace.Eight),
                new Card(CardSuit.Club, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Heart, CardFace.Eight));

            Assert.AreEqual(6, pointCalc.GetPairPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetPairPoints_FourOfKind()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Eight),
                new Card(CardSuit.Heart, CardFace.Three),
                new Card(CardSuit.Spade, CardFace.Eight),
                new Card(CardSuit.Club, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Heart, CardFace.Eight));

            Assert.AreEqual(12, pointCalc.GetPairPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetPairPoints_TwoPairs()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Two),
                new Card(CardSuit.Heart, CardFace.Three),
                new Card(CardSuit.Spade, CardFace.Three),
                new Card(CardSuit.Club, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Heart, CardFace.Eight));

            Assert.AreEqual(4, pointCalc.GetPairPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetPairPoints_NoPairs()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Heart, CardFace.Six),
                new Card(CardSuit.Heart, CardFace.Seven),
                new Card(CardSuit.Heart, CardFace.Eight),
                new Card(CardSuit.Heart, CardFace.Nine),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Heart, CardFace.Ten));

            Assert.AreEqual(0, pointCalc.GetPairPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetFlushPoints_Flush4()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Two),
                new Card(CardSuit.Diamond, CardFace.Four),
                new Card(CardSuit.Diamond, CardFace.Six),
                new Card(CardSuit.Diamond, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Heart, CardFace.Ten));

            Assert.AreEqual(4, pointCalc.GetFlushPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetFlushPoints_Flush5()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Two),
                new Card(CardSuit.Diamond, CardFace.Four),
                new Card(CardSuit.Diamond, CardFace.Six),
                new Card(CardSuit.Diamond, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Diamond, CardFace.Ten));

            Assert.AreEqual(5, pointCalc.GetFlushPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetFlushPoints_NoFlush()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Two),
                new Card(CardSuit.Diamond, CardFace.Two),
                new Card(CardSuit.Heart, CardFace.Ace),
                new Card(CardSuit.Diamond, CardFace.Jack),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Diamond, CardFace.Ten));

            Assert.AreEqual(0, pointCalc.GetFlushPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetStraightPoints_Size3()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Two),
                new Card(CardSuit.Diamond, CardFace.Queen),
                new Card(CardSuit.Heart, CardFace.Jack),
                new Card(CardSuit.Diamond, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Diamond, CardFace.Ten));

            Assert.AreEqual(3, pointCalc.GetStraightPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetStraightPoints_Size4()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.King),
                new Card(CardSuit.Diamond, CardFace.Queen),
                new Card(CardSuit.Heart, CardFace.Jack),
                new Card(CardSuit.Diamond, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Diamond, CardFace.Ten));

            Assert.AreEqual(4, pointCalc.GetStraightPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetStraightPoints_Size5()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Nine),
                new Card(CardSuit.Diamond, CardFace.Queen),
                new Card(CardSuit.Heart, CardFace.Jack),
                new Card(CardSuit.Diamond, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Diamond, CardFace.Ten));

            Assert.AreEqual(5, pointCalc.GetStraightPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetStraightPoints_VerifyFixKingCountedAsStraight()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.King),
                new Card(CardSuit.Club, CardFace.Six),
                new Card(CardSuit.Spade, CardFace.Jack),
                new Card(CardSuit.Spade, CardFace.Six),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Spade, CardFace.Two));

            Assert.AreEqual(0, pointCalc.GetStraightPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetStraightPoints_TwoStraights()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Eight),
                new Card(CardSuit.Heart, CardFace.Nine),
                new Card(CardSuit.Spade, CardFace.Four),
                new Card(CardSuit.Club, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Heart, CardFace.Ten));

            Assert.AreEqual(6, pointCalc.GetStraightPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetStraightPoints_DoubleDoubleStraight()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Ten),
                new Card(CardSuit.Heart, CardFace.Ten),
                new Card(CardSuit.Spade, CardFace.Eight),
                new Card(CardSuit.Club, CardFace.Eight),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Heart, CardFace.Nine));

            Assert.AreEqual(12, pointCalc.GetStraightPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetStraightPoints_TripleStraights()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Eight),
                new Card(CardSuit.Heart, CardFace.Eight),
                new Card(CardSuit.Spade, CardFace.Eight),
                new Card(CardSuit.Club, CardFace.Nine),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Heart, CardFace.Ten));

            Assert.AreEqual(9, pointCalc.GetStraightPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetFifteenPoints_Simple15()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Three),
                new Card(CardSuit.Diamond, CardFace.Ten),
                new Card(CardSuit.Heart, CardFace.Eight),
                new Card(CardSuit.Diamond, CardFace.Nine),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Diamond, CardFace.Five));

            Assert.AreEqual(2, pointCalc.GetFifteenPoints());
        }

        [TestMethod]
        public void HandPointCalculator_GetFifteenPoints_Multiple15()
        {
            Hand hand = new Hand();

            hand.Cards = new List<Card>()
            {
                new Card(CardSuit.Diamond, CardFace.Four),
                new Card(CardSuit.Diamond, CardFace.Eight),
                new Card(CardSuit.Heart, CardFace.Four),
                new Card(CardSuit.Diamond, CardFace.Three),
            };

            HandPointCalculator pointCalc = new HandPointCalculator(hand, new Card(CardSuit.Club, CardFace.Three));

            Assert.AreEqual(8, pointCalc.GetFifteenPoints());
        }
    }
}
