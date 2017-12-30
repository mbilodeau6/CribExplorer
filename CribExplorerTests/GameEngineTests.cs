using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CribExplorer.Model;
using CribExplorer;
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

        [TestMethod]
        public void GameEngine_CurrentPlayer_HandleDraw()
        {
            Mock<IDeck> mockDeck = new Mock<IDeck>();

            mockDeck.SetupSequence(x => x.GetNextCard())
                .Returns(new Card(CardSuit.Heart, CardFace.Ten))
                .Returns(new Card(CardSuit.Diamond, CardFace.Ten))
                .Returns(new Card(CardSuit.Heart, CardFace.Eight))
                .Returns(new Card(CardSuit.Diamond, CardFace.Eight))
                .Returns(new Card(CardSuit.Heart, CardFace.Ace))
                .Returns(new Card(CardSuit.Diamond, CardFace.Nine));

            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            IList<int> currentPlayers = gameEngine.GetCurrentPlayers();
            Assert.AreEqual(1, currentPlayers.Count, "Unexpected current player count");
            Assert.AreEqual(0, currentPlayers[0], "Unexpected current player");

            mockDeck.Verify(x => x.GetNextCard(), Times.Exactly(19));
        }

        [TestMethod]
        public void GameEngine_GetCurrentPlayer_Initial()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();
            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            IList<int> currentPlayers = gameEngine.GetCurrentPlayers();

            Assert.AreEqual(1, currentPlayers.Count, "Unexpected number of players with active actions");
            Assert.AreEqual(1, currentPlayers[0], "Unexpected current player");
        }

        [TestMethod]
        public void GameEngine_GetDealer_Initial()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();
            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            Assert.AreEqual(1, gameEngine.GetDealer());
        }

        [TestMethod]
        public void GameEngine_GetPlayerHand_Initial()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();
            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            Hand playerHand = gameEngine.GetPlayerHand(0);

            Assert.AreEqual(6, playerHand.Cards.Count, "Unexpected card count for Player 0's hand");
        }

        [TestMethod]
        public void GameEngine_GetGetCrib_Initial()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();
            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            Assert.AreEqual(0, gameEngine.GetCrib().Cards.Count, "Unexpected cards found in the crib");
        }

        [TestMethod]
        public void GameEngine_GetStarterCard_Initial()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();
            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            Assert.AreEqual(new Card(CardSuit.Diamond, CardFace.Six), gameEngine.GetStarterCard());
        }

        [TestMethod]
        public void GameEngine_GetPlayerDiscards_Initial()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();
            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            Assert.AreEqual(0, gameEngine.GetPlayerDiscards(0).Cards.Count, "Unexpected cards found in Player 0's initial discard pile");
        }

        [TestMethod]
        public void GameEngine_GetCurrentAction_Initial()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();
            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            Assert.AreEqual(PlayerAction2.Deal, gameEngine.GetCurrentAction());
        }

        [TestMethod]
        public void GameEngine_GetCurrentAction_Deal_NewGame()
        {
            // Set up state so that PlayerB is dealer and game at stage
            // where dealer needs to score the crib.
            GameState startingState = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.ScoreCrib,
                Dealer = 1,
                CurrentPlayers = new List<int>() { 1 }
            };

            Mock<IDeck> mockDeck = new Mock<IDeck>();

            GameEngine gameEngine = new GameEngine(startingState);

            // Verify that when PlayerB provides crib score a new round 
            // is started with PlayerA as the dealer.
            gameEngine.ProvideScoreForCrib(10);

            Assert.AreEqual(PlayerAction2.Deal, gameEngine.GetCurrentAction(), "Unexpected action");

            IList<int> currentPlayers = gameEngine.GetCurrentPlayers();
            Assert.AreEqual(1, currentPlayers.Count, "Unexpected current player count");
            Assert.AreEqual(0, currentPlayers[0], "Unexpected current player");
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GameEngine_PlayCard_InvalidPlayerIndexLow()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();

            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            gameEngine.PlayCard(-1, new Card(CardSuit.Heart, CardFace.Ten));
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GameEngine_PlayCard_InvalidPlayerIndexHigh()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();

            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            gameEngine.PlayCard(2, new Card(CardSuit.Heart, CardFace.Ten));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GameEngine_PlayCard_WrongCard()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();

            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            gameEngine.PlayCard(1, new Card(CardSuit.Heart, CardFace.Ten));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GameEngine_PlayCard_NotPlayersTurn()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();

            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            gameEngine.PlayCard(0, new Card(CardSuit.Heart, CardFace.Eight));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GameEngine_PlayCard_NoValidCardsToPlay()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();

            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            // Valid plays
            gameEngine.PlayCard(1, new Card(CardSuit.Diamond, CardFace.Nine));
            gameEngine.PlayCard(0, new Card(CardSuit.Heart, CardFace.Jack));
            gameEngine.PlayCard(1, new Card(CardSuit.Diamond, CardFace.Seven));
            gameEngine.PlayCard(0, new Card(CardSuit.Heart, CardFace.Four));

            // Invalid because this would exceed 31
            gameEngine.PlayCard(1, new Card(CardSuit.Diamond, CardFace.Four));
        }

        [TestMethod]
        public void GameEngine_PlayCard()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();

            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            gameEngine.PlayCard(1, new Card(CardSuit.Diamond, CardFace.Nine));
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GameEngine_PlayerPass_InvalidPlayerIndexLow()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();

            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            gameEngine.PlayerPass(-1);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GameEngine_PlayerPass_InvalidPlayerIndexHigh()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();

            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            gameEngine.PlayerPass(2);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void GameEngine_PlayerPass_NotAllowed()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();

            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            gameEngine.PlayerPass(1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GameEngine_PlayerPass_NotPlayersTurn()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 1,
            };

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.PlayerPass(0);
        }

        [TestMethod]
        public void GameEngine_PlayerPass()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 1,
                SumOfPlayedCards = 30
            };

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.PlayerPass(1);
        }

        [TestMethod]
        public void GameEngine_GetNextStage_Initial()
        {
            GameEngine gameEngine = new GameEngine(new GameState(testPlayerNames));

            Assert.AreEqual(PlayerAction2.NewGame, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_NewRound_First()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0
            };

            GameEngine gameEngine = new GameEngine(state);

            Assert.AreEqual(PlayerAction2.NewRound, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_NewRound_Continued()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = PlayerAction2.EndRound
            };

            AddTestCards(state);

            state.Players[0].Score = 119;
            state.Players[1].Score = 120;

            GameEngine gameEngine = new GameEngine(state);

            Assert.AreEqual(PlayerAction2.NewRound, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_Reset()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = PlayerAction2.NewRound
            };

            state.Reset();

            GameEngine gameEngine = new GameEngine(state);

            Assert.AreEqual(PlayerAction2.NewGame, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_CreateCrib_First()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = PlayerAction2.NewRound
            };

            AddTestCards(state);

            GameEngine gameEngine = new GameEngine(state);

            Assert.AreEqual(PlayerAction2.CreateCrib, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_CreateCrib_Continue()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = PlayerAction2.CreateCrib
            };

            AddTestCards(state);

            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Three));
            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Four));
            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Five));

            GameEngine gameEngine = new GameEngine(state);

            Assert.AreEqual(PlayerAction2.CreateCrib, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_StartRound()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = PlayerAction2.CreateCrib
            };

            AddTestCards(state);

            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Three));
            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Four));
            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Five));
            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Six));

            GameEngine gameEngine = new GameEngine(state);

            Assert.AreEqual(PlayerAction2.StartRound, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_NewPlay_MoreCards()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = PlayerAction2.NewPlay
            };

            AddTestCards(state);

            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Three));
            state.Starter = new Card(CardSuit.Heart, CardFace.Four);

            GameEngine gameEngine = new GameEngine(state);

            TestHelpers.DiscardCards(state, gameEngine.GetMaxTotalHandCount() - 1);

            Assert.AreEqual(PlayerAction2.NewPlay, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_NewPlay_NotYet31()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = PlayerAction2.NewPlay
            };

            AddTestCards(state);

            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Three));
            state.Starter = new Card(CardSuit.Heart, CardFace.Four);

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.PlayCard(0, new Card(CardSuit.Heart, CardFace.King));
            gameEngine.PlayCard(1, new Card(CardSuit.Heart, CardFace.Queen));
            gameEngine.PlayCard(0, new Card(CardSuit.Heart, CardFace.Nine));

            Assert.AreEqual(PlayerAction2.NewPlay, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_EndPlay_First31()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = PlayerAction2.NewPlay
            };

            AddTestCards(state);

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.PlayCard(0, new Card(CardSuit.Heart, CardFace.King));
            gameEngine.PlayCard(1, new Card(CardSuit.Heart, CardFace.Queen));
            gameEngine.PlayCard(0, new Card(CardSuit.Heart, CardFace.Ace));
            gameEngine.PlayCard(1, new Card(CardSuit.Heart, CardFace.Jack));

            Assert.AreEqual(PlayerAction2.EndPlay, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_EndPlay_OutOfCards()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = PlayerAction2.NewPlay
            };

            AddTestCards(state);

            state.Crib.Add(new Card(CardSuit.Heart, CardFace.Three));
            state.Starter = new Card(CardSuit.Heart, CardFace.Four);

            GameEngine gameEngine = new GameEngine(state);

            TestHelpers.DiscardCards(state, gameEngine.GetMaxTotalHandCount());

            Assert.AreEqual(PlayerAction2.EndPlay, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_EndPlay_31NotPossible()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = PlayerAction2.NewPlay
            };

            AddTestCards(state);

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.PlayCard(0, new Card(CardSuit.Heart, CardFace.Nine));
            gameEngine.PlayCard(1, new Card(CardSuit.Heart, CardFace.Queen));
            gameEngine.PlayCard(0, new Card(CardSuit.Heart, CardFace.Ace));
            gameEngine.PlayCard(1, new Card(CardSuit.Heart, CardFace.Jack));

            Assert.AreEqual(PlayerAction2.EndPlay, gameEngine.GetNextStage());
        }


        [TestMethod]
        public void GameEngine_GetNextStage_ScoreHands_First()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = PlayerAction2.EndPlay,
                Dealer = 0
            };

            AddTestCards(state);

            GameEngine gameEngine = new GameEngine(state);

            TestHelpers.DiscardCards(state, gameEngine.GetMaxTotalHandCount());

            Assert.AreEqual(PlayerAction2.ScoreHands, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_ScoreHands_Continue()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 1,
                Stage = PlayerAction2.ScoreHands,
                Dealer = 0
            };

            AddTestCards(state);

            GameEngine gameEngine = new GameEngine(state);

            TestHelpers.DiscardCards(state, gameEngine.GetMaxTotalHandCount());

            Assert.AreEqual(PlayerAction2.ScoreHands, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_ScoreCrib()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 1,
                Stage = PlayerAction2.ScoreHands,
                Dealer = 1,
                AllHandScoresProvided = true
            };

            AddTestCards(state);

            GameEngine gameEngine = new GameEngine(state);

            TestHelpers.DiscardCards(state, gameEngine.GetMaxTotalHandCount());

            Assert.AreEqual(PlayerAction2.ScoreCrib, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_EndRound()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 1,
                Stage = PlayerAction2.ScoreCrib,
                Dealer = 1
            };

            AddTestCards(state);

            GameEngine gameEngine = new GameEngine(state);

            TestHelpers.DiscardCards(state, gameEngine.GetMaxTotalHandCount());

            Assert.AreEqual(PlayerAction2.EndRound, gameEngine.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_EndGame()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 0,
                Stage = PlayerAction2.EndRound
            };

            AddTestCards(state);

            state.Players[0].Score = 119;
            state.Players[1].Score = 121;

            GameEngine gameEngine = new GameEngine(state);

            Assert.AreEqual(PlayerAction2.EndGame, gameEngine.GetNextStage());
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
            GameEngine gameEngine = new GameEngine(state);

            Assert.AreEqual(6, gameEngine.GetCardCountToDeal());
        }
    }
}
