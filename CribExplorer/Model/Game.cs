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
        private int playerTurn;

        public Game(IDeck deck, int playerCount)
        {
            if (deck == null)
                throw new ArgumentNullException("deck");

            if (playerCount != 2)
                throw new NotImplementedException("Current version only supports 2 players");

            this.deck = deck;
            this.Players = new List<Player>(playerCount);

            for (int i = 0; i < playerCount; i++)
                this.Players.Add(new Player());

            // TODO: Add code to handle ties
            if (deck.GetNextCard().Value < deck.GetNextCard().Value)
                playerTurn = 0;
            else
                playerTurn = 1;
        }

        public int PlayerTurn
        {
            get
            {
                return playerTurn;
            }

            private set
            {
                playerTurn = value;
            }
        }

        public Card Starter
        {
            get;
            private set;
        }

        public IList<Player> Players
        {
            get;
            private set;
        }
    }
}
