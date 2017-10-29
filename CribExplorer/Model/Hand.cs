using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer.Model
{
    public class Hand
    {
        public IList<Card> Cards;

        public Hand()
        {
            Cards = new List<Card>();
        }
    }
}
