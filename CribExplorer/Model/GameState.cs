using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer.Model
{
    public class GameState
    {
        public void Reset(int playerCount)
        {
            PlayerTurn = -1;
            Starter = null;
            Players = new List<Player>();

            for (int i = 0; i < playerCount; i++ )
            {
                Players.Add(new Player());
                Players[i].Hand = new Hand();
            }
        }

        public GameState(int playerCount)
        {
            Reset(playerCount);
        }

        public int PlayerTurn
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
            set;
        }
    }
}
