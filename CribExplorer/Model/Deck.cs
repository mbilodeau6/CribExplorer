using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer.Model
{
    public class Deck
    {
        private IList<Card> cards = new List<Card>(52);
        private int nextCard = 0;

        public Deck()
        {
            foreach(CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach(CardFace face in Enum.GetValues(typeof(CardFace)))
                {
                    cards.Add(new Card() { Suit = suit, Face = face });
                }
            }
        }

        public Card GetNextCard()
        {
            if (nextCard >= cards.Count)
                return null;

            return cards[nextCard++];
        }
    }
}
