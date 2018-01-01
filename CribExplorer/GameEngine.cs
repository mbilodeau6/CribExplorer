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
            state.CurrentPlayers.Clear();

            while (state.PlayerTurn < 0)
            {
                int playerOneCardValue = deck.GetNextCard().Value;
                int playerTwoCardValue = deck.GetNextCard().Value;

                // TODO: Need to handle more than 2 players
                if (playerOneCardValue < playerTwoCardValue)
                {
                    state.PlayerTurn = 0;
                    state.CurrentPlayers.Add(0);
                }
                else if (playerOneCardValue > playerTwoCardValue)
                {
                    state.PlayerTurn = 1;
                    state.CurrentPlayers.Add(1);
                }

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


        public IList<int> GetCurrentPlayers()
        {
            return state.CurrentPlayers;
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

        public int ProvideScoreForCrib(int providedScore)
        {
            // TODO: Add real implementation
            return 0;
        }

        public PlayerAction2 GetCurrentAction()
        {
            PlayerAction2 nextAction = state.Stage;

            // TODO: Need to rename or refactor more. Do I need this routine to 
            // transition to the next state? If yes, it shouldn't be a Get method.
            switch (state.Stage)
            {
                case PlayerAction2.CreateCrib:
                    if (state.Crib.Count == GameEngine.RequiredHandCardCount)
                    {
                        state.CurrentPlayers.Clear();
                        state.CurrentPlayers.Add(state.Dealer);
                        nextAction = PlayerAction2.PlayOrPass;
                    }
                    break;
                case PlayerAction2.PlayOrPass:
                    if (state.AllCardsPlayed())
                    {
                        nextAction = PlayerAction2.ScoreHands;
                        state.CurrentPlayers.Clear();
                        state.CurrentPlayers.Add(GetNextPlayerIndex(state.Dealer));
                        state.AllHandScoresProvided = false;
                    }
                    else if (state.SumOfPlayedCards == 31 || !state.CardsPlayable())
                        state.SumOfPlayedCards = 0;
                    break;
                case PlayerAction2.ScoreHands:
                    if (state.AllHandScoresProvided)
                    {
                        nextAction = PlayerAction2.ScoreCrib;
                        state.CurrentPlayers.Clear();
                        state.CurrentPlayers.Add(state.Dealer);
                    }

                    break;
                case PlayerAction2.ScoreCrib:
                    state.Dealer = GetNextPlayerIndex(state.Dealer);
                    state.CurrentPlayers.Clear();
                    state.CurrentPlayers.Add(state.Dealer);
                    nextAction = PlayerAction2.Deal;
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

            state.PlayerTurn = GetNextPlayerIndex(state.PlayerTurn);
        }

        public void PlayCard(int playerIndex, Card card)
        {
            if (playerIndex < 0 || playerIndex >= state.Players.Count)
                throw new IndexOutOfRangeException(string.Format("Invalid player index of {0}", playerIndex));

            if (state.Stage != PlayerAction2.PlayOrPass)
                throw new ApplicationException("Invalid game stage to play a card");

            if (!state.Players[playerIndex].Hand.Cards.Contains(card))
                throw new ArgumentException(string.Format("Player {0} does not have the card requested", playerIndex));

            if (state.SumOfPlayedCards + card.Value > 31)
                throw new ArgumentException("Playing the selected card would put the count over 31");

            if (!state.CurrentPlayers.Contains(playerIndex))
                throw new ArgumentException(string.Format("It isn't Player {0}'s turn.", playerIndex));

            state.Players[playerIndex].Discard(card);
            state.SumOfPlayedCards += card.Value;
            MoveToNextPlayer();    
        }

        public void PlayerPass(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= state.Players.Count)
                throw new IndexOutOfRangeException(string.Format("Invalid player index of {0}", playerIndex));

            if (state.Stage != PlayerAction2.PlayOrPass)
                throw new ApplicationException("Invalid game stage to pass");

            if (!state.CurrentPlayers.Contains(playerIndex))
                throw new ArgumentException(string.Format("It is not Player {0}'s turn", playerIndex));

            if (state.CardsPlayable(state.Players[playerIndex]))
                throw new ApplicationException(string.Format("Player {0} has playable cards and can not pass.", playerIndex));

            MoveToNextPlayer();
        }

        public bool IsProvidedScoreCorrectForHand(int playerId, int score)
        {
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

        public void AddToCrib(int playerIndex, Card card)
        {
            Debug.Assert(state.Crib.Count <= GameEngine.RequiredHandCardCount);

            if (playerIndex < 0 || playerIndex >= state.Players.Count)
                throw new IndexOutOfRangeException(string.Format("Invalid player index of {0}", playerIndex));

            if (state.Stage != PlayerAction2.CreateCrib)
                throw new ApplicationException("Invalid game stage to contribute to crib");

            if (!state.CurrentPlayers.Contains(playerIndex))
                throw new ArgumentException(string.Format("Player {0} does not need to contribute to the crib", playerIndex));

            if (!state.Players[playerIndex].Hand.Cards.Contains(card))
                throw new ArgumentException(string.Format("Player {0} does not have the card requested", playerIndex));

            state.Players[playerIndex].Hand.Cards.Remove(card);
            state.Crib.Add(card);

            if (state.Players[playerIndex].Hand.Cards.Count <= GameEngine.RequiredHandCardCount)
                state.CurrentPlayers.Remove(playerIndex);
        }

        public void DealCards()
        {
            if (state.Stage != PlayerAction2.Deal && state.Stage != PlayerAction2.NewGame)
                throw new ApplicationException("Invalid game stage to deal");

            state.Stage = PlayerAction2.CreateCrib;

            state.CurrentPlayers.Clear();
            for (int i = 0; i < state.Players.Count; i++)
                state.CurrentPlayers.Add(i);
        }
    }
}
