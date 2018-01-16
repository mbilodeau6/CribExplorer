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
        private string lastAction;
        private string playerName;

        private void DisplayCards(StackPanel panel, Hand hand, bool hidden = false)
        {
            CardPlayedReaction desiredReaction = null;

            if (gameEngine.GetCurrentAction() == PlayerAction.PlayOrPass)
                desiredReaction = PlaySelectedCard;
            if (gameEngine.GetCurrentAction() == PlayerAction.CreateCrib)
                desiredReaction = AddSelectedCardToCrib;

            foreach(Card card in hand.Cards)
            {
                panel.Children.Add(new CardControl(card, desiredReaction, hidden));
            }
        }

        public MainWindow()
        {
            GetPlayerName getPlayerNameForm = new GetPlayerName();
            if (!(getPlayerNameForm.ShowDialog() ?? false))
                this.Close();

            playerName = getPlayerNameForm.GetName();

            InitializeComponent();
        }

        public void UpdateDashboard()
        {
            PlayerAction currentAction = gameEngine.GetCurrentAction();

            bool showAllCards = ((currentAction == PlayerAction.ScoreHands) || (currentAction == PlayerAction.ScoreCrib) || (currentAction == PlayerAction.DeclareWinner));

            stackPanelComputerCards.Children.Clear();
            DisplayCards(stackPanelComputerCards, gameEngine.GetPlayerHand(0), !showAllCards);
            stackPanelComputerPlayedCards.Children.Clear();
            DisplayCards(stackPanelComputerPlayedCards, gameEngine.GetPlayerDiscards(0));
            stackPanelPlayerCards.Children.Clear();
            DisplayCards(stackPanelPlayerCards, gameEngine.GetPlayerHand(1));
            stackPanelCribCards.Children.Clear();
            DisplayCards(stackPanelCribCards, gameEngine.GetCrib(), !showAllCards);
            stackPanelPlayedCards.Children.Clear();
            DisplayCards(stackPanelPlayedCards, gameEngine.GetPlayerDiscards(1));
            stackPanelStarterCard.Children.Clear();
            if (gameEngine.GetStarterCard() != null && currentAction != PlayerAction.CreateCrib)
                stackPanelStarterCard.Children.Add(new CardControl(gameEngine.GetStarterCard(), null));

            textBoxDealer.Text = gameEngine.GetPlayerName(gameEngine.GetDealer());

            IList<int> currentPlayers = gameEngine.GetCurrentPlayers();
            
            textBoxPlayersTurn.Text = gameEngine.GetPlayerName(currentPlayers[0]);
            textBoxSumPlayed.Text = gameEngine.GetSumOfPlayedCards().ToString();
            textBoxComputersScore.Text = gameEngine.GetPlayerScore(0).ToString();
            textBoxPlayersScore.Text = gameEngine.GetPlayerScore(1).ToString();

            buttonDeal.IsEnabled = gameEngine.GetCurrentPlayers()[0] != 0 && currentAction == PlayerAction.Deal;
            buttonPass.IsEnabled = gameEngine.GetCurrentPlayers()[0] != 0 && currentAction == PlayerAction.PlayOrPass;

            textBoxMessage.AppendText(lastAction);
            lastAction = string.Empty;
            textBoxMessage.AppendText(string.Format("\nWaiting for {0} to {1}... ", gameEngine.GetPlayerName(gameEngine.GetCurrentPlayers()[0]), currentAction.ToString()));
            textBoxMessage.ScrollToEnd();
        }

        // TODO: Move this to the AI module
        private Card SelectCardForCrib()
        {
            Hand hand = gameEngine.GetPlayerHand(0);

            if (hand.Cards.Count > 0)
            {
                Card selectedCard = hand.Cards[0];
                this.lastAction = string.Format("Computer added {0} to crib. ", selectedCard.ToString());
                return selectedCard;
            }

            throw new ApplicationException("Unexpected Error: The computer was asked to add a card to the crib but the computer doesn't have any more cards.");
        }

        // TODO: Move this to the AI module
        private Card SelectCardForPlay()
        {
            foreach(Card card in gameEngine.GetPlayerHand(0).Cards)
            {
                if (gameEngine.GetSumOfPlayedCards() + card.Value <= 31)
                {
                    this.lastAction = string.Format("Computer played {0}. ", card.ToString());
                    return card;
                }
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
                    pointCalculator = new HandPointCalculator(gameEngine.GetPlayerHand(currentPlayer), gameEngine.GetStarterCard());
                    gameEngine.IsProvidedScoreCorrectForHand(currentPlayer, pointCalculator.GetAllPoints());
                    return;
                case PlayerAction.ScoreCrib:
                    // TODO: If human, allow them to provide score
                    pointCalculator = new HandPointCalculator(gameEngine.GetCrib(), gameEngine.GetStarterCard());
                    gameEngine.IsProvidedScoreCorrectForCrib(pointCalculator.GetAllPoints());

                    IList<RoundScore> roundScores = new List<RoundScore>();

                    for(int i = 0; i < gameEngine.GetNumberOfPlayers(); i++)
                    {
                        // TODO: Should be able to refactor so that I can use the hand/crib 
                        // scores from earlier calculations
                        HandPointCalculator handPointCalculator = new HandPointCalculator(gameEngine.GetPlayerHand(i), gameEngine.GetStarterCard());
                        int handScore = handPointCalculator.GetAllPoints();

                        int cribScore = 0;

                        if (i == gameEngine.GetDealer())
                        {
                            HandPointCalculator cribPointCalculator = new HandPointCalculator(gameEngine.GetCrib(), gameEngine.GetStarterCard());
                            cribScore = cribPointCalculator.GetAllPoints();
                        }

                        roundScores.Add(new RoundScore(gameEngine.GetPlayerName(i), handScore, cribScore, gameEngine.GetPlayerScore(i) - handScore - cribScore));    
                    }

                    EndOfRound endOfRoundForm = new EndOfRound(roundScores);
                    endOfRoundForm.Owner = this;
                    endOfRoundForm.ShowDialog();
                    return;
                case PlayerAction.DeclareWinner:
                    string message;

                    if (gameEngine.GetWinningPlayer() == 0)
                        message = "Computer won. Better luck next time.";
                    else
                        message = string.Format("Congratulations {0}! You won!", gameEngine.GetPlayerName(gameEngine.GetWinningPlayer()));

                    DeclareWinner declareWinnerForm = new DeclareWinner(message);
                    declareWinnerForm.Owner = this;
                    declareWinnerForm.ShowDialog();

                    gameEngine.StartNextGame();
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
            this.gameEngine = new GameEngine(new Deck(), new List<string>() {"Computer", playerName});
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
            try
            {
                gameEngine.AddToCrib(1, selectedCard);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Invalid selection: {0}", e.Message));
                return;
            }

            UpdateDashboard();
            this.Dispatcher.InvokeAsync<Task>(GameLoop);
        }

        private void PlaySelectedCard(Card selectedCard)
        {
            try
            {
                gameEngine.PlayCard(1, selectedCard);
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Invalid selection: {0}", e.Message));
                return;
            }

            UpdateDashboard();
            this.Dispatcher.InvokeAsync<Task>(GameLoop);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            this.Dispatcher.InvokeAsync<Task>(GameLoop);
        }

        private void buttonPass_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gameEngine.PlayerPass(1);
            }
            catch (Exception exception)
            {
                MessageBox.Show(string.Format("Invalid selection: {0}", exception.Message));
                return;
            }

            UpdateDashboard();
            this.Dispatcher.InvokeAsync<Task>(GameLoop);
        }

    }
}
