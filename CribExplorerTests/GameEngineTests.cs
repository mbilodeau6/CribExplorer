using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CribExplorer;
using CribExplorer.Model;

namespace CribExplorerTests
{
    [TestClass]
    public class GameEngineTests
    {
        [TestMethod]
        public void GameEngine_GetNextStage_Initial()
        {
            GameEngine game = new GameEngine(new GameState(2) );

            Assert.AreEqual(GameEngine.GameStage.NewGame, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_NewRound()
        {
            GameState state = new GameState(2)
            {
                PlayerTurn = 0
            };

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.NewRound, game.GetNextStage());
        }

        [TestMethod]
        public void GameEngine_GetNextStage_Reset()
        {
            GameState state = new GameState(2)
            {
                PlayerTurn = 0
            };

            state.Reset(2);

            GameEngine game = new GameEngine(state);

            Assert.AreEqual(GameEngine.GameStage.NewGame, game.GetNextStage());
        }
    }
}
