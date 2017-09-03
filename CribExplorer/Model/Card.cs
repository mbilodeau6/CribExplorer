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
        public CardSuit Suit { get; set; }

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

        public CardFace Face { get; set; }

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
    }
}
