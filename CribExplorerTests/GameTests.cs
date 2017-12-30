using System;
using System.Collections.Generic;
using CribExplorer.Model;
using CribExplorer;
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

        private Mock<IDeck> CreateMockDeck()
        {
            Mock<IDeck> mockDeck = new Mock<IDeck>();

            mockDeck.SetupSequence(x => x.GetNextCard())
                .Returns(new Card(CardSuit.Heart, CardFace.Ten))
                .Returns(new Card(CardSuit.Diamond, CardFace.Two))
                .Returns(new Card(CardSuit.Heart, CardFace.Eight))
                .Returns(new Card(CardSuit.Diamond, CardFace.Eight))
                .Returns(new Card(CardSuit.Heart, CardFace.Ace))
                .Returns(new Card(CardSuit.Diamond, CardFace.Nine))
                .Returns(new Card(CardSuit.Heart, CardFace.Jack))
                .Returns(new Card(CardSuit.Diamond, CardFace.Five))
                .Returns(new Card(CardSuit.Heart, CardFace.Three))
                .Returns(new Card(CardSuit.Diamond, CardFace.Four))
                .Returns(new Card(CardSuit.Heart, CardFace.Four))
                .Returns(new Card(CardSuit.Diamond, CardFace.Seven))
                .Returns(new Card(CardSuit.Heart, CardFace.Seven))
                .Returns(new Card(CardSuit.Diamond, CardFace.Queen))
                .Returns(new Card(CardSuit.Diamond, CardFace.Six));

            return mockDeck;
        }

        [TestMethod]
        public void Game_GetNextAction_PlayerNeedsToPlayCard()
        {
            // Set up state where current player has a card that is playable.
            GameState startingState = new GameState(testTwoPlayers)
            {
                Stage = PlayerAction2.NewPlay,
                PlayerTurn = 1,
                SumOfPlayedCards = 30
            };

            startingState.Players[1].Hand.Cards.Add(new Card(CardSuit.Diamond, CardFace.Two));
            startingState.Players[1].Hand.Cards.Add(new Card(CardSuit.Diamond, CardFace.Ace));

            Mock<IDeck> mockDeck = new Mock<IDeck>();

            Game game = new Game(mockDeck.Object, testTwoPlayers, startingState);

            PlayerAction action = game.GetNextAction();

            Assert.AreEqual(PlayerAction.ActionType.PlayCard, action.Action, "Unexpected action");
            Assert.AreEqual(1, action.Players.Count, "Unexpected player count");
            Assert.AreEqual("PlayerB", action.Players[0], "Unexpected player name");
        }

        [TestMethod]
        public void Game_GetNextAction_PlayerNeedsToPass()
        {
            // Set up state where current player doesn't have a card that is playable.
            GameState startingState = new GameState(testTwoPlayers)
            {
                Stage = PlayerAction2.NewPlay,
                PlayerTurn = 1,
                SumOfPlayedCards = 30
            };

            // Player 0 has a playable card but it is player 1's turn.
            startingState.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Ace));
            startingState.Players[1].Hand.Cards.Add(new Card(CardSuit.Diamond, CardFace.Two));

            Mock<IDeck> mockDeck = new Mock<IDeck>();

            Game game = new Game(mockDeck.Object, testTwoPlayers, startingState);

            PlayerAction action = game.GetNextAction();

            Assert.AreEqual(PlayerAction.ActionType.PlayerMustPass, action.Action, "Unexpected action");
            Assert.AreEqual(1, action.Players.Count, "Unexpected player count");
            Assert.AreEqual("PlayerB", action.Players[0], "Unexpected player name");
        }

        [TestMethod]
        public void Game_GetNextAction_NonDealerNeedsToCountHand()
        {
            // Set up state where all cards are played and therefore the round is over.
            GameState startingState = new GameState(testTwoPlayers)
            {
                Stage = PlayerAction2.EndPlay,
                Dealer = 1,
                PlayerTurn = 0,
            };

            for (int i=0; i < GameEngine.RequiredHandCardCount; i++)
            {
                startingState.Players[0].Discards.Cards.Add(null);
                startingState.Players[1].Discards.Cards.Add(null);
            }

            Mock<IDeck> mockDeck = new Mock<IDeck>();

            Game game = new Game(mockDeck.Object, testTwoPlayers, startingState);

            // Verify the system knows it is time to calculate scores.
            PlayerAction action = game.GetNextAction();

            Assert.AreEqual(PlayerAction.ActionType.CalculateScore, action.Action, "Unexpected action");
            Assert.AreEqual(1, action.Players.Count, "Unexpected player count");
            Assert.AreEqual("PlayerA", action.Players[0], "Unexpected player name");
        }

        [TestMethod]
        public void Game_GetNextAction_DealerNeedsToCountHand()
        {
            // Set up state where round is done and non-dealer (PlayerA) has provided 
            // their score.
            GameState startingState = new GameState(testTwoPlayers)
            {
                Stage = PlayerAction2.ScoreHands,
                Dealer = 1,
                PlayerTurn = 0
            };

            Mock<IDeck> mockDeck = new Mock<IDeck>();

            Game game = new Game(mockDeck.Object, testTwoPlayers, startingState);

            game.IsProvidedScoreCorrectForHand(0, 10);

            // Verify the system knows the dealer (PlayerB) still has to provide 
            // a score.
            PlayerAction action = game.GetNextAction();

            Assert.AreEqual(PlayerAction.ActionType.CalculateScore, action.Action, "Unexpected action");
            Assert.AreEqual(1, action.Players.Count, "Unexpected player count");
            Assert.AreEqual("PlayerB", action.Players[0], "Unexpected player name");
        }

        [TestMethod]
        public void Game_GetNextAction_DealerNeedsToCountCrib()
        {
            // Set up state so that PlayerB is dealer and game at stage
            // where dealer needs to score their hand.
            GameState startingState = new GameState(testTwoPlayers)
            {
                Stage = PlayerAction2.ScoreHands,
                Dealer = 1,
                PlayerTurn = 0,
                AllHandScoresProvided = false
            };

            Mock<IDeck> mockDeck = new Mock<IDeck>();

            Game game = new Game(mockDeck.Object, testTwoPlayers, startingState);

            game.IsProvidedScoreCorrectForHand(0, 10);
            PlayerAction action = game.GetNextAction();

            // Verify that when the dealer (PlayerB) scores their hand the
            // game moves to the stage where the crib is calculated.
            game.IsProvidedScoreCorrectForHand(1, 10);

            action = game.GetNextAction();

            Assert.AreEqual(PlayerAction.ActionType.CalculateCribScore, action.Action, "Unexpected action");
            Assert.AreEqual(1, action.Players.Count, "Unexpected player count");
            Assert.AreEqual("PlayerB", action.Players[0], "Unexpected player name");
        }
    }
}
