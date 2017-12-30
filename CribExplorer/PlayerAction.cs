using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer
{
    // TODO: Rename to PlayerAction once original PlayerAction class removed.
    public enum PlayerAction2
    {
        Deal,
        CreateCrib,
        PlayOrPass,
        ScoreHands,
        ScoreCrib,
        DeclareWinner,
        // TODO: Will remove reamining after refactoring
        NewGame,
        NewRound,
        StartRound,
        NewPlay,
        EndPlay,
        EndRound,
        EndGame
    }

    public class PlayerAction
    {

        public enum ActionType
        {
            Deal,
            CreateCrib,
            PlayOrPass,
            ScoreHands,
            ScoreCrib,
            DeclareWinner,
            SelectCardForCrib,
            PlayCard,
            PlayerMustPass,
            CalculateScore,
            CalculateCribScore,
            NoAction
        }

        public PlayerAction()
        {
            Action = ActionType.NoAction;
            Players = new List<string>();
        }

        public ActionType Action
        {
            get;
            set;
        }

        public IList<string> Players
        {
            get;
            set;
        }
    }
}
