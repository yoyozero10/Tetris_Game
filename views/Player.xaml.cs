using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MySql.Data.MySqlClient;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NAudio.Wave;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Tetris_Game
{
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>
    public partial class Player : Window
    {
        private InGame inGame;
        private Register register;
        private readonly DatabaseManager dbManager;
        private readonly AudioManager clickedAudio;
        public Player(InGame inGame)
        {
            InitializeComponent();
            clickedAudio = new AudioManager("resources/audio/ClickedAudio.wav");
            dbManager = new DatabaseManager();
            this.inGame = inGame;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            clickedAudio.Play();
            
            try
            {
                string query = @"SELECT Gender FROM InfoPlayer WHERE UserName = @UserName AND PassWord = @PassWord";

                var result = dbManager.ExecuteQuery(query,
                    new MySqlParameter("@UserName", NamePlayer.Text),
                    new MySqlParameter("@PassWord", PassWord.Password));

                if (result.Rows.Count > 0 && NamePlayer.Text != "" && PassWord.Password != "")
                {
                    string gender = result.Rows[0]["Gender"].ToString();
                    string insertHistoryQuery = @"
                        INSERT INTO History (UserName, Date, LoginTime, TimesPlayed)
                        SELECT @UserName, CURRENT_DATE(), @LoginTime, t.NewTimes
                        FROM (
                            SELECT COUNT(*) + 1 AS NewTimes
                            FROM History
                            WHERE UserName = @UserName
                        ) t";

                    dbManager.ExecuteNonQuery(insertHistoryQuery,
                        new MySqlParameter("@UserName", NamePlayer.Text),
                        new MySqlParameter("@LoginTime", DateTime.Now.ToString("HH:mm:ss")));


                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Player.Text = NamePlayer.Text;
                    mainWindow.logginTime = DateTime.Now.ToString("HH:mm:ss");

                    if (gender == "Male")
                    {
                        mainWindow.StatusPlayer.Source = new BitmapImage(new Uri("pack://application:,,,/resources/assets/Status_Male.png"));
                        mainWindow.Player.Margin = new Thickness(80, 252, 0, 0);
                        mainWindow.GoalScore.Margin = new Thickness(124, 296, 0, 0);
                        mainWindow.OldLevel.Margin = new Thickness(128, 318, 0, 0);
                        mainWindow.Score.Margin = new Thickness(131, 354, 0, 0);
                        mainWindow.NewLevel.Margin = new Thickness(129, 375, 0, 0);
                        mainWindow.GoalTime.Margin = new Thickness(124, 410.5, 0, 0);
                        mainWindow.NewTime.Margin = new Thickness(124, 431.5, 0, 0);
                    }
                    else
                    {
                        mainWindow.StatusPlayer.Source = new BitmapImage(new Uri("pack://application:,,,/resources/assets/Status_Female.png"));
                        mainWindow.Player.Margin = new Thickness(81, 245, 0, 0);
                        mainWindow.GoalScore.Margin = new Thickness(125, 290, 0, 0);
                        mainWindow.OldLevel.Margin = new Thickness(130, 311, 0, 0);
                        mainWindow.Score.Margin = new Thickness(131, 345.5, 0, 0);
                        mainWindow.NewLevel.Margin = new Thickness(130, 367, 0, 0);
                        mainWindow.GoalTime.Margin = new Thickness(124, 403.5, 0, 0);
                        mainWindow.NewTime.Margin = new Thickness(124, 424, 0, 0);
                    }

                    mainWindow.Show();
                    this.Close();
                    inGame.Close();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Register_MouseDown(object sender, MouseButtonEventArgs e)
        {
            register = new Register();
            register.Show();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            clickedAudio.Play();
            this.Close();
        }
    }
}
