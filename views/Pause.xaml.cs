using NAudio.Wave;
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

namespace Tetris_Game
{
    /// <summary>
    /// Interaction logic for Pause.xaml
    /// </summary>
    public partial class Pause : Window
    {
        private MainWindow mainWindow;
        public Pause(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void ButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.clickedAudio.Play();
            this.Close();
            mainWindow.PauseMenu.Visibility = Visibility.Collapsed;
            mainWindow.ContinueGame();
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow.IsMuted == true)
            {
                mainWindow.ButtonVolume_Click(mainWindow.ButtonVolume, null);
            }
            else
            {
                mainWindow.clickedAudio.Play();
            }

            mainWindow.SaveData();
            InGame inGame = new InGame();
            this.Close();
            mainWindow.Close();
            inGame.Show();
        }
    }
}
