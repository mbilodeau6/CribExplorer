using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CribExplorer.Model;

namespace CribExplorerTests
{
    [TestClass]
    public class PlayerTests
    {
        private Player CreateTestPlayer()
        {
            string playerName = "Joe";
            Player player = new Player(playerName);

            player.Hand.Cards.Add(new Card(CardSuit.Club, CardFace.Ace));
            player.Hand.Cards.Add(new Card(CardSuit.Diamond, CardFace.King));
            player.Hand.Cards.Add(new Card(CardSuit.Spade, CardFace.Jack));

            return player;
        }

        [TestMethod]
        public void Player_Constructor()
        {
            string playerName = "Joe";
            Player player = new Player(playerName);

            Assert.AreEqual(playerName, player.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Player_Constructor_MissingName()
        {
            Player player = new Player(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Player_Constructor_EmptyName()
        {
            Player player = new Player(string.Empty);
        }

        [TestMethod]
        public void Player_Discard()
        {
            Player player = CreateTestPlayer();
            Card selectedCard = player.Hand.Cards[1];

            player.Discard(selectedCard);

            Assert.AreEqual(2, player.Hand.Cards.Count, "Unexpected number of cards in hand.");
            Assert.AreEqual(1, player.Discards.Cards.Count, "Unexpected number of cards in discard pile.");
            Assert.AreEqual(selectedCard, player.Discards.Cards[0], "Unexpected card found in discard pile.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Player_Discard_InvalidCard()
        {
            Player player = CreateTestPlayer();
            player.Discard(new Card(CardSuit.Spade, CardFace.Five));
        }
    }
}
