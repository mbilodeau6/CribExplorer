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

        public Game(IDeck deck, IList<string> playerNames)
        {
            if (deck == null)
                throw new ArgumentNullException("deck");

            if (playerNames.Count != 2)
                throw new NotImplementedException("Current version only supports 2 players");

            this.deck = deck;
            this.gameState = new GameState(playerNames);

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

        public void StartNew()
        {
            deck.Shuffle();

            while (gameState.PlayerTurn < 0)
            {
                int playerOneCardValue = deck.GetNextCard().Value;
                int playerTwoCardValue = deck.GetNextCard().Value;

                if (playerOneCardValue < playerTwoCardValue)
                    gameState.PlayerTurn = 0;
                else if (playerOneCardValue > playerTwoCardValue)
                    gameState.PlayerTurn = 1;
            }

            // TODO: Need to deal more than 4 cards for the crib
            for (int i = 0; i < 4; i++)
            {
                gameState.Players[0].Hand.Cards.Add(deck.GetNextCard());
                gameState.Players[1].Hand.Cards.Add(deck.GetNextCard());
            }

            // REVIEW: Should I introduce a method to cut for the Starter?
            gameState.Starter = deck.GetNextCard();
        }

        public void AddToCrib(Card card)
        {
            Debug.Assert(gameState.Crib.Count <= GameEngine.RequiredHandCardCount);

            gameState.Crib.Add(card);
        }

        public void PlayCard(int playerIndex, Card card)
        {
            if (playerIndex <= 0 || playerIndex >= gameState.Players.Count)
                throw new IndexOutOfRangeException(string.Format("Invalid player index of {0}", playerIndex));

            if (!gameState.Players[playerIndex].Hand.Cards.Contains(card))
                throw new ArgumentException(string.Format("Player {0} does not have the card requested", playerIndex));

            gameState.Players[playerIndex].Discard(card);
        }

    }
}
