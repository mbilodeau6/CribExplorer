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
        public void GameState_NoCardsPlayable_True()
        {
            GameState state = CreateTestGameState();

            state.PlayCount = 28;
            state.Players[0].Discard(new Card(CardSuit.Club, CardFace.Two));
            state.Players[0].Discard(new Card(CardSuit.Club, CardFace.Ace));

            state.Players[1].Discard(new Card(CardSuit.Spade, CardFace.Three));

            Assert.IsTrue(state.NoCardsPlayable());
        }

        [TestMethod]
        public void GameState_NoCardsPlayable_False()
        {
            GameState state = CreateTestGameState();

            state.PlayCount = 28;
            state.Players[0].Discard(new Card(CardSuit.Club, CardFace.Two));
            state.Players[1].Discard(new Card(CardSuit.Spade, CardFace.Three));

            Assert.IsFalse(state.NoCardsPlayable());
        }
    }
}
