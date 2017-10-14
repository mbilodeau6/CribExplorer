using System;
using CribExplorer.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CribExplorerTests
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Game_Constructor_MissingDeck()
        {
            Game game = new Game(null, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Game_Constructor_PlayerCountTooLow()
        {
            IDeck deck = new Deck();
            Game game = new Game(deck, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Game_Constructor_PlayerCountTooHigh()
        {
            IDeck deck = new Deck();
            Game game = new Game(deck, 1);
        }

        [TestMethod]
        public void Game_Constructor()
        {
            IDeck deck = new Deck();
            Game game = new Game(deck, 2);

            Assert.AreEqual(2, game.Players.Count);
        }

    }
}
