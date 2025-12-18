using System;
using System.Data;
using System.Windows;
using System.Windows.Input;

namespace Tetris_Game
{
    /// <summary>
    /// Interaction logic for InGame.xaml
    /// </summary>
    public partial class InGame : Window
    {
        private readonly AudioManager hoverAudio;
        private readonly AudioManager clickedAudio;
        private readonly DatabaseManager dbManager;
        private Leaderboard leaderboard;

        public InGame()
        {
            InitializeComponent();

            hoverAudio = new AudioManager("resources/audio/HoverAudio.wav");
            clickedAudio = new AudioManager("resources/audio/ClickedAudio.wav");
            dbManager = new DatabaseManager();
        }

        private void LoadTopPlayers()
        {
            try
            {
                string query = @"SELECT UserName, BestScore, BestLevel FROM Goal ORDER BY BestScore DESC LIMIT 4";
                var dataTable = dbManager.ExecuteQuery(query);

                var nameControls = new[] { leaderboard.NamePlayerTop1, leaderboard.NamePlayerTop2, leaderboard.NamePlayerTop3, leaderboard.NamePlayerTop4 };
                var scoreControls = new[] { leaderboard.ScoreTop1, leaderboard.ScoreTop2, leaderboard.ScoreTop3, leaderboard.ScoreTop4 };

                for (int i = 0; i < nameControls.Length && i < dataTable.Rows.Count; i++)
                {
                    nameControls[i].Text = dataTable.Rows[i]["UserName"].ToString();
                    scoreControls[i].Text = dataTable.Rows[i]["BestScore"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading top players: " + ex.Message);
            }
        }

        private void ButtonPlay_Default_Click(object sender, RoutedEventArgs e)
        {
            clickedAudio.Play();
            var player = new Player(this);
            player.Show();
        }

        private void ButtonRank_Default_Click(object sender, RoutedEventArgs e)
        {
            clickedAudio.Play();
            leaderboard = new Leaderboard();
            LoadTopPlayers();
            leaderboard.Show();
        }

        private void ButtonQuit_Default_Click(object sender, RoutedEventArgs e)
        {
            clickedAudio.Play();
            Application.Current.Shutdown();
        }

        private void ButtonPlay_Default_MouseEnter(object sender, MouseEventArgs e) => hoverAudio.Play();

        private void ButtonRank_Default_MouseEnter(object sender, MouseEventArgs e) => hoverAudio.Play();

        private void ButtonQuit_Default_MouseEnter(object sender, MouseEventArgs e) => hoverAudio.Play();

        private void ButtonInstruction_Click(object sender, RoutedEventArgs e)
        {
            clickedAudio.Play();
            var instruction = new Instruction();
            instruction.Show();
        }
    }
}

