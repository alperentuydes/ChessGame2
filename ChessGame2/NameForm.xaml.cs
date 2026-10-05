using ChessGame2.Models;
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

namespace ChessGame2
{
    /// <summary>
    /// Interaction logic for NameForm.xaml
    /// </summary>
    public partial class NameForm : Window
    {
        public string PlayerName;

        public NameForm()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(userName.Text))
            {
                MessageBox.Show("Lütfen bir isim girin.");
                return;
            }

            string playerName = userName.Text;

            MainWindow mainWindow = new MainWindow(playerName);
            mainWindow.Show();

            this.Close();
        }
    }
}
