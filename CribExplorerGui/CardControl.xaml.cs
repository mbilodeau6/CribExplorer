using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CribExplorer.Model;

namespace CribExplorerGui
{
    /// <summary>
    /// Interaction logic for CardControl.xaml
    /// </summary>
    public partial class CardControl : UserControl
    {
        public Card Card
        {
            get;
            private set;
        }

        public bool Hidden
        {
            get;
            set;
        }

        private CardPlayedReaction reaction;

        public CardControl(Card card, CardPlayedReaction reaction, bool hidden = false)
        {
            InitializeComponent();
            this.Hidden = hidden;
            this.Card = card;
            this.reaction = reaction;

            string faceValue = "";

            switch (card.Face)
            {
                case CardFace.Ace:
                    faceValue = " A";
                    break;
                case CardFace.Two:
                    faceValue = " 2";
                    break;
                case CardFace.Three:
                    faceValue = " 3";
                    break;
                case CardFace.Four:
                    faceValue = " 4";
                    break;
                case CardFace.Five:
                    faceValue = " 5";
                    break;
                case CardFace.Six:
                    faceValue = " 6";
                    break;
                case CardFace.Seven:
                    faceValue = " 7";
                    break;
                case CardFace.Eight:
                    faceValue = " 8";
                    break;
                case CardFace.Nine:
                    faceValue = " 9";
                    break;
                case CardFace.Ten:
                    faceValue = "10";
                    break;
                case CardFace.Jack:
                    faceValue = " J";
                    break;
                case CardFace.Queen:
                    faceValue = " Q";
                    break;
                case CardFace.King:
                    faceValue = " K";
                    break;
            }

            cardFace.Content = faceValue;

            switch (Card.Color)
            {
                case CardColor.Black:
                    cardFace.Foreground = new SolidColorBrush(Colors.Black);
                    break;
                case CardColor.Red:
                    cardFace.Foreground = new SolidColorBrush(Colors.Red);
                    break;
            }

            // TODO: Need to remove hardcoded image paths
            switch (Card.Suit)
            {
                case CardSuit.Club:
                    cardSuit.Source = new BitmapImage(new Uri(@"c:\src\CribExplorer\CribExplorerGui\Resources\Club.jpg"));
                    break;
                case CardSuit.Heart:
                    cardSuit.Source = new BitmapImage(new Uri(@"c:\src\CribExplorer\CribExplorerGui\Resources\Heart.jpg"));
                    break;
                case CardSuit.Spade:
                    cardSuit.Source = new BitmapImage(new Uri(@"c:\src\CribExplorer\CribExplorerGui\Resources\Spade.jpg"));
                    break;
                case CardSuit.Diamond:
                    cardSuit.Source = new BitmapImage(new Uri(@"c:\src\CribExplorer\CribExplorerGui\Resources\Diamond.jpg"));
                    break;
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            reaction(this.Card);
        }
    }
}
