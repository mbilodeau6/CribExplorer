using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CribExplorer;
using CribExplorer.Model;

namespace CribExplorerTests
{
    [TestClass]
    public class GameEngineTests
    {
        private IList<string> testPlayerNames = new List<String>() {"PlayerA", "PlayerB"};

        [TestMethod]
        public void GameEngine_GetNextStage_Initial()
        {
            GameEngine game = new GameEngine(new GameState(testPlayerNames));

            Assert.AreEqual(GameEngine.GameStage.NewGame, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_NewRound_First()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0
            };

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.NewRound, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_NewRound_Continued()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.EndRound
            };

            AddTestCards(state);

            state.Players[0].Score = 119;
            state.Players[1].Score = 120;

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.NewRound, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_Reset()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.NewRound
            };

            state.Reset();

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.NewGame, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_CreateCrib()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.NewRound
            };

            AddTestCards(state);

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.CreateCrib, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_StartRound()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.CreateCrib
            };

            AddTestCards(state);

            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Three));

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.StartRound, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_NewSubRound_First()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.StartRound
            };

            AddTestCards(state);

            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Three));
            state.Starter = new Card(CardSuit.Heart, CardFace.Four);

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.NewSubRound, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_NewSubRound_Continue()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.EndSubRound
            };

            AddTestCards(state);

            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Three));
            state.Starter = new Card(CardSuit.Heart, CardFace.Four);

            DiscardCards(state, state.Players.Count * 4 - 1);

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.NewSubRound, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_EndSubRound()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.NewSubRound
            };

            AddTestCards(state);

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.EndSubRound, game.GetNextStage());
        }

        private void DiscardCards(GameState state, int discardCount)
        {
            int i = 0;
            foreach (Player player in state.Players)
            {
                IList<Card> cardsInHand = new List<Card>(player.Hand.Cards);

                foreach (Card card in cardsInHand)
                {
                    if (i >= discardCount)
                        return;

                    player.Discard(card);
                    i++;
                }
            }

            if (i < discardCount)
                throw new ArgumentException(string.Format("Unable to discard {0} cards because only {1} cards exist.", discardCount, i));
        }

        [TestMethod]
        public void GameEngine_GetNextStage_EndRound()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.EndSubRound
            };

            AddTestCards(state);

            // TODO: Get rid of magic number 4 (count of cards in hand)
            DiscardCards(state, state.Players.Count * 4);

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.EndRound, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_EndGame()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.EndRound
            };

            AddTestCards(state);

            state.Players[0].Score = 119;
            state.Players[1].Score = 121;

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.EndGame, game.GetNextStage());
        }

        private void AddTestCards(GameState state)
        {
            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Ace));
            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Two));
            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Three));
            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Four));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Five));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Six));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Seven));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Eight));
        }
    }
}
