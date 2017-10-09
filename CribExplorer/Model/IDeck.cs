using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer.Model
{
    interface IDeck
    {
        Card GetNextCard();
        void Shuffle();
    }
}
