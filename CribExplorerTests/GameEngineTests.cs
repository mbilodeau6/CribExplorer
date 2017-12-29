using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CribExplorer;
using CribExplorer.Model;
using Moq;

namespace CribExplorerTests
{
    [TestClass]
    public class GameEngineTests
    {
        private IList<string> testPlayerNames = new List<String>() {"PlayerA", "PlayerB"};

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
        public void GameEngine_Constructor_Deck()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();
            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            Assert.IsNotNull(gameEngine, "GameEngine object not created");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GameEngine_Constructor_MissingDeck()
        {
            GameEngine gameEngine = new GameEngine(null, testPlayerNames);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GameEngine_Constructor_MissingPlayers()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();
            GameEngine gameEngine = new GameEngine(mockDeck.Object, null);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GameEngine_Constructor_TooManyPlayers()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();
            IList<string> wrongNumberOfPlayerNames = new List<string>() { "PlayerA", "PlayerB", "Playerc" };
            GameEngine gameEngine = new GameEngine(mockDeck.Object, wrongNumberOfPlayerNames);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GameEngine_Constructor_TooFewPlayers()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();
            IList<string> wrongNumberOfPlayerNames = new List<string>() { "PlayerA" };
            GameEngine gameEngine = new GameEngine(mockDeck.Object, wrongNumberOfPlayerNames);
        }

        [TestMethod]
        public void GameEngine_Constructor_State()
        {
            GameState state = new GameState(testPlayerNames)
            {
                Dealer = 1
            };

            GameEngine gameEngine = new GameEngine(state);
            
            Assert.IsNotNull(gameEngine, "GameEngine not created");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GameEngine_Constructor_MissingState()
        {
            GameEngine gameEngine = new GameEngine(null);
        }

        // TODO: Add tests for the following.
        //   GetCurrentAction()
        //   GetCurrentPlayer()
        //   GetDealer()
        //   GetPlayerHand(int playerId)
        //   GetCrib()
        //   GetStarterCard()
        //   GetPlayerDiscards(int playerId)

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
        public void GameEngine_GetNextStage_CreateCrib_First()
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
        public void GameEngine_GetNextStage_CreateCrib_Continue()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.CreateCrib
            };

            AddTestCards(state);

            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Three));
            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Four));
            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Five));

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
            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Four));
            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Five));
            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Six));

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.StartRound, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_NewPlay_MoreCards()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.NewPlay
            };

            AddTestCards(state);

            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Three));
            state.Starter = new Card(CardSuit.Heart, CardFace.Four);

            GameEngine game = new GameEngine(state);

            TestHelpers.DiscardCards(state, game.GetMaxTotalHandCount() - 1);

            Assert.AreEqual(GameEngine.GameStage.NewPlay, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_NewPlay_NotYet31()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.NewPlay
            };

            AddTestCards(state);

            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Three));
            state.Starter = new Card(CardSuit.Heart, CardFace.Four);

            GameEngine game = new GameEngine(state);

            game.PlayCard(0, new Card(CardSuit.Heart, CardFace.King));
            game.PlayCard(1, new Card(CardSuit.Heart, CardFace.Queen));
            game.PlayCard(1, new Card(CardSuit.Heart, CardFace.Jack));

            Assert.AreEqual(GameEngine.GameStage.NewPlay, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_EndPlay_First31()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.NewPlay
            };

            AddTestCards(state);

            GameEngine game = new GameEngine(state);

            game.PlayCard(0, new Card(CardSuit.Heart, CardFace.King));
            game.PlayCard(0, new Card(CardSuit.Heart, CardFace.Ace));
            game.PlayCard(1, new Card(CardSuit.Heart, CardFace.Queen));
            game.PlayCard(1, new Card(CardSuit.Heart, CardFace.Jack));

            Assert.AreEqual(GameEngine.GameStage.EndPlay, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_EndPlay_OutOfCards()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.NewPlay
            };

            AddTestCards(state);

            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Three));
            state.Starter = new Card(CardSuit.Heart, CardFace.Four);

            GameEngine game = new GameEngine(state);

            TestHelpers.DiscardCards(state, game.GetMaxTotalHandCount());

            Assert.AreEqual(GameEngine.GameStage.EndPlay, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_EndPlay_31NotPossible()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.NewPlay
            };

            AddTestCards(state);

            GameEngine game = new GameEngine(state);

            game.PlayCard(0, new Card(CardSuit.Heart, CardFace.Nine));
            game.PlayCard(0, new Card(CardSuit.Heart, CardFace.Ace));
            game.PlayCard(1, new Card(CardSuit.Heart, CardFace.Queen));
            game.PlayCard(1, new Card(CardSuit.Heart, CardFace.Jack));

            Assert.AreEqual(GameEngine.GameStage.EndPlay, game.GetNextStage());
        }


        [TestMethod]
        public void GameEngine_GetNextStage_ScoreHands_First()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = GameEngine.GameStage.EndPlay,
                Dealer = 0
            };

            AddTestCards(state);

            GameEngine game = new GameEngine(state);

            TestHelpers.DiscardCards(state, game.GetMaxTotalHandCount());

            Assert.AreEqual(GameEngine.GameStage.ScoreHands, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_ScoreHands_Continue()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 1,
                Stage = GameEngine.GameStage.ScoreHands,
                Dealer = 0
            };

            AddTestCards(state);

            GameEngine game = new GameEngine(state);

            TestHelpers.DiscardCards(state, game.GetMaxTotalHandCount());

            Assert.AreEqual(GameEngine.GameStage.ScoreHands, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_ScoreCrib()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 1,
                Stage = GameEngine.GameStage.ScoreHands,
                Dealer = 1,
                AllHandScoresProvided = true
            };

            AddTestCards(state);

            GameEngine game = new GameEngine(state);

            TestHelpers.DiscardCards(state, game.GetMaxTotalHandCount());

            Assert.AreEqual(GameEngine.GameStage.ScoreCrib, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_EndRound()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 1,
                Stage = GameEngine.GameStage.ScoreCrib,
                Dealer = 1
            };

            AddTestCards(state);

            GameEngine game = new GameEngine(state);

            TestHelpers.DiscardCards(state, game.GetMaxTotalHandCount());

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
            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Five));
            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Nine));
            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.King));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Two));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Six));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Queen));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Jack));
        }

        [TestMethod]
        public void GameEngine_GetCountToDeal()
        {
            GameState state = new GameState(testPlayerNames);
            GameEngine game = new GameEngine(state);

            Assert.AreEqual(6, game.GetCardCountToDeal());
        }
    }
}
