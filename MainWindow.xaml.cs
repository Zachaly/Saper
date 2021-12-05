using System;
using System.Windows;

namespace SaperWPF
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }
        
        //Generates window with a game depending on button clicked

        private void SmallGameBtn_Click(object sender, RoutedEventArgs e)
        {
            GameWindow game = new GameWindow(8, 8, 10);
            game.Show();
        }

        private void MediuGameBtn_Click(object sender, RoutedEventArgs e)
        {
            GameWindow game = new GameWindow(16, 16, 40);
            game.Show();
        }

        private void LargeGameBtn_Click(object sender, RoutedEventArgs e)
        {
            GameWindow game = new GameWindow(30, 16, 99);
            game.Show();
        }
    }
}
