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
        static void ShowCard(Card card)
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

        static void ShowHand(GameEngine gameEngine, Hand playerHand, bool withIndexes = false)
        {
            int index = 0;

            foreach (Card card in playerHand.Cards)
            {
                if (withIndexes)
                    Console.Write("{0}-", index++);

                ShowCard(card);
            }

            Console.WriteLine("");
        }

        static void ShowPlayerHand(GameEngine game, int playerId, bool withIndexes = false)
        {
            Console.Write("{0}'s hand: ", game.GetPlayerName(playerId));
            ShowHand(game, game.GetPlayerHand(playerId), withIndexes);
        }
        
        static IList<string> GetPlayerNames()
        {
            IList<string> playerNames = new List<string>();

            Console.Write("Please enter the first player's name: ");
            playerNames.Add(Console.ReadLine());
            Console.Write("Please enter the second player's name: ");
            playerNames.Add(Console.ReadLine());
            Console.WriteLine();

            return playerNames;
        }

        static string GetCurrentPlayersName(GameEngine gameEngine)
        {
            return gameEngine.GetPlayerName(gameEngine.GetCurrentPlayers()[0]);
        }

        static void DisplayScores(GameEngine gameEngine)
        {
            Console.Write("Current Score: ");
            for (int i = 0; i < gameEngine.GetNumberOfPlayers(); i++)
                Console.Write("{0} = {1}; ", gameEngine.GetPlayerName(i), gameEngine.GetPlayerScore(i));

            Console.WriteLine();
        }

        static void PerformGameAction(GameEngine gameEngine)
        {
            bool cardSelected = false;
            int currentPlayerIndex = -1;
            Hand playerHand = null;
            int selectedIndex = -1;
            int score = 0;

            DisplayScores(gameEngine);

            switch (gameEngine.GetCurrentAction())
            {
                case PlayerAction.Deal:
                    Console.WriteLine("{0}... Press ENTER to deal cards...", GetCurrentPlayersName(gameEngine));
                    Console.ReadLine();
                    Console.WriteLine();
                    gameEngine.DealCards();
                    ShowPlayerHand(gameEngine, 0);
                    ShowPlayerHand(gameEngine, 1);

                    Console.WriteLine();
                    break;
                case PlayerAction.CreateCrib:
                    currentPlayerIndex = gameEngine.GetCurrentPlayers()[0];
                    playerHand = gameEngine.GetPlayerHand(currentPlayerIndex);

                    while (!cardSelected)
                    {
                        ShowPlayerHand(gameEngine, currentPlayerIndex, true);
                        Console.Write("{0}... Pick a card for the crib: ", GetCurrentPlayersName(gameEngine));

                        if (!int.TryParse(Console.ReadLine(), out selectedIndex) || 
                                selectedIndex < 0 || 
                                selectedIndex >= playerHand.Cards.Count)
                            Console.WriteLine("Invalid card index. Please select a number between 0 and {0}", playerHand.Cards.Count - 1);
                        else
                            cardSelected = true;

                        Console.WriteLine();
                    }

                    gameEngine.AddToCrib(currentPlayerIndex, playerHand.Cards[selectedIndex]);

                    break;
                case PlayerAction.PlayOrPass:
                    currentPlayerIndex = gameEngine.GetCurrentPlayers()[0];
                    playerHand = gameEngine.GetPlayerHand(currentPlayerIndex);

                    Console.WriteLine("Played Card Total: {0}", gameEngine.GetSumOfPlayedCards());

                    Console.Write("Starter Card:");
                    ShowCard(gameEngine.GetStarterCard());
                    Console.WriteLine();

                        ShowPlayerHand(gameEngine, currentPlayerIndex, true);
                        Console.Write("{0}... Pick a card to play or press 9 to pass: ", GetCurrentPlayersName(gameEngine));

                        if ((!int.TryParse(Console.ReadLine(), out selectedIndex) ||
                                selectedIndex < 0 ||
                                selectedIndex >= playerHand.Cards.Count) && selectedIndex != 9)
                        {
                            Console.WriteLine("Invalid card index. Please select a number between 0 and {0} or select 9 to pass", playerHand.Cards.Count - 1);
                        }
                        else
                        {
                            try
                            {
                                if (selectedIndex == 9)
                                {
                                    Console.WriteLine("Player Passed");
                                    gameEngine.PlayerPass(currentPlayerIndex);
                                }
                                else
                                {
                                    Console.Write("Played ");
                                    ShowCard(playerHand.Cards[selectedIndex]);
                                    Console.WriteLine();
                                    gameEngine.PlayCard(currentPlayerIndex, playerHand.Cards[selectedIndex]);
                                }
                            }
                            catch (Exception e) // TODO: Review. Should these be exceptions? If yes, should I create custom exceptions?
                            {
                                Console.WriteLine("ERROR: Invalid Selection. {0}", e.Message);
                            }
                        }

                    Console.WriteLine();
                    break;
                case PlayerAction.ScoreHands:
                    currentPlayerIndex = gameEngine.GetCurrentPlayers()[0];
                    playerHand = gameEngine.GetPlayerHand(currentPlayerIndex);

                    ShowPlayerHand(gameEngine, currentPlayerIndex);
                    Console.Write("Starter Card: ");
                    ShowCard(gameEngine.GetStarterCard());
                    Console.WriteLine();

                    PointCalculator handPoints = new PointCalculator(playerHand, gameEngine.GetStarterCard());
                    score = handPoints.GetAllPoints();
                    Console.WriteLine("Hand Score: {0}", score);
                    gameEngine.IsProvidedScoreCorrectForHand(currentPlayerIndex, score);

                    Console.WriteLine();
                    break;
                case PlayerAction.ScoreCrib:
                    currentPlayerIndex = gameEngine.GetCurrentPlayers()[0];
                    playerHand = gameEngine.GetCrib();

                    Console.Write("Crib: ");
                    ShowHand(gameEngine, gameEngine.GetCrib());
                    Console.Write("Starter Card: ");
                    ShowCard(gameEngine.GetStarterCard());
                    Console.WriteLine();

                    PointCalculator cribPoints = new PointCalculator(playerHand, gameEngine.GetStarterCard());

                    score = cribPoints.GetAllPoints();
                    Console.WriteLine("Crib Score: {0}", score);
                    gameEngine.IsProvidedScoreCorrectForCrib(score);

                    Console.WriteLine();
                    break;
                case PlayerAction.DeclareWinner:
                        Console.WriteLine("??? is the WINNER!");
                    break;
                default:
                    throw new ApplicationException("Unexpected PlayerAction");
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("NEW GAME");
            GameEngine game = new GameEngine(new Deck(), GetPlayerNames());
            Console.WriteLine("{0} won the cut.", game.GetPlayerName(game.GetCurrentPlayers()[0]));

            while(true)
            {
                PerformGameAction(game);
            }

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }
    }
}
