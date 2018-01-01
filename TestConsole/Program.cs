using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CribExplorer.Model;
using CribExplorer;

namespace TestConsole
{
    class Program
    {
        static void ShowPlayerHand(GameEngine game, int playerId)
        {
            Console.Write("{0}'s hand: ", game.GetPlayerName(playerId));

            foreach(Card card in game.GetPlayerHand(playerId).Cards)
            {
                switch (card.Face)
                {
                    case CardFace.Ace:
                        Console.Write("A");
                        break;
                    case CardFace.Two:
                        Console.Write("2");
                        break;
                    case CardFace.Three:
                        Console.Write("3");
                        break;
                    case CardFace.Four:
                        Console.Write("4");
                        break;
                    case CardFace.Five:
                        Console.Write("5");
                        break;
                    case CardFace.Six:
                        Console.Write("6");
                        break;
                    case CardFace.Seven:
                        Console.Write("7");
                        break;
                    case CardFace.Eight:
                        Console.Write("8");
                        break;
                    case CardFace.Nine:
                        Console.Write("9");
                        break;
                    case CardFace.Ten:
                        Console.Write("T");
                        break;
                    case CardFace.Jack:
                        Console.Write("J");
                        break;
                    case CardFace.Queen:
                        Console.Write("Q");
                        break;
                    case CardFace.King:
                        Console.Write("K");
                        break;
                    default:
                        throw new Exception("Unrecognized card face");
                }

                switch (card.Suit)
                {
                    case CardSuit.Club:
                        Console.Write("C");
                        break;
                    case CardSuit.Spade:
                        Console.Write("S");
                        break;
                    case CardSuit.Diamond:
                        Console.Write("D");
                        break;
                    case CardSuit.Heart:
                        Console.Write("H");
                        break;
                    default:
                        throw new Exception("Unexpected card suit");
                }

                Console.Write(" ");
            }

            Console.WriteLine("");
        }

        static void Main(string[] args)
        {
            GameEngine game = new GameEngine(new Deck(), new List<string>() {"Ann", "Bill"});

            Console.WriteLine();
            Console.WriteLine("NEW GAME");

            Console.WriteLine("{0} won the cut.", game.GetPlayerName(game.GetCurrentPlayers()[0]));
            ShowPlayerHand(game, 0);
            ShowPlayerHand(game, 1);

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }
    }
}
