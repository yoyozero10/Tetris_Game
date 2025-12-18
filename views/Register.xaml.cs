using MySql.Data.MySqlClient;
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
using Tetris_Game;

namespace Tetris_Game
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private readonly DatabaseManager dbManager;
        private readonly AudioManager clickedAudio;
        public Register()
        {
            InitializeComponent();
            clickedAudio = new AudioManager("resources/audio/ClickedAudio.wav");
            dbManager = new DatabaseManager();
        }
        private void Male_Checked(object sender, RoutedEventArgs e)
        {
            Female.IsChecked = false;
        }

        private void Female_Checked(object sender, RoutedEventArgs e)
        {
            Male.IsChecked = false;
        }
        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            clickedAudio.Play();
            if (PassWord1.Password != PassWord2.Password)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            try
            {
                var exists = dbManager.ExecuteQuery(
                    "SELECT 1 FROM InfoPlayer WHERE UserName = @UserName LIMIT 1",
                    new MySqlParameter("@UserName", UserName.Text)
                ).Rows.Count > 0;

                if (exists)
                {
                    MessageBox.Show("Username already exists. Please try another username.");
                    return;
                }

                dbManager.ExecuteNonQuery(
                    "INSERT INTO InfoPlayer (UserName, PassWord, Gender) VALUES (@UserName, @PassWord, @Gender)",
                    new MySqlParameter("@UserName", UserName.Text),
                    new MySqlParameter("@PassWord", PassWord1.Password),
                    new MySqlParameter("@Gender", Male.IsChecked == true ? "Male" : "Female")
                );

                MessageBox.Show("Registration successful.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            clickedAudio.Play();
            this.Close();
        }
    }
}


