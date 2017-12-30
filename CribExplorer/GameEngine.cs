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
            PlayerAction2 nextAction;

            // TODO: Add real implementation
            switch (state.Stage)
            {
                case PlayerAction2.ScoreCrib:
                    state.Dealer = GetNextPlayerIndex(state.Dealer);
                    state.CurrentPlayers.Clear();
                    state.CurrentPlayers.Add(state.Dealer);
                    nextAction = PlayerAction2.Deal;
                    break;
                default:
                    nextAction = PlayerAction2.Deal;
                    break;
            }

            return nextAction;
        }

        public PlayerAction2 GetNextStage()
        {
            // TODO: Need to add GameWon stage that can be hit on any stage where
            // points are earned.

            if (state.Stage == PlayerAction2.NewGame && state.PlayerTurn < 0)
                return state.Stage;

            if (state.Stage == PlayerAction2.NewGame && state.PlayerTurn >= 0)
                return (state.Stage = PlayerAction2.NewRound);

            if (state.Stage == PlayerAction2.NewRound && state.Players[0].Hand.Cards.Count > 0)
                return (state.Stage = PlayerAction2.CreateCrib);

            if (state.Stage == PlayerAction2.CreateCrib && state.Crib.Count == GameEngine.RequiredHandCardCount)
                return (state.Stage = PlayerAction2.StartRound);

            if (state.Stage == PlayerAction2.CreateCrib)
                return state.Stage;

            if (state.Stage == PlayerAction2.StartRound && state.Starter != null)
                return (state.Stage = PlayerAction2.NewPlay);

            if (state.Stage == PlayerAction2.NewPlay)
            {
                if (state.SumOfPlayedCards == 31 || state.AllCardsPlayed() || !state.CardsPlayable())
                    return (state.Stage = PlayerAction2.EndPlay);
                else
                    return state.Stage;
            }

            if (state.Stage == PlayerAction2.EndPlay)
            {
                if (state.AllCardsPlayed())
                {
                    state.AllHandScoresProvided = false;
                    state.PlayerTurn = GetNextPlayerIndex(state.Dealer);
                    return (state.Stage = PlayerAction2.ScoreHands);
                }
                else
                    return (state.Stage = PlayerAction2.NewPlay);
            }

            if (state.Stage == PlayerAction2.ScoreHands)
            {
                if (state.AllHandScoresProvided)
                    return (state.Stage = PlayerAction2.ScoreCrib);

                if (state.Dealer == state.PlayerTurn)
                    state.AllHandScoresProvided = true;

                return state.Stage;
            }

            if (state.Stage == PlayerAction2.ScoreCrib)
                return (state.Stage = PlayerAction2.EndRound);

            if (state.Stage == PlayerAction2.EndRound)
            {
                if (state.GetWinningPlayer() >= 0)
                    return (state.Stage = PlayerAction2.EndGame);
                else
                    return (state.Stage = PlayerAction2.NewRound);
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

        public void PlayCard(int playerIndex, Card card)
        {
            if (playerIndex < 0 || playerIndex >= state.Players.Count)
                throw new IndexOutOfRangeException(string.Format("Invalid player index of {0}", playerIndex));

            if (!state.Players[playerIndex].Hand.Cards.Contains(card))
                throw new ArgumentException(string.Format("Player {0} does not have the card requested", playerIndex));

            if (state.SumOfPlayedCards + card.Value > 31)
                throw new ArgumentException("Playing the selected card would put the count over 31");

            if (state.PlayerTurn != playerIndex)
                throw new ArgumentException(string.Format("It isn't Player {0}'s turn.", playerIndex));

            state.Players[playerIndex].Discard(card);
            state.SumOfPlayedCards += card.Value;
            MoveToNextPlayer();    
        }

        public void PlayerPass(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= state.Players.Count)
                throw new IndexOutOfRangeException(string.Format("Invalid player index of {0}", playerIndex));

            if (playerIndex != state.PlayerTurn)
                throw new ArgumentException(string.Format("It is not Player {0}'s turn", playerIndex));

            if (state.CardsPlayable(state.Players[playerIndex]))
                throw new ApplicationException(string.Format("Player {0} has playable cards and can not pass.", playerIndex));

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
