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
        private const int WinningScore = 121;

        public GameEngine(GameState state)
        {
            this.state = state;
        }

        public GameStage GetNextStage()
        {
            if (state.Stage == GameEngine.GameStage.NewGame && state.PlayerTurn < 0)
                return state.Stage;

            if (state.Stage == GameEngine.GameStage.NewGame && state.PlayerTurn >= 0)
                return (state.Stage = GameEngine.GameStage.NewRound);

            if (state.Stage == GameEngine.GameStage.NewRound && state.Players[0].Hand.Cards.Count > 0)
                return (state.Stage = GameEngine.GameStage.CreateCrib);

            if (state.Stage == GameEngine.GameStage.CreateCrib && state.Crib.Count > 0)
                return (state.Stage = GameEngine.GameStage.StartRound);

            if (state.Stage == GameEngine.GameStage.StartRound && state.Starter != null)
                return (state.Stage = GameEngine.GameStage.NewSubRound);

            // TODO: Add conditions for when we move to EndSubRound
            if (state.Stage == GameEngine.GameStage.NewSubRound)
                return (state.Stage = GameEngine.GameStage.EndSubRound);

            if (state.Stage == GameEngine.GameStage.EndSubRound)
            {
                if (GetDiscardCount() >= 4 * state.Players.Count)
                    return (state.Stage = GameEngine.GameStage.EndRound);
                else
                    return (state.Stage = GameEngine.GameStage.NewSubRound);
            }

            if (state.Stage == GameEngine.GameStage.EndRound)
            {
                if (GetWinningPlayer() >= 0)
                    return (state.Stage = GameEngine.GameStage.EndGame);
                else
                    return (state.Stage = GameEngine.GameStage.NewRound);
            }

            throw new ApplicationException("Invalid state.");
        }

        private int GetWinningPlayer()
        {
            for (int i = 0; i < state.Players.Count; i++ )
                if (state.Players[i].Score >= WinningScore)
                    return i;

            // No players have won yet
            return -1;
        }

        private int GetDiscardCount()
        {
            int count = 0;

            foreach (Player player in state.Players)
                count += player.Discards.Cards.Count;

            return count;
        }
    }
}
