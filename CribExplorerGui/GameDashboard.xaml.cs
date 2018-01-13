using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
using CribExplorer;

namespace CribExplorerGui
{
    public delegate void CardPlayedReaction(Card selectedCard);

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GameEngine gameEngine;

        private void DisplayCards(StackPanel panel, Hand hand)
        {
            CardPlayedReaction desiredReaction = null;

            if (gameEngine.GetCurrentAction() == PlayerAction.PlayOrPass)
                desiredReaction = PlaySelectedCard;
            if (gameEngine.GetCurrentAction() == PlayerAction.CreateCrib)
                desiredReaction = AddSelectedCardToCrib;

            foreach(Card card in hand.Cards)
            {
                panel.Children.Add(new CardControl(card, desiredReaction));
            }
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        public void UpdateDashboard()
        {
            stackPanelComputerCards.Children.Clear();
            DisplayCards(stackPanelComputerCards, gameEngine.GetPlayerHand(0));
            stackPanelComputerPlayedCards.Children.Clear();
            DisplayCards(stackPanelComputerPlayedCards, gameEngine.GetPlayerDiscards(0));
            stackPanelPlayerCards.Children.Clear();
            DisplayCards(stackPanelPlayerCards, gameEngine.GetPlayerHand(1));
            stackPanelCribCards.Children.Clear();
            DisplayCards(stackPanelCribCards, gameEngine.GetCrib());
            stackPanelPlayedCards.Children.Clear();
            DisplayCards(stackPanelPlayedCards, gameEngine.GetPlayerDiscards(1));
            stackPanelStarterCard.Children.Clear();
            if (gameEngine.GetStarterCard() != null && gameEngine.GetCurrentAction() != PlayerAction.CreateCrib)
                stackPanelStarterCard.Children.Add(new CardControl(gameEngine.GetStarterCard(), null));

            textBoxDealer.Text = gameEngine.GetPlayerName(gameEngine.GetDealer());

            IList<int> currentPlayers = gameEngine.GetCurrentPlayers();
            
            // TODO: Bug here... State should change as soon as crib is full (i.e. 
            // shouldn't have to call GetCurrentAction().
            if (currentPlayers.Count == 0)
            {
                gameEngine.GetCurrentAction();
                UpdateDashboard();
                return;
            }

            textBoxPlayersTurn.Text = gameEngine.GetPlayerName(currentPlayers[0]);
            textBoxSumPlayed.Text = gameEngine.GetSumOfPlayedCards().ToString();
            textBoxComputersScore.Text = gameEngine.GetPlayerScore(0).ToString();
            textBoxPlayersScore.Text = gameEngine.GetPlayerScore(1).ToString();

            buttonDeal.IsEnabled = gameEngine.GetCurrentPlayers()[0] != 0 && gameEngine.GetCurrentAction() == PlayerAction.Deal;
            buttonPass.IsEnabled = gameEngine.GetCurrentPlayers()[0] != 0 && gameEngine.GetCurrentAction() == PlayerAction.PlayOrPass;

            textBoxMessage.Text = string.Format("Waiting for {0} to {1}", gameEngine.GetPlayerName(gameEngine.GetCurrentPlayers()[0]), gameEngine.GetCurrentAction().ToString());
        }

        // TODO: Move this to the AI module
        private Card SelectCardForCrib()
        {
            Hand hand = gameEngine.GetPlayerHand(0);

            if (hand.Cards.Count > 0)
                return hand.Cards[0];

            throw new ApplicationException("Unexpected Error: The computer was asked to add a card to the crib but the computer doesn't have any more cards.");
        }

        // TODO: Move this to the AI module
        private Card SelectCardForPlay()
        {
            foreach(Card card in gameEngine.GetPlayerHand(0).Cards)
            {
                if (gameEngine.GetSumOfPlayedCards() + card.Value <= 31)
                    return card;
            }

            // No playable cards found so indicate that computer should pass
            return null;
        }

        public async Task DoNextAction()
        {
            int currentPlayer = gameEngine.GetCurrentPlayers()[0];

            PointCalculator pointCalculator;

            switch(gameEngine.GetCurrentAction())
            {
                case PlayerAction.ScoreHands:
                    // TODO: If human, allow them to provide score
                    pointCalculator = new PointCalculator(gameEngine.GetPlayerHand(currentPlayer), gameEngine.GetStarterCard());
                    gameEngine.IsProvidedScoreCorrectForHand(currentPlayer, pointCalculator.GetAllPoints());
                    return;
                case PlayerAction.ScoreCrib:
                    // TODO: Provide correct score.
                    pointCalculator = new PointCalculator(gameEngine.GetCrib(), gameEngine.GetStarterCard());
                    gameEngine.IsProvidedScoreCorrectForCrib(pointCalculator.GetAllPoints());
                    return;
                case PlayerAction.DeclareWinner:
                    MessageBox.Show(string.Format("Player {0} won!", gameEngine.GetWinningPlayer()));
                    return;
            }

            if (currentPlayer == 0)
            {
                Thread.Sleep(500);

                switch(gameEngine.GetCurrentAction())
                {
                    case PlayerAction.Deal:
                        gameEngine.DealCards();
                        break;
                    case PlayerAction.CreateCrib:
                        gameEngine.AddToCrib(0, SelectCardForCrib());
                        break;
                    case PlayerAction.PlayOrPass:
                        Card selectedCard = SelectCardForPlay();

                        if (selectedCard == null)
                            gameEngine.PlayerPass(0);
                        else
                            gameEngine.PlayCard(0, selectedCard);

                        break;
                    default:
                        textBoxMessage.Text = "Computer doesn't know what to do... yet";
                        break;
                }
            }
        }

        public async Task GameLoop()
        {
            while(gameEngine.GetCurrentPlayers()[0] == 0 || gameEngine.GetCurrentAction() == PlayerAction.ScoreHands || gameEngine.GetCurrentAction() == PlayerAction.ScoreCrib)
            {
                await this.Dispatcher.InvokeAsync<Task>(DoNextAction);
                this.UpdateDashboard();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.gameEngine = new GameEngine(new Deck(), new List<string>() {"Computer", "Human"});
            UpdateDashboard();
        }

        private void buttonDeal_Click(object sender, RoutedEventArgs e)
        {
            if (gameEngine.GetCurrentAction() != PlayerAction.Deal)
                return;

            gameEngine.DealCards();

            UpdateDashboard();
            this.Dispatcher.InvokeAsync<Task>(GameLoop);
        }

        private void AddSelectedCardToCrib(Card selectedCard)
        {
            gameEngine.AddToCrib(1, selectedCard);
            UpdateDashboard();
            this.Dispatcher.InvokeAsync<Task>(GameLoop);
        }

        private void PlaySelectedCard(Card selectedCard)
        {
            gameEngine.PlayCard(1, selectedCard);
            UpdateDashboard();
            this.Dispatcher.InvokeAsync<Task>(GameLoop);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.Dispatcher.InvokeAsync<Task>(GameLoop);
        }

    }
}
