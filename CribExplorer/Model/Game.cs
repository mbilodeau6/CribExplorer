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
        // TODO: Need to clarify distinction between Game and GameEngine.
        // Do I really need both? If yes, what goes where?

        private IDeck deck;
        private GameState gameState;
        private GameEngine gameEngine;

        /// <summary>
        /// Called to start a new game. By default, the gameState will be determined
        /// by the drawing of cards (i.e. a real game) although caller has the option
        /// to specify a starting state.
        /// </summary>
        /// <param name="deck">The deck to draw from.</param>
        /// <param name="playerNames">Players participating the in the game.</param>
        /// <param name="gameState">Optional starting state</param>
        public Game(IDeck deck, IList<string> playerNames, GameState gameState = null)
        {
            if (deck == null)
                throw new ArgumentNullException("deck");

            if (playerNames.Count != 2)
                throw new NotImplementedException("Current version only supports 2 players");

            this.deck = deck;
            this.gameState = gameState == null ? new GameState(playerNames) : gameState;
            this.gameEngine = new GameEngine(this.gameState);

            if (gameState == null)
            {
                StartNew();
            }
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
            gameEngine.PlayerPass(playerIndex);
        }

        public void PlayCard(int playerIndex, Card card)
        {
            gameEngine.PlayCard(playerIndex, card);
        }

        public bool IsProvidedScoreCorrectForHand(int playerIndex, int score)
        {
            if (playerIndex < 0 || playerIndex >= gameState.Players.Count)
                throw new IndexOutOfRangeException(string.Format("Invalid player index of {0}", playerIndex));

            if (gameState.PlayerTurn != playerIndex)
                throw new ArgumentException(string.Format("It isn't Player {0}'s turn.", playerIndex));

            // TODO: Need to figure out what we want to do if the incorrect score is 
            // provided. Probably a configurable option where they are: a) just 
            // notified; b) lose points; c) if another player challenges, the 
            // challenging player gets the difference.

            return gameEngine.IsProvidedScoreCorrectForHand(playerIndex, score);
        }

        public bool IsProvidedScoreCorrectForCrib(int score)
        {
            // TODO: Need to figure out what we want to do if the incorrect score is 
            // provided. Probably a configurable option where they are: a) just 
            // notified; b) lose points; c) if another player challenges, the 
            // challenging player gets the difference.

            return gameEngine.IsProvidedScoreCorrectForCrib(score);
        }

        public PlayerAction GetNextAction()
        {
            PlayerAction action = new PlayerAction();

            switch(gameEngine.GetNextStage())
            {
                case PlayerAction2.NewRound:
                case PlayerAction2.StartRound:
                    // TODO: This isn't intuitive...Need to refactor/rename. GetNextStage is advancing the game.
                    gameEngine.GetNextStage();
                    break;
                case PlayerAction2.CreateCrib:
                    action.Action = PlayerAction.ActionType.SelectCardForCrib;

                    // Check which players still need to contribute to the crib
                    foreach(Player player in gameState.Players)
                    {
                        if (player.Hand.Cards.Count > GameEngine.RequiredHandCardCount)
                            action.Players.Add(player.Name);
                    }
                    break;
                case PlayerAction2.NewPlay:
                    if (gameState.CardsPlayable(gameState.Players[gameState.PlayerTurn]))
                        action.Action = PlayerAction.ActionType.PlayCard;
                    else
                        action.Action = PlayerAction.ActionType.PlayerMustPass;

                    action.Players.Add(gameState.Players[gameState.PlayerTurn].Name);
                    break;
                case PlayerAction2.EndPlay:
                    gameState.SumOfPlayedCards = 0;
                    break;
                case PlayerAction2.ScoreHands:
                    action.Players.Add(gameState.Players[gameState.PlayerTurn].Name);
                    action.Action = PlayerAction.ActionType.CalculateScore;
                    break;
                case PlayerAction2.ScoreCrib:
                    action.Players.Add(gameState.Players[gameState.Dealer].Name);
                    action.Action = PlayerAction.ActionType.CalculateCribScore;
                    break;
                default:
                    action.Action = PlayerAction.ActionType.NoAction;
                    break;
            }

            return action;
        }

    }
}
