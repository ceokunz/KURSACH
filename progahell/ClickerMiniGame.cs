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
        public string Name { get; private set; } = "Бегство из Преисподней";
        public string GameType { get; private set; } = "clicker";
        public int MaxScore { get; private set; } = 100;
        public int PlayerScore { get; private set; }

        public event Action<IMiniGame> Completed;

        // === Настройки геймплея ===
        private const float TargetPosition = 100f;     // Финальная точка
        private const float DriftSpeed = 1f;           // Откат назад в секунду
        private const float ClickStep = 5f;            // Прогресс за клик
        private const int TimeLimitSeconds = 30;       // Время на прохождение

        // === Внутренние переменные ===
        private float currentPosition = 0f;
        private int timeLeft = TimeLimitSeconds;
        private bool isGameActive = true;

        // === UI элементы ===
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
            // Создаём новое окно для мини-игры
            gameWindow = new Window
            {
                Title = Name,
                Width = 500,
                Height = 350,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.Black
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) }); // Позиция
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) }); // Таймер
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) }); // Прогресс
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(80) }); // Кнопка
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) }); // Статус

            // Текст: позиция
            positionText = new TextBlock
            {
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 18,
                Margin = new Thickness(10)
            };
            Grid.SetRow(positionText, 0);
            grid.Children.Add(positionText);

            // Текст: таймер
            timerText = new TextBlock
            {
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 18,
                Margin = new Thickness(10)
            };
            Grid.SetRow(timerText, 1);
            grid.Children.Add(timerText);

            // Прогресс-бар
            progressBar = new ProgressBar
            {
                Minimum = 0,
                Maximum = TargetPosition,
                Value = 0,
                Margin = new Thickness(20, 0, 20, 0)
            };
            Grid.SetRow(progressBar, 2);
            grid.Children.Add(progressBar);

            // Кнопка "клик"
            clickButton = new Button
            {
                Content = "КЛИКНИ, ЧТОБЫ ДВИНУТЬСЯ ВПЕРЁД!",
                FontSize = 16,
                Margin = new Thickness(20),
                Background = Brushes.DarkRed,
                Foreground = Brushes.White
            };
            clickButton.Click += (s, e) => OnClick();
            Grid.SetRow(clickButton, 3);
            grid.Children.Add(clickButton);

            // Статус (победа/поражение)
            statusText = new TextBlock
            {
                Foreground = Brushes.Yellow,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 16,
                Text = "Успей добраться до цели за 30 секунд!",
                Margin = new Thickness(10)
            };
            Grid.SetRow(statusText, 4);
            grid.Children.Add(statusText);

            gameWindow.Content = grid;

            // Если игрок закроет окно — считаем поражение
            gameWindow.Closed += (s, e) => ForceEnd();

            gameWindow.Show();
        }

        private void StartGameLogic()
        {
            UpdateUI();

            // Таймер отката (раз в секунду)
            driftTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            driftTimer.Tick += (s, e) =>
            {
                if (!isGameActive) return;
                currentPosition -= DriftSpeed;
                timeLeft--;
                CheckGameState();
                UpdateUI();
            };
            driftTimer.Start();
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
            EndGame(true);
            PlayerScore = MaxScore;
            statusText.Text = "✅ ПОБЕДА! Ты вырвался из Ада!";
            statusText.Foreground = Brushes.GreenYellow;
            clickButton.IsEnabled = false;
        }

        private void Lose()
        {
            EndGame(false);
            PlayerScore = 0;
            statusText.Text = "💀 ПОРАЖЕНИЕ... Время вышло.";
            statusText.Foreground = Brushes.Red;
            clickButton.IsEnabled = false;
        }

        private void ForceEnd()
        {
            if (isGameActive)
            {
                PlayerScore = 0;
                Completed?.Invoke(this);
            }
        }

        private void EndGame(bool win)
        {
            if (!isGameActive) return;
            isGameActive = false;
            driftTimer?.Stop();

            // Через 2 секунды закрываем окно автоматически
            _ = Task.Run(async () =>
            {
                await Task.Delay(2000);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (gameWindow != null && gameWindow.IsVisible)
                        gameWindow.Close();
                });
            });

            Completed?.Invoke(this);
        }

        private void UpdateUI()
        {
            if (gameWindow == null || !gameWindow.IsLoaded) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                positionText.Text = $"Позиция: {Math.Max(0, (int)currentPosition)} / {(int)TargetPosition}";
                timerText.Text = $"Времени осталось: {Math.Max(0, timeLeft)} сек";
                progressBar.Value = Math.Min(TargetPosition, Math.Max(0, currentPosition));
            });
        }
    }
}