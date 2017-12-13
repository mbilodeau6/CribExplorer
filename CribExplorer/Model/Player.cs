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

        public void Discard(Card card)
        {
            if (!Hand.Cards.Contains(card))
                throw new ArgumentException("Requested card does not exist in the players hand.");

            Discards.Cards.Add(card);
            Hand.Cards.Remove(card);
        }
    }
}
