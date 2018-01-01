using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CribExplorer.Model;

namespace CribExplorer
{
    public class GameEngine
    {
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

            // TODO: Need to handle 2 to 4 players
            if (playerNames.Count != 2)
                throw new ArgumentOutOfRangeException("Current version only supports 2 players");

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
            state.CurrentPlayers.Clear();

            while (state.CurrentPlayers.Count == 0)
            {
                int playerOneCardValue = deck.GetNextCard().Value;
                int playerTwoCardValue = deck.GetNextCard().Value;

                // TODO: Need to handle more than 2 players
                if (playerOneCardValue < playerTwoCardValue)
                {
                    state.CurrentPlayers.Add(0);
                }
                else if (playerOneCardValue > playerTwoCardValue)
                {
                    state.CurrentPlayers.Add(1);
                }
            }

            state.Dealer = state.CurrentPlayers[0];

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


        public IList<int> GetCurrentPlayers()
        {
            return state.CurrentPlayers;
        }

        public int GetDealer()
        {
            return state.Dealer;
        }

        public Hand GetPlayerHand(int playerId)
        {
            ValidatePlayerId(playerId);

            return state.Players[playerId].Hand;
        }

        public string GetPlayerName(int playerId)
        {
            ValidatePlayerId(playerId);

            return state.Players[playerId].Name;
        }

        public Hand GetCrib()
        {
            return state.Crib;
        }

        public Card GetStarterCard()
        {
            return state.Starter;
        }

        public Hand GetPlayerDiscards(int playerId)
        {
            ValidatePlayerId(playerId);

            return state.Players[playerId].Discards;
        }

        public PlayerAction GetCurrentAction()
        {
            PlayerAction nextAction = state.Stage;

            // TODO: Need to rename or refactor more. Do I need this routine to 
            // transition to the next state? If yes, it shouldn't be a Get method.
            switch (state.Stage)
            {
                case PlayerAction.CreateCrib:
                    if (state.Crib.Cards.Count == GameEngine.RequiredHandCardCount)
                    {
                        state.CurrentPlayers.Clear();
                        state.CurrentPlayers.Add(state.Dealer);
                        nextAction = PlayerAction.PlayOrPass;
                    }
                    break;
                case PlayerAction.PlayOrPass:
                    if (state.AllCardsPlayed())
                    {
                        nextAction = PlayerAction.ScoreHands;
                        state.CurrentPlayers.Clear();
                        state.CurrentPlayers.Add(GetNextPlayerIndex(state.Dealer));
                        state.AllHandScoresProvided = false;
                    }
                    else if (state.SumOfPlayedCards == 31 || !state.CardsPlayable())
                        state.SumOfPlayedCards = 0;
                    break;
                case PlayerAction.ScoreHands:
                    if (state.AllHandScoresProvided)
                    {
                        nextAction = PlayerAction.ScoreCrib;
                        state.CurrentPlayers.Clear();
                        state.CurrentPlayers.Add(state.Dealer);
                    }

                    break;
                case PlayerAction.ScoreCrib:
                    state.Dealer = GetNextPlayerIndex(state.Dealer);
                    state.CurrentPlayers.Clear();
                    state.CurrentPlayers.Add(state.Dealer);
                    nextAction = PlayerAction.Deal;
                    break;
            }

            state.Stage = nextAction;

            return nextAction;
        }

        public int GetNextPlayerIndex(int currentPlayer)
        {
            return (currentPlayer + 1) % state.Players.Count;
        }

        private void MoveToNextPlayer()
        {
            Debug.Assert(state.CurrentPlayers.Count == 1, "Can not move to next player and no single player is marked as current.");

            int currentPlayer = state.CurrentPlayers[0];
            state.CurrentPlayers.Clear();
            state.CurrentPlayers.Add(GetNextPlayerIndex(currentPlayer));
        }

        public void PlayCard(int playerId, Card card)
        {
            ValidatePlayerId(playerId);

            if (state.Stage != PlayerAction.PlayOrPass)
                throw new ApplicationException("Invalid game stage to play a card");

            if (!state.Players[playerId].Hand.Cards.Contains(card))
                throw new ArgumentException(string.Format("Player {0} does not have the card requested", playerId));

            if (state.SumOfPlayedCards + card.Value > 31)
                throw new ArgumentException("Playing the selected card would put the count over 31");

            if (!state.CurrentPlayers.Contains(playerId))
                throw new ArgumentException(string.Format("It isn't Player {0}'s turn.", playerId));

            state.Players[playerId].Discard(card);
            state.SumOfPlayedCards += card.Value;
            MoveToNextPlayer();    
        }

        public void PlayerPass(int playerId)
        {
            ValidatePlayerId(playerId);

            if (state.Stage != PlayerAction.PlayOrPass)
                throw new ApplicationException("Invalid game stage to pass");

            if (!state.CurrentPlayers.Contains(playerId))
                throw new ArgumentException(string.Format("It is not Player {0}'s turn", playerId));

            if (state.CardsPlayable(state.Players[playerId]))
                throw new ApplicationException(string.Format("Player {0} has playable cards and can not pass.", playerId));

            MoveToNextPlayer();
        }

        public bool IsProvidedScoreCorrectForHand(int playerId, int score)
        {
            ValidatePlayerId(playerId);

            if (playerId == state.Dealer)
                state.AllHandScoresProvided = true;

            // TODO: Add logic and tests to check score.
            MoveToNextPlayer();
            return true;
        }

        public bool IsProvidedScoreCorrectForCrib(int score)
        {
            // TODO: Add logic and tests to check score.
            return true;
        }

        public void AddToCrib(int playerId, Card card)
        {
            Debug.Assert(state.Crib.Cards.Count <= GameEngine.RequiredHandCardCount);

            ValidatePlayerId(playerId);

            if (state.Stage != PlayerAction.CreateCrib)
                throw new ApplicationException("Invalid game stage to contribute to crib");

            if (!state.CurrentPlayers.Contains(playerId))
                throw new ArgumentException(string.Format("Player {0} does not need to contribute to the crib", playerId));

            if (!state.Players[playerId].Hand.Cards.Contains(card))
                throw new ArgumentException(string.Format("Player {0} does not have the card requested", playerId));

            state.Players[playerId].Hand.Cards.Remove(card);
            state.Crib.Cards.Add(card);

            if (state.Players[playerId].Hand.Cards.Count <= GameEngine.RequiredHandCardCount)
                state.CurrentPlayers.Remove(playerId);
        }

        public void DealCards()
        {
            if (state.Stage != PlayerAction.Deal)
                throw new ApplicationException("Invalid game stage to deal");

            state.Stage = PlayerAction.CreateCrib;

            state.CurrentPlayers.Clear();
            for (int i = 0; i < state.Players.Count; i++)
                state.CurrentPlayers.Add(i);
        }

        private void ValidatePlayerId(int playerId)
        {
            if (playerId < 0 || playerId >= state.Players.Count)
                throw new ArgumentOutOfRangeException(string.Format("Invalid player id of {0}", playerId));
        }
    }
}
