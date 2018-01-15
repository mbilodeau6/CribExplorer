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
using System.Windows.Shapes;

namespace CribExplorerGui
{
    /// <summary>
    /// Interaction logic for EndOfRound.xaml
    /// </summary>
    public partial class EndOfRound : Window
    {
        private IList<RoundScore> roundScores;

        public EndOfRound(IList<RoundScore> roundScores)
        {
            InitializeComponent();
            this.roundScores = roundScores;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LabelPlayer0.Content = roundScores[0].PlayerName;
            LabelPlayer1.Content = roundScores[1].PlayerName;
            textBoxPlayer0HandScore.Text = roundScores[0].HandScore.ToString();
            textBoxPlayer1HandScore.Text = roundScores[1].HandScore.ToString();
            textBoxPlayer0CribScore.Text = roundScores[0].CribScore.ToString();
            textBoxPlayer1CribScore.Text = roundScores[1].CribScore.ToString();

            int roundTotalPlayer0 = roundScores[0].HandScore + roundScores[0].CribScore;
            int roundTotalPlayer1 = roundScores[1].HandScore + roundScores[1].CribScore;

            textBoxPlayer0RoundTotal.Text = roundTotalPlayer0.ToString();
            textBoxPlayer1RoundTotal.Text = roundTotalPlayer1.ToString();

            int newScorePlayer0 = roundScores[0].PreviousScore + roundTotalPlayer0;
            int newScorePlayer1 = roundScores[1].PreviousScore + roundTotalPlayer1;

            textBoxPlayer0GameTotal.Text = newScorePlayer0.ToString();
            textBoxPlayer1GameTotal.Text = newScorePlayer1.ToString();

            if (newScorePlayer0 == newScorePlayer1)
                LabelRoundMessage.Content = "Players are tied.";
            else
            {
                if (newScorePlayer0 > newScorePlayer1)
                {
                    if (roundScores[0].PreviousScore > roundScores[1].PreviousScore)
                        LabelRoundMessage.Content = string.Format("{0} has maintained their lead.", roundScores[0].PlayerName);
                    else
                        LabelRoundMessage.Content = string.Format("{0} has taken the lead!", roundScores[0].PlayerName);
                }
                else
                {
                    if (roundScores[1].PreviousScore > roundScores[0].PreviousScore)
                        LabelRoundMessage.Content = string.Format("{0} has maintained their lead.", roundScores[1].PlayerName);
                    else
                        LabelRoundMessage.Content = string.Format("{0} has taken the lead!", roundScores[1].PlayerName);
                }
            }
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
