using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CribExplorer.Model;
using CribExplorer;

namespace CribExplorerTests
{
    [TestClass]
    public class GameStateTests
    {
        private GameState CreateTestGameState()
        {
            GameState state = new GameState(new List<string>() { "A", "B" });

            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Club, CardFace.Ten));
            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Club, CardFace.Two));
            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Club, CardFace.Eight));
            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Club, CardFace.Ace));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Spade, CardFace.Ten));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Spade, CardFace.Queen));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Spade, CardFace.Three));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Spade, CardFace.Nine));

            return state;
        }

        [TestMethod]
        public void GameState_Constructor()
        {
            GameState state = new GameState(new List<string>() { "A", "B", "C" });

            Assert.AreEqual(3, state.Players.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GameState_Constructor_MissingPlayerNames()
        {
            GameState state = new GameState(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GameState_Constructor_TooFewPlayers()
        {
            GameState state = new GameState(new List<string>() {"A"});
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GameState_Constructor_TooManyPlayers()
        {
            GameState state = new GameState(new List<string>() { "A", "B", "C", "D", "E" });
        }

        [TestMethod]
        public void GameState_GetWinningPlayer_None()
        {
            GameState state = CreateTestGameState();

            state.Players[0].Score = 100;
            state.Players[1].Score = 90;

            Assert.AreEqual(-1, state.GetWinningPlayer());
        }

        [TestMethod]
        public void GameState_GetWinningPlayer()
        {
            GameState state = CreateTestGameState();

            state.Players[0].Score = 100;
            state.Players[1].Score = 122;

            Assert.AreEqual(1, state.GetWinningPlayer());
        }

        [TestMethod]
        public void GameState_AllCardsPlayed_True()
        {
            GameState state = CreateTestGameState();

            TestHelpers.DiscardCards(state, GameEngine.RequiredHandCardCount * state.Players.Count);

            Assert.IsTrue(state.AllCardsPlayed());
        }

        [TestMethod]
        public void GameState_AllCardsPlayed_False()
        {
            GameState state = CreateTestGameState();

            state.Players[0].Discard(new Card(CardSuit.Club, CardFace.Ten));
            state.Players[1].Discard(new Card(CardSuit.Spade, CardFace.Ten));

            Assert.IsFalse(state.AllCardsPlayed());
        }

        [TestMethod]
        public void GameState_CardsPlayable_AllPlayersFalse()
        {
            GameState state = CreateTestGameState();

            state.SumOfPlayedCards = 28;
            state.Players[0].Discard(new Card(CardSuit.Club, CardFace.Two));
            state.Players[0].Discard(new Card(CardSuit.Club, CardFace.Ace));

            state.Players[1].Discard(new Card(CardSuit.Spade, CardFace.Three));

            Assert.IsFalse(state.CardsPlayable());
        }

        [TestMethod]
        public void GameState_CardsPlayable_AllPlayersTrue()
        {
            GameState state = CreateTestGameState();

            state.SumOfPlayedCards = 28;
            state.Players[0].Discard(new Card(CardSuit.Club, CardFace.Two));
            state.Players[1].Discard(new Card(CardSuit.Spade, CardFace.Three));

            Assert.IsTrue(state.CardsPlayable());
        }

        [TestMethod]
        public void GameState_CardsPlayable_OnePlayerFalse()
        {
            GameState state = CreateTestGameState();

            state.SumOfPlayedCards = 28;
            state.Players[1].Discard(new Card(CardSuit.Spade, CardFace.Three));

            Assert.IsFalse(state.CardsPlayable(state.Players[1]));
        }

        [TestMethod]
        public void GameState_CardsPlayable_OnePlayerTrue()
        {
            GameState state = CreateTestGameState();

            state.SumOfPlayedCards = 28;
            state.Players[0].Discard(new Card(CardSuit.Club, CardFace.Two));
            state.Players[0].Discard(new Card(CardSuit.Club, CardFace.Ace));

            Assert.IsTrue(state.CardsPlayable(state.Players[1]));
        }

        [TestMethod]
        public void GameState_ResetForNewRound()
        {
            GameState state = new GameState(new List<string>() {"A", "B"});

            state.Crib.Cards.Add(null);
            state.Starter = new Card(CardSuit.Club, CardFace.Ace);
            state.SumOfPlayedCards = 10;
            state.AllScoresProvided = true;
            state.Players[0].Hand.Cards.Add(null);
            state.Players[0].Discards.Cards.Add(null);
            state.Players[1].Hand.Cards.Add(null);
            state.Players[1].Discards.Cards.Add(null);

            state.ResetForNewRound();

            Assert.AreEqual(0, state.Crib.Cards.Count, "Unexpected number of cards in crib");
            Assert.IsNull(state.Starter, "There should be no starter card");
            Assert.AreEqual(0, state.SumOfPlayedCards, "Unexpected SumOfPlayedCards");
            Assert.IsFalse(state.AllScoresProvided, "Unexpected value for AllHandScoresProvided");
            Assert.AreEqual(0, state.Players[0].Hand.Cards.Count, "Unexpected cards in player 0's hand");
            Assert.AreEqual(0, state.Players[0].Discards.Cards.Count, "Unexpected cards in player 0's discards");
            Assert.AreEqual(0, state.Players[1].Hand.Cards.Count, "Unexpected cards in player 1's hand");
            Assert.AreEqual(0, state.Players[1].Discards.Cards.Count, "Unexpected cards in player 1's discards");
        }
    }
}
