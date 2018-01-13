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

        public override string ToString()
        {
            string faceValue;
            string suitValue;

            switch (Face)
            {
                case CardFace.Ace:
                    faceValue = "A";
                    break;
                case CardFace.Two:
                    faceValue = "2";
                    break;
                case CardFace.Three:
                    faceValue = "3";
                    break;
                case CardFace.Four:
                    faceValue = "4";
                    break;
                case CardFace.Five:
                    faceValue = "5";
                    break;
                case CardFace.Six:
                    faceValue = "6";
                    break;
                case CardFace.Seven:
                    faceValue = "7";
                    break;
                case CardFace.Eight:
                    faceValue = "8";
                    break;
                case CardFace.Nine:
                    faceValue = "9";
                    break;
                case CardFace.Ten:
                    faceValue = "T";
                    break;
                case CardFace.Jack:
                    faceValue = "J";
                    break;
                case CardFace.Queen:
                    faceValue = "Q";
                    break;
                case CardFace.King:
                    faceValue = "K";
                    break;
                default:
                    throw new Exception("Unrecognized card face");
            }

            switch (Suit)
            {
                case CardSuit.Club:
                    suitValue = "C";
                    break;
                case CardSuit.Spade:
                    suitValue = "S";
                    break;
                case CardSuit.Diamond:
                    suitValue = "D";
                    break;
                case CardSuit.Heart:
                    suitValue = "H";
                    break;
                default:
                    throw new Exception("Unexpected card suit");
            }

            return string.Format("{0}{1}", faceValue, suitValue);
        }
    }
}
