using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer.Model
{
    public class Deck : IDeck
    {
        private IList<Card> cards = new List<Card>(52);
        private int nextCard = 0;

        // TODO: Need to set seed and probably should be shared with larger app
        private static Random rand = new Random(DateTime.Now.Second);

        public Deck()
        {
            foreach(CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach(CardFace face in Enum.GetValues(typeof(CardFace)))
                {
                    cards.Add(new Card(suit, face));
                }
            }
        }

        public Card GetNextCard()
        {
            if (nextCard >= cards.Count)
                return null;

            return cards[nextCard++];
        }

        // Knuth Shuffle Algorithm from https://tekpool.wordpress.com/2006/10/06/shuffling-shuffle-a-deck-of-cards-knuth-shuffle/
        public void Shuffle()
        {
            int j;
            Card temp;

            for (int i = cards.Count - 1; i >= 0; i--)
            {
                j = rand.Next(0, i);

                temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }

            nextCard = 0;
        }
    }
}
