using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Data;
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
using static System.Formats.Asn1.AsnWriter;

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

                if (dataTable.Rows.Count > 0) leaderboard.NamePlayerTop1.Text = dataTable.Rows[0]["UserName"].ToString();
                if (dataTable.Rows.Count > 0) leaderboard.ScoreTop1.Text = dataTable.Rows[0]["BestScore"].ToString();
                if (dataTable.Rows.Count > 1) leaderboard.NamePlayerTop2.Text = dataTable.Rows[1]["UserName"].ToString();
                if (dataTable.Rows.Count > 1) leaderboard.ScoreTop2.Text = dataTable.Rows[1]["BestScore"].ToString();
                if (dataTable.Rows.Count > 2) leaderboard.NamePlayerTop3.Text = dataTable.Rows[2]["UserName"].ToString();
                if (dataTable.Rows.Count > 2) leaderboard.ScoreTop3.Text = dataTable.Rows[2]["BestScore"].ToString();
                if (dataTable.Rows.Count > 3) leaderboard.NamePlayerTop4.Text = dataTable.Rows[3]["UserName"].ToString();
                if (dataTable.Rows.Count > 3) leaderboard.ScoreTop4.Text = dataTable.Rows[3]["BestScore"].ToString();
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

