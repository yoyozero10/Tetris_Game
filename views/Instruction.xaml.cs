using System.Windows;

namespace Tetris_Game
{
    /// <summary>
    /// Interaction logic for Instruction.xaml
    /// </summary>
    public partial class Instruction : Window
    {
        private readonly AudioManager clickedAudio;
        public Instruction()
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
