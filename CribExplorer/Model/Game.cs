using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer.Model
{
    public class Game
    {

        private IDeck deck = new Deck();
        private GameState gameState;
        private GameEngine gameEngine;

        public Game(IDeck deck, IList<string> playerNames)
        {
            if (deck == null)
                throw new ArgumentNullException("deck");

            if (playerNames.Count != 2)
                throw new NotImplementedException("Current version only supports 2 players");

            this.deck = deck;
            this.gameState = new GameState(playerNames);
            this.gameEngine = new GameEngine(this.gameState);

            StartNew();
        }

        public int PlayerTurn
        {
            get
            {
                return gameState.PlayerTurn;
            }
        }

        public Card Starter
        {
            get
            {
                return gameState.Starter;
            }
        }

        public IList<Player> Players
        {
            get
            {
                return gameState.Players;
            }
        }

        public IList<Card> Crib
        {
            get
            {
                return gameState.Crib;
            }
        }

        private int currentTurnForScoring;

        public void StartNew()
        {
            deck.Shuffle();

            currentTurnForScoring = -1;

            while (gameState.PlayerTurn < 0)
            {
                int playerOneCardValue = deck.GetNextCard().Value;
                int playerTwoCardValue = deck.GetNextCard().Value;

                // TODO: Need to handle more than 2 players
                if (playerOneCardValue < playerTwoCardValue)
                    gameState.PlayerTurn = 0;
                else if (playerOneCardValue > playerTwoCardValue)
                    gameState.PlayerTurn = 1;

                gameState.Dealer = gameState.PlayerTurn;
            }

            for (int i = 0; i < gameEngine.GetCardCountToDeal(); i++)
            {
                // TODO: Need to handle more than 2 players
                gameState.Players[0].Hand.Cards.Add(deck.GetNextCard());
                gameState.Players[1].Hand.Cards.Add(deck.GetNextCard());
            }

            // REVIEW: Should I introduce a method to cut for the Starter?
            gameState.Starter = deck.GetNextCard();
        }

        public void AddToCrib(int playerIndex, Card card)
        {
            Debug.Assert(gameState.Crib.Count <= GameEngine.RequiredHandCardCount);

            if (playerIndex < 0 || playerIndex >= gameState.Players.Count)
                throw new IndexOutOfRangeException(string.Format("Invalid player index of {0}", playerIndex));

            if (!gameState.Players[playerIndex].Hand.Cards.Contains(card))
                throw new ArgumentException(string.Format("Player {0} does not have the card requested", playerIndex));

            gameState.Players[playerIndex].Hand.Cards.Remove(card);
            gameState.Crib.Add(card);
        }

        public void PlayerPass(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= gameState.Players.Count)
                throw new IndexOutOfRangeException(string.Format("Invalid player index of {0}", playerIndex));

            if (playerIndex != gameState.PlayerTurn)
                throw new ArgumentException(string.Format("It is not Player {0}'s turn", playerIndex));

            gameEngine.PlayerPass(playerIndex);
        }

        public void PlayCard(int playerIndex, Card card)
        {
            if (playerIndex < 0 || playerIndex >= gameState.Players.Count)
                throw new IndexOutOfRangeException(string.Format("Invalid player index of {0}", playerIndex));

            if (!gameState.Players[playerIndex].Hand.Cards.Contains(card))
                throw new ArgumentException(string.Format("Player {0} does not have the card requested", playerIndex));

            if (gameState.SumOfPlayedCards + card.Value > 31)
                throw new ArgumentException("Playing the selected card would put the count over 31");

            if (gameState.PlayerTurn != playerIndex)
                throw new ArgumentException(string.Format("It isn't Player {0}'s turn.", playerIndex));

            gameEngine.PlayCard(playerIndex, card);
        }

        public PlayerAction GetNextAction()
        {
            PlayerAction action = new PlayerAction();

            switch(gameEngine.GetNextStage())
            {
                case GameEngine.GameStage.NewRound:
                case GameEngine.GameStage.StartRound:
                    // TODO: This isn't intuitive...Need to refactor/rename. GetNextStage is advancing the game.
                    gameEngine.GetNextStage();
                    break;
                case GameEngine.GameStage.CreateCrib:
                    action.Action = PlayerAction.ActionType.SelectCardForCrib;

                    // Check which players still need to contribute to the crib
                    foreach(Player player in gameState.Players)
                    {
                        if (player.Hand.Cards.Count > GameEngine.RequiredHandCardCount)
                            action.Players.Add(player.Name);
                    }
                    break;
                case GameEngine.GameStage.NewPlay:
                    if (gameState.CardsPlayable(gameState.Players[gameState.PlayerTurn]))
                        action.Action = PlayerAction.ActionType.PlayCard;
                    else
                        action.Action = PlayerAction.ActionType.PlayerMustPass;

                    action.Players.Add(gameState.Players[gameState.PlayerTurn].Name);
                    break;
                case GameEngine.GameStage.EndPlay:
                    gameState.SumOfPlayedCards = 0;
                    break;
                case GameEngine.GameStage.EndRound:
                    if (currentTurnForScoring == gameState.Dealer)
                    {
                        // All hands have been counted. Count the crib.
                        action.Players.Add(gameState.Players[currentTurnForScoring].Name);
                        currentTurnForScoring = -1;
                        action.Action = PlayerAction.ActionType.CalculateCribScore;

                        // Switch Dealer
                        gameState.Dealer = gameEngine.GetNextPlayerIndex(gameState.Dealer);
                    }
                    else
                    {
                        if (currentTurnForScoring < 0)
                            // Player left of dealer starts counting
                            currentTurnForScoring = gameEngine.GetNextPlayerIndex(gameState.Dealer);
                        else
                            // Continue counting to the left
                            currentTurnForScoring = gameEngine.GetNextPlayerIndex(currentTurnForScoring);

                        action.Action = PlayerAction.ActionType.CalculateScore;
                        action.Players.Add(gameState.Players[currentTurnForScoring].Name);
                    }

                    break;
                default:
                    action.Action = PlayerAction.ActionType.NoAction;
                    break;
            }

            return action;
        }

    }
}
