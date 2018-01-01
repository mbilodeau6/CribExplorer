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
        // TODO: Should handle up to 4 players
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

            // TODO: Should really be PlayerAction2.Deal once refactoring done
            Assert.AreEqual(PlayerAction2.NewGame, gameEngine.GetCurrentAction());
        }

        [TestMethod]
        public void GameEngine_GetCurrentAction_Deal_NewRoundAsNoWinnerYet()
        {
            // Set up state so that PlayerB is dealer and game at stage
            // where dealer needs to score the crib.
            GameState startingState = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.ScoreCrib,
                Dealer = 1,
                CurrentPlayers = new List<int>() { 1 }
            };

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
        public void GameEngine_GetCurrentAction_CreateCrib_AllPlayers()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();

            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            gameEngine.DealCards();

            PlayerAction2 action = gameEngine.GetCurrentAction();

            Assert.AreEqual(PlayerAction2.CreateCrib, action, "Unexpected action");
            Assert.AreEqual(2, gameEngine.GetCurrentPlayers().Count, "Unexpected player count");
        }

        [TestMethod]
        public void GameEngine_GetCurrentAction_CreateCrib_OnePlayerLeft()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();

            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            gameEngine.DealCards();

            gameEngine.AddToCrib(0, new Card(CardSuit.Heart, CardFace.Three));
            gameEngine.AddToCrib(1, new Card(CardSuit.Diamond, CardFace.Eight));
            gameEngine.AddToCrib(1, new Card(CardSuit.Diamond, CardFace.Queen));

            PlayerAction2 action = gameEngine.GetCurrentAction();

            Assert.AreEqual(PlayerAction2.CreateCrib, action, "Unexpected action");
            Assert.AreEqual(1, gameEngine.GetCurrentPlayers().Count, "Unexpected player count");
        }

        [TestMethod]
        public void GameEngine_GetCurrentAction_PlayOrPass_Initial()
        {
            // Set up state where current all crib cards have been provided
            // and therefore the game state should transition to PlayOrPass
            GameState startingState = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.CreateCrib,
                Dealer = 1
            };

            for(int i=0; i < GameEngine.RequiredHandCardCount; i++)
            {
                startingState.Players[0].Hand.Cards.Add(null);
                startingState.Players[1].Hand.Cards.Add(null);
                startingState.Crib.Add(null);
            }

            GameEngine gameEngine = new GameEngine(startingState);

            Assert.AreEqual(PlayerAction2.PlayOrPass, gameEngine.GetCurrentAction(), "Unexpected action");

            IList<int> currentPlayers = gameEngine.GetCurrentPlayers();

            Assert.AreEqual(1, currentPlayers.Count, "Unexpected player count");
            Assert.AreEqual(1, currentPlayers[0], "Unexpected current player");
        }

        [TestMethod]
        public void GameEngine_GetCurrentAction_PlayOrPass_ResetCountAsReachedFirst31()
        {
            GameState state = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.PlayOrPass,
                SumOfPlayedCards = 22
            };

            state.CurrentPlayers.Add(0);

            Card testCard = new Card(CardSuit.Heart, CardFace.Nine);
            state.Players[0].Hand.Cards.Add(testCard);
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Diamond, CardFace.Five));

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.PlayCard(0, testCard);

            Assert.AreEqual(31, state.SumOfPlayedCards, "Playing H9 should have made card count equal 31");
            Assert.AreEqual(PlayerAction2.PlayOrPass, gameEngine.GetCurrentAction(), "Should have remained in the PlayOrPassStage");
            Assert.AreEqual(0, state.SumOfPlayedCards, "SumOfPlayedCards has not been reset");
        }

        [TestMethod]
        public void GameEngine_GetCurrentAction_PlayOrPass_ResetCountAs31NotPossible()
        {
            GameState state = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.PlayOrPass,
                SumOfPlayedCards = 29
            };

            state.CurrentPlayers.Add(0);
            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Heart, CardFace.Nine));
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Diamond, CardFace.Five));

            GameEngine gameEngine = new GameEngine(state);

            Assert.AreEqual(PlayerAction2.PlayOrPass, gameEngine.GetCurrentAction(), "Should have remained in the PlayOrPassStage");
            Assert.AreEqual(0, state.SumOfPlayedCards, "SumOfPlayedCards has not been reset");
        }

        [TestMethod]
        public void GameEngine_GetCurrentAction_PlayOrPass_NotYet31()
        {
            GameState state = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.PlayOrPass,
                SumOfPlayedCards = 19
            };

            state.CurrentPlayers.Add(0);
            Card testCard = new Card(CardSuit.Heart, CardFace.Nine);
            state.Players[0].Hand.Cards.Add(testCard);
            state.Players[1].Hand.Cards.Add(new Card(CardSuit.Diamond, CardFace.Five));

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.PlayCard(0, testCard);

            Assert.AreEqual(28, state.SumOfPlayedCards, "Unexpected card count");
            Assert.AreEqual(PlayerAction2.PlayOrPass, gameEngine.GetCurrentAction(), "Should have remained in the PlayOrPassStage");
        }

        [TestMethod]
        public void GameEngine_GetCurrentAction_ScoreHand_NonDealer()
        {
            // Set up state where all cards are played and therefore the round is over.
            GameState startingState = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.PlayOrPass,
                Dealer = 1
            };

            for (int i = 0; i < GameEngine.RequiredHandCardCount; i++)
            {
                startingState.Players[0].Discards.Cards.Add(null);
                startingState.Players[1].Discards.Cards.Add(null);
            }

            GameEngine gameEngine = new GameEngine(startingState);

            Assert.AreEqual(PlayerAction2.ScoreHands, gameEngine.GetCurrentAction(), "Unexpected action");

            IList<int> currentPlayers = gameEngine.GetCurrentPlayers();

            Assert.AreEqual(1, currentPlayers.Count, "Unexpected player count");
            Assert.AreEqual(0, currentPlayers[0], "Unexpected current player");
        }

        [TestMethod]
        public void GameEngine_GetCurrentAction_ScoreHand_Dealer()
        {
            // Set up state where round is done and non-dealer (PlayerA) has provided 
            // their score.
            GameState startingState = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.ScoreHands,
                Dealer = 1,
            };

            startingState.CurrentPlayers.Add(0);

            GameEngine gameEngine = new GameEngine(startingState);

            gameEngine.IsProvidedScoreCorrectForHand(0, 10);

            // Verify the system knows the dealer (PlayerB) still has to provide 
            // a score.
            Assert.AreEqual(PlayerAction2.ScoreHands, gameEngine.GetCurrentAction(), "Unexpected action");

            IList<int> currentPlayers = gameEngine.GetCurrentPlayers();

            Assert.AreEqual(1, currentPlayers.Count, "Unexpected player count");
            Assert.AreEqual(1, currentPlayers[0], "Unexpected current player");
        }

        [TestMethod]
        public void GameEngine_GetCurrentAction_ScoreCrib()
        {
            // Set up state so that PlayerB is dealer and game at stage
            // where dealer needs to score their hand.
            GameState startingState = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.ScoreHands,
                Dealer = 1,
            };

            startingState.CurrentPlayers.Add(0);

            GameEngine gameEngine = new GameEngine(startingState);

            gameEngine.IsProvidedScoreCorrectForHand(0, 10);

            // Verify that when the dealer (PlayerB) scores their hand the
            // game moves to the stage where the crib is calculated.
            gameEngine.IsProvidedScoreCorrectForHand(1, 10);

            Assert.AreEqual(PlayerAction2.ScoreCrib, gameEngine.GetCurrentAction(), "Unexpected action");

            IList<int> currentPlayers = gameEngine.GetCurrentPlayers();
            
            Assert.AreEqual(1, currentPlayers.Count, "Unexpected player count");
            Assert.AreEqual(1, currentPlayers[0], "Unexpected current player");
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
        [ExpectedException(typeof(ApplicationException))]
        public void GameEngine_PlayCard_InvalidActionForCurrentStage()
        {
            Mock<IDeck> mockDeck = CreateMockDeck();

            GameEngine gameEngine = new GameEngine(mockDeck.Object, testPlayerNames);

            gameEngine.PlayCard(1, new Card(CardSuit.Diamond, CardFace.Nine));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GameEngine_PlayCard_WrongCard()
        {
            Card testCard = new Card(CardSuit.Diamond, CardFace.Ace);

            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 1,
                Stage = PlayerAction2.PlayOrPass
            };

            state.Players[1].Hand.Cards.Add(testCard);

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.PlayCard(1, new Card(CardSuit.Heart, CardFace.Ten));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GameEngine_PlayCard_NotPlayersTurn()
        {
            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 1,
                Stage = PlayerAction2.PlayOrPass
            };

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.PlayCard(0, new Card(CardSuit.Heart, CardFace.Eight));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GameEngine_PlayCard_NoValidCardsToPlay()
        {
            Card testCard = new Card(CardSuit.Diamond, CardFace.Four);

            GameState state = new GameState(testPlayerNames)
            {
                PlayerTurn = 1,
                Stage = PlayerAction2.PlayOrPass,
                SumOfPlayedCards = 28
            };

            // Player 0 has playable card but it is Player 1's turn.
            state.Players[1].Hand.Cards.Add(testCard);
            state.Players[0].Hand.Cards.Add(new Card(CardSuit.Diamond, CardFace.Ace));

            GameEngine gameEngine = new GameEngine(state);

            // Invalid because this would exceed 31
            gameEngine.PlayCard(1, testCard);
        }

        [TestMethod]
        public void GameEngine_PlayCard()
        {
            Card testCard = new Card(CardSuit.Diamond, CardFace.Ace);

            GameState state = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.PlayOrPass
            };

            state.CurrentPlayers.Add(1);
            state.Players[1].Hand.Cards.Add(testCard);

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.PlayCard(1, testCard);
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
        public void GameEngine_PlayerPass_InvalidActionForCurrentStage()
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
                Stage = PlayerAction2.PlayOrPass
            };

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.PlayerPass(0);
        }

        [TestMethod]
        public void GameEngine_PlayerPass()
        {
            GameState state = new GameState(testPlayerNames)
            {
                SumOfPlayedCards = 30,
                Stage = PlayerAction2.PlayOrPass
            };

            state.CurrentPlayers.Add(1);

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.PlayerPass(1);
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

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GameEngine_AddToCrib_PlayerIndexTooLow()
        {
            GameState state = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.CreateCrib
            };

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.AddToCrib(-1, new Card(CardSuit.Diamond, CardFace.Eight));
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void GameEngine_AddToCrib_PlayerIndexTooHigh()
        {
            GameState state = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.CreateCrib
            };

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.AddToCrib(2, new Card(CardSuit.Diamond, CardFace.Eight));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GameEngine_AddToCrib_NotPlayersTurn()
        {
            GameState state = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.CreateCrib
            };

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.AddToCrib(0, new Card(CardSuit.Heart, CardFace.Eight));
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void GameEngine_AddToCrib_InvalidActionForCurrentStage()
        {
            GameState state = new GameState(testPlayerNames);
            GameEngine gameEngine = new GameEngine(state);

            gameEngine.AddToCrib(0, new Card(CardSuit.Heart, CardFace.Eight));
        }

        [TestMethod]
        public void GameEngine_AddToCrib()
        {
            GameState state = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.CreateCrib
            };

            Card testCard = new Card(CardSuit.Heart, CardFace.Eight);
            state.Players[1].Hand.Cards.Add(testCard);
            state.CurrentPlayers.Add(1);

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.AddToCrib(1, testCard);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void GameEngine_Deal_InvalidActionForCurrentStage()
        {
            GameState state = new GameState(testPlayerNames)
            {
                Stage = PlayerAction2.PlayOrPass
            };

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.DealCards();
        }

        [TestMethod]
        public void GameEngine_Deal()
        {
            GameState state = new GameState(testPlayerNames)
            {
                // TODO: Shouldn't have to set this to .Deal when refactoring done
                Stage = PlayerAction2.Deal
            };

            GameEngine gameEngine = new GameEngine(state);

            gameEngine.DealCards();

            Assert.AreEqual(PlayerAction2.CreateCrib, gameEngine.GetCurrentAction(), "Unexpected current action");
            Assert.AreEqual(2, gameEngine.GetCurrentPlayers().Count, "Unexpected current player count");
        }
    }
}
