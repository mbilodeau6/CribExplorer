using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorerGui
{
    public class RoundScore
    {
        public RoundScore(string playerName, int handScore, int cribScore, int previousScore)
        {
            PlayerName = playerName;
            HandScore = handScore;
            CribScore = cribScore;
            PreviousScore = previousScore;
        }

        public string PlayerName
        {
            get;
            private set;
        }

        public int HandScore
        {
            get;
            private set;
        }

        public int CribScore
        {
            get;
            private set;
        }

        public int PreviousScore
        {
            get;
            private set;
        }
    }
}
