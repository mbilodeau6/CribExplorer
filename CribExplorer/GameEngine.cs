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
        public enum GameStage
        {
            NewGame,
            NewRound,
            CreateCrib,
            StartRound,
            NewSubRound,
            EndSubRound,
            EndRound,
            EndGame
        }

        private GameState state;

        public GameEngine(GameState state)
        {
            this.state = state;
        }

        public GameStage GetNextStage()
        {
            if (state.PlayerTurn < 0 && state.Starter == null)
                return GameEngine.GameStage.NewGame;

            if (state.PlayerTurn >= 0 && state.Starter == null)
                return GameEngine.GameStage.NewRound;

            throw new ApplicationException("Invalid state.");
        }
    }
}
