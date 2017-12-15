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
            NewPlay,
            EndPlay,
            EndRound,
            EndGame
        }

        private GameState state;

        public const int WinningScore = 121;
        public const int RequiredHandCardCount = 4;
        
        public GameEngine(GameState state)
        {
            this.state = state;
        }

        public int GetMaxTotalHandCount()
        {
            return RequiredHandCardCount * state.Players.Count;
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
                return (state.Stage = GameEngine.GameStage.NewPlay);

            if (state.Stage == GameEngine.GameStage.NewPlay)
            {
                if (state.PlayCount == 31 || state.AllCardsPlayed() || state.NoCardsPlayable())
                    return (state.Stage = GameEngine.GameStage.EndPlay);
                else
                    return GameEngine.GameStage.NewPlay;
            }

            if (state.Stage == GameEngine.GameStage.EndPlay)
            {
                if (state.AllCardsPlayed())
                    return (state.Stage = GameEngine.GameStage.EndRound);
                else
                    return (state.Stage = GameEngine.GameStage.NewPlay);
            }

            if (state.Stage == GameEngine.GameStage.EndRound)
            {
                if (state.GetWinningPlayer() >= 0)
                    return (state.Stage = GameEngine.GameStage.EndGame);
                else
                    return (state.Stage = GameEngine.GameStage.NewRound);
            }

            throw new ApplicationException("Invalid state.");
        }

        public bool PlayCard(int playerId, Card card)
        {
            if (state.PlayCount + card.Value > 31)
                return false;

            state.Players[playerId].Discard(card);
            state.PlayCount += card.Value;

            return true;

        }
    }
}
