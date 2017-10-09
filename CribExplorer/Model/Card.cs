using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CribExplorer.Model
{
    public enum CardSuit
    {
        Diamond,
        Heart,
        Spade,
        Club
    }

    public enum CardColor
    {
        Red,
        Black
    }

    public enum CardFace
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }

    public class Card
    {
        private int hashCode;

        private int CalculateHashCode(Card card)
        {
            return (Enum.GetValues(typeof(CardFace)).Length * (int)card.Suit) + (int)card.Face;
        }

        public Card(CardSuit suit, CardFace face)
        {
            Suit = suit;
            Face = face;

            hashCode = CalculateHashCode(this);
        }

        public CardSuit Suit { get; private set; }

        public CardColor Color
        {
            get
            {
                if (Suit == CardSuit.Diamond || Suit == CardSuit.Heart)
                    return CardColor.Red;
                else
                    return CardColor.Black;
            }
        }

        public CardFace Face { get; private set; }

        public int Value
        {
            get
            {
                switch (Face)
                {
                    case CardFace.Jack:
                    case CardFace.Queen:
                    case CardFace.King:
                        return 10;
                    default:
                        return (int)Face;
                }
            }
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is Card))
                return false;
            else
                return hashCode == CalculateHashCode((Card)obj);
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }
}
