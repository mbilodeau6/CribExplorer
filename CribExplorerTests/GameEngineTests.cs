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

            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Ace));
            state.Players[0].Score = 119;
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Two));
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

            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Ace));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Two));

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

            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Ace));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Two));

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

            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Ace));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Two));

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

            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Ace));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Two));

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

            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Ace));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Two));

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.EndSubRound, game.GetNextStage());
        }

        private void DiscardCards(GameState state, int discardCount)
        {
            int i = 0;
            foreach (CardFace nextCardFace in Enum.GetValues(typeof(CardFace)))
            {
                if (i >= discardCount)
                    break;

                state.Discards.Add(new Card(CardSuit.Diamond, nextCardFace));
                i++;
            }
        }

        [TestMethod]
        public void GameEngine_GetNextStage_EndRound()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.EndSubRound
            };

            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Ace));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Two));

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

            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Ace));
            state.Players[0].Score = 119;
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Two));
            state.Players[1].Score = 121;

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.EndGame, game.GetNextStage());
        }
    }
}
