using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer.Model
{
    public class Player
    {
        public string Name
        {
            get;
            set;
        }

        public Hand Hand
        {
            get;
            set;
        }

        public int Score
        {
            get;
            set;
        }
    }
}
