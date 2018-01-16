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
    /// Interaction logic for GetPlayerName.xaml
    /// </summary>
    public partial class GetPlayerName : Window
    {
        private string name = string.Empty;

        public GetPlayerName()
        {
            InitializeComponent();
            textBoxName.Text = name;
        }

        public string GetName()
        {
            return name;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text))
            {
                MessageBox.Show("Please provide your name.");
                return;
            }

            if (string.Equals("computer", textBoxName.Text, StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Please provide a different name. \"Computer is used for the system\".");
                return;
            }

            name = textBoxName.Text.Trim();
            DialogResult = true;
        }
    }
}
