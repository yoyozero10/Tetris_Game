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
    /// Interaction logic for Leaderboard.xaml
    /// </summary>
    public partial class Leaderboard : Window
    {
        private readonly AudioManager clickedAudio;
        public Leaderboard()
        {
            InitializeComponent();
            clickedAudio = new AudioManager("resources/audio/ClickedAudio.wav");
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            clickedAudio.Play();
            this.Close();
        }
    }
}
