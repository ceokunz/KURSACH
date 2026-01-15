using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace progahell
{
    public class ClickerMiniGame : IMiniGame
    {
        private bool _isCompleted = false;
        public string Name { get; private set; } = "Успей на лекцию";
        public string GameType { get; private set; } = "clicker";
        public int MaxScore { get; private set; } = 20;
        public int PlayerScore { get; private set; }

        public event Action<IMiniGame> Completed;

        //гейплей
        private const float TargetPosition = 200f;
        private const float DriftSpeed = 5f;
        private const float DriftIntervalSeconds = 0.3f;
        private const float ClickStep = 5f;           
        private const int TimeLimitSeconds = 30;       

        private float currentPosition = 0f;
        private int timeLeft = TimeLimitSeconds;
        private bool isGameActive = true;
        private DispatcherTimer countdownTimer;

        private Window gameWindow;
        private DispatcherTimer driftTimer;
        private TextBlock positionText;
        private TextBlock timerText;
        private ProgressBar progressBar;
        private Button clickButton;
        private TextBlock statusText;

        public void Start()
        {
            CreateAndShowWindow();
            StartGameLogic();
        }

        private void CreateAndShowWindow()
        {
            // новое окно мини-игры
            gameWindow = new Window
            {
                Title = Name,
                Width = 700,
                Height = 550,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.Black
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) }); // Позиция
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) }); // Таймер
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) }); // Прогресс
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) }); // Кнопка
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) }); // Статус

            //позиция
            positionText = new TextBlock
            {
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Style = (Style)Application.Current.FindResource("TextBlockGameStyle"),
                Margin = new Thickness(10)
            };
            Grid.SetRow(positionText, 0);
            grid.Children.Add(positionText);

            //таймер
            timerText = new TextBlock
            {
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                Style = (Style)Application.Current.FindResource("TextBlockGameStyle"),
                Margin = new Thickness(10)
            };
            Grid.SetRow(timerText, 1);
            grid.Children.Add(timerText);

            // прогресс-бар
            progressBar = new ProgressBar
            {
                Minimum = 0,
                Maximum = TargetPosition,
                Value = 0,
                Width = 500,
                Style = (Style)Application.Current.FindResource("ProgressBarStyle"),
                Margin = new Thickness(20, 0, 20, 0)
            };
            Grid.SetRow(progressBar, 2);
            grid.Children.Add(progressBar);

            // кнопка
            clickButton = new Button
            {
                Content = "БЕЖАТЬ!",
                Style = (Style)Application.Current.FindResource("NextButtonStyle"),
                Foreground = Brushes.OrangeRed
            };
            clickButton.Click += (s, e) => OnClick();
            Grid.SetRow(clickButton, 3);
            grid.Children.Add(clickButton);

            // Статус
            statusText = new TextBlock
            {
                Foreground = Brushes.Yellow,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 16,
                Text = "Успей на лекцию за 30 секунд!",
                Margin = new Thickness(10)
            };
            Grid.SetRow(statusText, 4);
            grid.Children.Add(statusText);

            gameWindow.Content = grid;

            gameWindow.Closed += (s, e) => ForceEnd();

            gameWindow.Show();
        }

        private void StartGameLogic()
        {
            UpdateUI();

            driftTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(DriftIntervalSeconds)
            };
            driftTimer.Tick += (s, e) =>
            {
                if (!isGameActive) return;
                currentPosition -= DriftSpeed;
                CheckGameState();
                UpdateUI();
            };
            driftTimer.Start();

            countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            countdownTimer.Tick += (s, e) =>
            {
                if (!isGameActive) return;
                timeLeft--;
                CheckGameState();
                UpdateUI();
            };
            countdownTimer.Start();
        }

        private void OnClick()
        {
            if (!isGameActive || gameWindow == null) return;
            currentPosition += ClickStep;
            CheckGameState();
            UpdateUI();
        }

        private void CheckGameState()
        {
            if (currentPosition >= TargetPosition)
            {
                Win();
            }
            else if (timeLeft <= 0)
            {
                Lose();
            }
        }

        private void Win()
        {
            statusText.Text = "Валера успел на лекцию!";
            statusText.Foreground = Brushes.GreenYellow;
            clickButton.IsEnabled = false;
            EndGame(true);
        }

        private void Lose()
        {
            statusText.Text = "Валера застрял в заборе и опоздал на полчаса...";
            statusText.Foreground = Brushes.Red;
            clickButton.IsEnabled = false;
            EndGame(false);
        }

        private void ForceEnd()
        {
            if (!_isCompleted)
            {
                _isCompleted = true;
                PlayerScore = 0;
                Completed?.Invoke(this);
            }
        }

        private void EndGame(bool win)
        {
            if (_isCompleted) return;
            _isCompleted = true;

            isGameActive = false;
            driftTimer?.Stop();
            countdownTimer?.Stop();

            PlayerScore = win ? MaxScore : 0;

            _ = Task.Run(async () =>
            {
                await Task.Delay(2000);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    gameWindow?.Close();
                });
            });

            Completed?.Invoke(this);
        }

        private void UpdateUI()
        {
            if (gameWindow == null || !gameWindow.IsLoaded) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                positionText.Text = $"Осталось до универа: {Math.Max(0, (int)currentPosition)} / {(int)TargetPosition}";
                timerText.Text = $"Осталось {Math.Max(0, timeLeft)} секунд";
                progressBar.Value = Math.Min(TargetPosition, Math.Max(0, currentPosition));
            });
        }
    }
}