using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer.Model
{
    public class Game
    {

        private IDeck deck = new Deck();
        private GameState gameState;

        public Game(IDeck deck, int playerCount)
        {
            if (deck == null)
                throw new ArgumentNullException("deck");

            if (playerCount != 2)
                throw new NotImplementedException("Current version only supports 2 players");

            this.deck = deck;
            this.gameState = new GameState(playerCount);

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

        public void StartNew()
        {
            deck.Shuffle();
            gameState.PlayerTurn = -1; 

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
        }

    }
}
