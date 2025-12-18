using System.Windows;

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
