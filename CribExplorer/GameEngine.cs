using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CribExplorer.Model;

namespace CribExplorer
{
    public class GameEngine
    {
        public enum GameStage
        {
            NewGame,
            NewRound,
            CreateCrib,
            StartRound,
            NewPlay,
            EndPlay,
            ScoreHands,
            ScoreCrib,
            EndRound,
            EndGame
        }

        private GameState state;
        private IDeck deck;

        public const int WinningScore = 121;
        public const int RequiredHandCardCount = 4;

        public GameEngine(IDeck deck, IList<string> playerNames)
        {
            if (deck == null)
                throw new ArgumentNullException("deck");

            if (playerNames == null)
                throw new ArgumentNullException("playerNames");

            if (playerNames.Count != 2)
                throw new IndexOutOfRangeException("Current version only supports 2 players");

            this.deck = deck;
            this.state = new GameState(playerNames);

            StartNew();
        }

        public GameEngine(GameState gameState)
        {
            if (gameState == null)
                throw new ArgumentNullException("gameState");

            this.state = gameState;
        }

        private void StartNew()
        {
            deck.Shuffle();

            while (state.PlayerTurn < 0)
            {
                int playerOneCardValue = deck.GetNextCard().Value;
                int playerTwoCardValue = deck.GetNextCard().Value;

                // TODO: Need to handle more than 2 players
                if (playerOneCardValue < playerTwoCardValue)
                    state.PlayerTurn = 0;
                else if (playerOneCardValue > playerTwoCardValue)
                    state.PlayerTurn = 1;

                state.Dealer = state.PlayerTurn;
            }

            for (int i = 0; i < GetCardCountToDeal(); i++)
            {
                // TODO: Need to handle more than 2 players
                state.Players[0].Hand.Cards.Add(deck.GetNextCard());
                state.Players[1].Hand.Cards.Add(deck.GetNextCard());
            }

            // REVIEW: Should I introduce a method to cut for the Starter?
            state.Starter = deck.GetNextCard();
        }

        public int GetMaxTotalHandCount()
        {
            return RequiredHandCardCount * state.Players.Count;
        }

        public int GetCardCountToDeal()
        {
            switch (state.Players.Count)
            {
                case 2:
                    return 6;
                case 3:
                case 4:
                    return 5;
            }

            throw new ApplicationException("Crib games require 2 to 4 players.");
        }

        public PlayerAction.ActionType GetCurrentAction()
        {
            // TODO: Add real implementation
            return PlayerAction.ActionType.Deal;
        }

        public IList<int> GetCurrentPlayer()
        {
            // TODO: Add real implementation
            return new List<int>() { 1 };
        }

        public int GetDealer()
        {
            // TODO: Add real implementation
            return 1;
        }

        public Hand GetPlayerHand(int playerId)
        {
            // TODO: Add real implementation
            Hand playerHand = new Hand();

            for (int i = 0; i < 6; i++)
                playerHand.Cards.Add(null);

            return playerHand;
        }

        public Hand GetCrib()
        {
            // TODO: Add real implementation
            return new Hand();
        }

        public Card GetStarterCard()
        {
            // TODO: Add real implementation
            return new Card(CardSuit.Diamond, CardFace.Six);
        }

        public Hand GetPlayerDiscards(int playerId)
        {
            // TODO: Add real implementation
            return new Hand();
        }

        public GameStage GetNextStage()
        {
            // TODO: Need to add GameWon stage that can be hit on any stage where
            // points are earned.

            if (state.Stage == GameEngine.GameStage.NewGame && state.PlayerTurn < 0)
                return state.Stage;

            if (state.Stage == GameEngine.GameStage.NewGame && state.PlayerTurn >= 0)
                return (state.Stage = GameEngine.GameStage.NewRound);

            if (state.Stage == GameEngine.GameStage.NewRound && state.Players[0].Hand.Cards.Count > 0)
                return (state.Stage = GameEngine.GameStage.CreateCrib);

            if (state.Stage == GameEngine.GameStage.CreateCrib && state.Crib.Count == GameEngine.RequiredHandCardCount)
                return (state.Stage = GameEngine.GameStage.StartRound);

            if (state.Stage == GameEngine.GameStage.CreateCrib)
                return state.Stage;

            if (state.Stage == GameEngine.GameStage.StartRound && state.Starter != null)
                return (state.Stage = GameEngine.GameStage.NewPlay);

            if (state.Stage == GameEngine.GameStage.NewPlay)
            {
                if (state.SumOfPlayedCards == 31 || state.AllCardsPlayed() || !state.CardsPlayable())
                    return (state.Stage = GameEngine.GameStage.EndPlay);
                else
                    return state.Stage;
            }

            if (state.Stage == GameEngine.GameStage.EndPlay)
            {
                if (state.AllCardsPlayed())
                {
                    state.AllHandScoresProvided = false;
                    state.PlayerTurn = GetNextPlayerIndex(state.Dealer);
                    return (state.Stage = GameEngine.GameStage.ScoreHands);
                }
                else
                    return (state.Stage = GameEngine.GameStage.NewPlay);
            }

            if (state.Stage == GameStage.ScoreHands)
            {
                if (state.AllHandScoresProvided)
                    return (state.Stage = GameEngine.GameStage.ScoreCrib);

                if (state.Dealer == state.PlayerTurn)
                    state.AllHandScoresProvided = true;

                return state.Stage;
            }

            if (state.Stage == GameStage.ScoreCrib)
                return (state.Stage = GameEngine.GameStage.EndRound);

            if (state.Stage == GameEngine.GameStage.EndRound)
            {
                if (state.GetWinningPlayer() >= 0)
                    return (state.Stage = GameEngine.GameStage.EndGame);
                else
                    return (state.Stage = GameEngine.GameStage.NewRound);
            }

            throw new ApplicationException("Invalid state.");
        }

        public int GetNextPlayerIndex(int currentPlayer)
        {
            return (currentPlayer + 1) % state.Players.Count;
        }

        private void MoveToNextPlayer()
        {
            state.PlayerTurn = GetNextPlayerIndex(state.PlayerTurn);
        }

        public void PlayCard(int playerId, Card card)
        {
            state.Players[playerId].Discard(card);
            state.SumOfPlayedCards += card.Value;
            MoveToNextPlayer();    
        }

        public void PlayerPass(int playerId)
        {
            MoveToNextPlayer();
        }

        public bool IsProvidedScoreCorrectForHand(int playerId, int score)
        {
            // TODO: Add logic and tests to check score.

            MoveToNextPlayer();
            return true;
        }

        public bool IsProvidedScoreCorrectForCrib(int score)
        {
            // TODO: Add logic and tests to check score.
            return true;
        }

    }
}
