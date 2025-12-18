using MySql.Data.MySqlClient;
using NAudio.Wave;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Tetris_Game
{
    public partial class MainWindow : Window
    {
        public readonly AudioManager clickedAudio;
        private GameState gameState;
        private readonly Image[,] imageControls;
        private readonly DatabaseManager dbManager;
        private DispatcherTimer timer;
        private int secondsElapsed;
        public string gender;
        public string logginTime;

        public bool IsMuted { get; private set; } = false;
        public bool IsGamePaused { get; private set; } = false;

        private readonly ImageSource[] TileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("../resources/assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/TileRed.png", UriKind.Relative))
        };

        private readonly ImageSource[] BlockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("../resources/assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("../resources/assets/Block-Z.png", UriKind.Relative)),
        };

        public MainWindow()
        {
            InitializeComponent();

            clickedAudio = new AudioManager("resources/audio/ClickedAudio.wav");
            gameState = new GameState();
            dbManager = new DatabaseManager();

            imageControls = SetUpGameCanvas(gameState.GameGrid);
            SetTime();
            LoadPlayerData();
        }

        private void SetTime()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
            }
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            secondsElapsed = 0;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!gameState.GameOver && !IsGamePaused)
            {
                secondsElapsed++;
                NewTime.Text = TimeSpan.FromSeconds(secondsElapsed).ToString(@"mm\:ss");
            }
        }

        private Image[,] SetUpGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize,
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }


        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = TileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = TileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = BlockImages[next.Id];
        }

        private void DrawHeldBlock(Block heldBlock)
        {
            if (heldBlock == null)
                HoldImage.Source = BlockImages[0];
            else
                HoldImage.Source = BlockImages[heldBlock.Id];
        }

        private void DrawGhostBlock(Block block)
        {
            int dropDistance = gameState.BlockDropDistance();

            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = TileImages[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            DrawHeldBlock(gameState.HeldBlock);
        }

        private void DisplayStatus()
        {
            Score.Text = $"{gameState.Score}";
            NewLevel.Text = $"{gameState.Level}";
        }

        private void LoadPlayerData()
        {
            try
            {
                string query = @"SELECT BestScore, BestLevel, BestTime FROM Goal WHERE UserName = @UserName";
                var dataTable = dbManager.ExecuteQuery(query, new MySqlParameter("@UserName", Player.Text));

                if (dataTable.Rows.Count > 0)
                {
                    GoalScore.Text = dataTable.Rows[0]["BestScore"].ToString();
                    OldLevel.Text = dataTable.Rows[0]["BestLevel"].ToString();
                    GoalTime.Text = dataTable.Rows[0]["BestTime"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading player data: " + ex.Message);
            }
        }


        private bool isGameLoopRunning = false;

        private async Task GameLoop()
        {
            if (isGameLoopRunning) return;
            isGameLoopRunning = true;

            Draw(gameState);
            DisplayStatus();
            gameState.backgroundAudio.Play();

            while (!gameState.GameOver)
            {
                if (gameState.backgroundAudio.audioReader.Position >= gameState.backgroundAudio.audioReader.Length)
                {
                    gameState.backgroundAudio.Reset();
                    gameState.backgroundAudio.Play();
                }

                if (IsGamePaused)
                {
                    gameState.backgroundAudio.Pause();
                    await Task.Delay(100);
                    continue;
                }

                if (gameState.backgroundAudio.GetPlaybackState() != NAudio.Wave.PlaybackState.Playing && !IsMuted)
                {
                    gameState.backgroundAudio.Continue();
                }

                int delay = Math.Max(100, 500 - (gameState.Level - 1) * 50);
                await Task.Delay(delay);

                if (!IsGamePaused && !gameState.GameOver)
                {
                    gameState.MoveBlockDown();
                    Draw(gameState);
                    DisplayStatus();
                }
            }

            isGameLoopRunning = false;
            gameState.backgroundAudio.Stop();
            gameState.gameOverAudio.Play();
            ShowGameOverMenu();
        }

        public void SaveData()
        {
            try
            {
                string historyQuery = @"
                          UPDATE History h
                          JOIN (
                              SELECT @UserName AS UserName,
                                     @LoginTime AS LoginTime,
                                     @Score AS Score,
                                     @Level AS Level,
                                     @PlayTime AS PlayTime,
                                     COUNT(*) AS NewTimesPlayed
                              FROM History
                              WHERE UserName = @UserName
                          ) t ON h.UserName = t.UserName AND h.LoginTime = t.LoginTime
                          SET
                              h.Score = t.Score,
                              h.Level = t.Level,
                              h.PlayTime = t.PlayTime,
                              h.TimesPlayed = t.NewTimesPlayed";

                dbManager.ExecuteNonQuery(historyQuery,
                    new MySqlParameter("@UserName", Player.Text),
                    new MySqlParameter("@Score", gameState.Score),
                    new MySqlParameter("@Level", gameState.Level),
                    new MySqlParameter("@PlayTime", NewTime.Text),
                    new MySqlParameter("@LoginTime", logginTime));

                string query = @"
                    INSERT INTO Goal (UserName, BestScore, BestLevel, BestTime)
                    VALUES (@UserName, @Score, @Level, @Time)
                    ON DUPLICATE KEY UPDATE
                      BestScore = GREATEST(@Score, BestScore),
                      BestLevel = GREATEST(@Level, BestLevel),
                      BestTime  = CASE WHEN @Time > BestTime THEN @Time ELSE BestTime END";

                dbManager.ExecuteNonQuery(query,
                    new MySqlParameter("@UserName", Player.Text),
                    new MySqlParameter("@Score", gameState.Score),
                    new MySqlParameter("@Level", gameState.Level),
                    new MySqlParameter("@Time", NewTime.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving player data: {ex.Message}");
            }
        }

        private void ShowGameOverMenu()
        {
            GameOverMenu.Visibility = Visibility.Visible;
            SaveData();
            FinalScoreText.Text = $"Best Score: {gameState.Score}";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver || IsGamePaused) return;

            switch (e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.Z:
                    gameState.RotateBlockCCW();
                    break;
                case Key.C:
                    gameState.HoldBlock();
                    break;
                case Key.Space:
                    gameState.DropBlock();
                    break;
                case Key.P:
                    ButtonControl_Click(null, null);
                    break;
                case Key.M:
                    ButtonVolume_Click(ButtonVolume, null);
                    break;
                default:
                    return;
            }

            e.Handled = true;
            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            if (!gameState.GameOver)
            {
                await GameLoop();
            }
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            clickedAudio.Play();
            isGameLoopRunning = false;
            gameState = new GameState();
            SetTime();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }


        public void ButtonVolume_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button != null)
            {
                var template = button.Template;
                var buttonImage = (Image)template.FindName("ButtonImage", button);
                if (buttonImage != null)
                {
                    if (IsMuted)
                    {
                        buttonImage.Source = new BitmapImage(new Uri("../resources/button/ButtonUnmute_Default.png", UriKind.Relative));
                        gameState.SetVolume(1);
                    }
                    else
                    {
                        buttonImage.Source = new BitmapImage(new Uri("../resources/button/ButtonMute_Default.png", UriKind.Relative));
                        gameState.SetVolume(0);
                    }
                    IsMuted = !IsMuted;
                }
            }

        }

        private void ButtonControl_Click(object sender, RoutedEventArgs e)
        {
            clickedAudio.Play();
            Pause pauseMenu = new Pause(this);

            if (!IsGamePaused)
            {
                IsGamePaused = true;
                PauseMenu.Visibility = Visibility.Visible;
                pauseMenu.Show();
            }

        }

        public void ContinueGame()
        {
            if (!IsGamePaused)
            {
                return;
            }

            IsGamePaused = false;

            if (!gameState.GameOver)
            {
                timer.Start();
            }
        }
    }
}


