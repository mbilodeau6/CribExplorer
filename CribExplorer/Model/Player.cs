using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer.Model
{
    public class Player
    {
        public Player(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            Name = name;
            Hand = new Hand();
            Discards = new Hand();
            Score = 0;
        }

        public string Name
        {
            get;
            private set;
        }

        public Hand Hand
        {
            get;
            private set;
        }

        public Hand Discards
        {
            get;
            private set;
        }

        public int Score
        {
            get;
            set;
        }

        public void Discard(int index)
        {
            if (index < 0 || index >= Hand.Cards.Count)
                throw new IndexOutOfRangeException("Invalid card index for player's hand.");

            Discards.Cards.Add(Hand.Cards[index]);
            Hand.Cards.RemoveAt(index);
        }
    }
}
