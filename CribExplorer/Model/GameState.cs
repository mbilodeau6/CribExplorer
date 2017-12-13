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
            PlayerNames = playerNames;
            Reset();
        }

        public void Reset()
        {
            Stage = GameEngine.GameStage.NewGame;
            PlayerTurn = -1;
            Starter = null;
            Players = new List<Player>();
            Crib = new List<Card>();

            foreach (string playerName in PlayerNames)
            {
                Players.Add(new Player(playerName));
            }
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
            private set;
        }

        public IList<Card> Crib
        {
            get;
            private set;
        }

        public GameEngine.GameStage Stage
        {
            get;
            set;
        }
    }
}
