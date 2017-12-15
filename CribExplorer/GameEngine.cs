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
                if (state.PlayCount == 31 || AllCardsPlayed() || NoCardsPlayable())
                    return (state.Stage = GameEngine.GameStage.EndPlay);
                else
                    return GameEngine.GameStage.NewPlay;
            }

            if (state.Stage == GameEngine.GameStage.EndPlay)
            {
                if (AllCardsPlayed())
                    return (state.Stage = GameEngine.GameStage.EndRound);
                else
                    return (state.Stage = GameEngine.GameStage.NewPlay);
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

        private bool NoCardsPlayable()
        {
            foreach(Player player in state.Players)
            {
                foreach(Card card in player.Hand.Cards)
                {
                    if (state.PlayCount + card.Value <= 31)
                        return false;
                }
            }

            return true;
        }

        public bool PlayCard(int playerId, Card card)
        {
            if (state.PlayCount + card.Value > 31)
                return false;

            state.Players[playerId].Discard(card);
            state.PlayCount += card.Value;

            return true;

        }

        private int GetWinningPlayer()
        {
            for (int i = 0; i < state.Players.Count; i++ )
                if (state.Players[i].Score >= WinningScore)
                    return i;

            // No players have won yet
            return -1;
        }

        private bool AllCardsPlayed()
        {
            int count = 0;

            foreach (Player player in state.Players)
                count += player.Discards.Cards.Count;

            return count >= GetMaxTotalHandCount();
        }
    }
}
