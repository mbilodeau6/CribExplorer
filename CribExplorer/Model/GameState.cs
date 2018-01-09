using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer.Model
{
    public class GameState
    {
        private IList<string> PlayerNames;

        public GameState(IList<string> playerNames)
        {
            if (playerNames == null)
                throw new ArgumentNullException("playerNames");

            if (playerNames.Count < 2 || playerNames.Count > 4)
                throw new ArgumentException("Crib requires 2 to 4 players.");

            PlayerNames = playerNames;

            // Set up game
            Stage = PlayerAction.Deal;
            CurrentPlayers = new List<int>();

            Players = new List<Player>();
            foreach (string playerName in PlayerNames)
            {
                Players.Add(new Player(playerName));
            }

            // Set up round
            ResetForNewRound();
        }

        public void ResetForNewRound()
        {
            Starter = null;
            Crib = new Hand();
            SumOfPlayedCards = 0;
            AllHandScoresProvided = false;

            foreach(Player player in Players)
            {
                player.Hand.Cards.Clear();
                player.Discards.Cards.Clear();
            }
        }

        // TODO: Should create a type for CurrentPlayers so that programmers
        // don't need to know how to use Lists and so that we can change the 
        // type without having to change code throughout the program.
        public IList<int> CurrentPlayers
        {
            get;
            set;
        }

        public int Dealer
        {
            get;
            set;
        }

        public Card Starter
        {
            get;
            set;
        }

        public IList<Player> Players
        {
            get;
            private set;
        }

        public Hand Crib
        {
            get;
            private set;
        }

        public PlayerAction Stage
        {
            get;
            set;
        }

        public int SumOfPlayedCards
        {
            get;
            set;
        }

        public bool AllHandScoresProvided
        {
            get;
            set;
        }

        public int GetWinningPlayer()
        {
            for (int i = 0; i < Players.Count; i++)
                if (Players[i].Score >= GameEngine.WinningScore)
                    return i;

            // No players have won yet
            return -1;
        }

        public bool AllCardsPlayed()
        {
            int count = 0;

            foreach (Player player in Players)
                count += player.Discards.Cards.Count;

            return count >= Players.Count * GameEngine.RequiredHandCardCount;
        }

        public bool CardsPlayable()
        {
            foreach (Player player in Players)
            {
                if (CardsPlayable(player))
                    return true;
            }

            return false;
        }

        public bool CardsPlayable(Player player)
        {
            foreach (Card card in player.Hand.Cards)
            {
                if (SumOfPlayedCards + card.Value <= 31)
                    return true;
            }

            return false;
        }

    }
}
