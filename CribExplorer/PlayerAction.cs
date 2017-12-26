using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer
{
    public class PlayerAction
    {
        public enum ActionType
        {
            SelectCardForCrib,
            PlayCard,
            PlayerMustPass,
            CalculateScore,
            CalculateCribScore,
            Deal,
            CutDeck,
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
