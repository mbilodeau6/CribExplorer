using System;
using System.Collections.Generic;
using CribExplorer.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CribExplorerTests
{
    [TestClass]
    public class GameTests
    {
        private IList<string> testOnePlayer = new List<string>() { "PlayerA" };
        private IList<string> testTwoPlayers = new List<string>() { "PlayerA", "PlayerB" };

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Game_Constructor_MissingDeck()
        {
            Game game = new Game(null, testTwoPlayers);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Game_Constructor_PlayerCountTooLow()
        {
            IDeck deck = new Deck();
            Game game = new Game(deck, testOnePlayer);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Game_Constructor_PlayerCountTooHigh()
        {
            IDeck deck = new Deck();
            Game game = new Game(deck, testOnePlayer);
        }

        [TestMethod]
        public void Game_Constructor()
        {
            IDeck deck = new Deck();
            Game game = new Game(deck, testTwoPlayers);

            Assert.AreEqual(2, game.Players.Count);
        }

        [TestMethod]
        public void Game_PlayerTurn_Initial()
        {
            Mock<IDeck> mockDeck = new Mock<IDeck>();

            mockDeck.SetupSequence(x => x.GetNextCard())
                .Returns(new Card(CardSuit.Heart, CardFace.Ten))
                .Returns(new Card(CardSuit.Diamond, CardFace.Ace));

            Game game = new Game(mockDeck.Object, testTwoPlayers);

            Assert.AreEqual(1, game.PlayerTurn);
        }

        [TestMethod]
        public void Game_PlayerTurn_HandleDraw()
        {
            Mock<IDeck> mockDeck = new Mock<IDeck>();

            mockDeck.SetupSequence(x => x.GetNextCard())
                .Returns(new Card(CardSuit.Heart, CardFace.Ten))
                .Returns(new Card(CardSuit.Diamond, CardFace.Ten))
                .Returns(new Card(CardSuit.Heart, CardFace.Eight))
                .Returns(new Card(CardSuit.Diamond, CardFace.Eight))
                .Returns(new Card(CardSuit.Heart, CardFace.Ace))
                .Returns(new Card(CardSuit.Diamond, CardFace.Nine));

            Game game = new Game(mockDeck.Object, testTwoPlayers);

            Assert.AreEqual(0, game.PlayerTurn);

            mockDeck.Verify(x => x.GetNextCard(), Times.Exactly(14));
        }


    }
}
