using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace progahell
{
    public class PuzzleMiniGame : IMiniGame
    {
        private bool _isCompleted = false;
        public string Name => "Ритуал поднятия мотивации";
        public string GameType => "ritual";
        public int MaxScore => 100;
        public int PlayerScore { get; private set; }

        public event Action<IMiniGame> Completed;

        private readonly string[] _correctOrder = {
            "Мы всё успеем и пойдем есть пиццу!",
            "Как только допишем код можно будет выспаться",
            "Делаем быстрее и идем смотреть Властелина колец"
        };

        private readonly string[] _allOptions = {
            "Как только допишем код можно будет выспаться",
            "Мы всегда можем сжечь Златин ноутбук",
            "Делаем быстрее и идем смотреть Властелина колец",
            "Еще чуть чуть и Валера напишет фанфик про *** и ****",
            "Курсач горит, как ад!",
            "Мы всё успеем и пойдем есть пиццу!"

        };

        private string[] _currentAnswers = new string[3];
        private Border[] _dropZones = new Border[3];
        private Window _window;

        public void Start()
        {
            CreateWindow();
        }

        private void CreateWindow()
        {
            _window = new Window
            {
                Title = "Мини-игра: Мотивация",
                Width = 800,
                Height = 550,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background = new SolidColorBrush(Color.FromRgb(20, 10, 30))
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

      
            var leftPanel = new StackPanel { Margin = new Thickness(20) };

            leftPanel.Children.Add(new TextBlock
            {
                Text = "Восстанови мотивацию Валеры и Златы",
                Foreground = Brushes.Gold,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            });

            for (int i = 0; i < 3; i++)
            {
                var label = new TextBlock
                {
                    Text = $"Ответ {i + 1}:",
                    Foreground = Brushes.LightSalmon,
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                leftPanel.Children.Add(label);

                var dropZone = new Border
                {
                    Width = 320,
                    MinHeight = 50,
                    Background = new SolidColorBrush(Color.FromRgb(30, 15, 40)),
                    BorderBrush = Brushes.DarkRed,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(10),
                    Tag = i,
                    AllowDrop = true
                };

                var placeholder = new TextBlock
                {
                    Text = "Перетащи сюда фразу",
                    Foreground = Brushes.Gray,
                    FontStyle = FontStyles.Italic,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                dropZone.Child = placeholder;

                dropZone.Drop += OnDrop;
                dropZone.DragEnter += (s, e) =>
                {
                    if (e.Data.GetDataPresent(DataFormats.Text))
                        e.Effects = DragDropEffects.Copy;
                };

                leftPanel.Children.Add(dropZone);
                _dropZones[i] = dropZone;
            }

            var checkButton = new Button
            {
                Content = "Восстановить",
                FontSize = 16,
                Padding = new Thickness(10),
                Margin = new Thickness(0, 30, 0, 0),
                Background = Brushes.DarkRed,
                Foreground = Brushes.White
            };
            checkButton.Click += (s, e) => CheckSolution();
            leftPanel.Children.Add(checkButton);

            Grid.SetColumn(leftPanel, 0);
            grid.Children.Add(leftPanel);

            // ===== ПРАВАЯ ЧАСТЬ =====
            var rightPanel = new StackPanel { Margin = new Thickness(20) };

            rightPanel.Children.Add(new TextBlock
            {
                Text = "Мотивационные фразы:",
                Foreground = Brushes.Yellow,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            });

            foreach (var phrase in _allOptions)
            {
                var phraseBlock = new Border
                {
                    Width = 320,
                    MinHeight = 50,
                    Background = new SolidColorBrush(Color.FromRgb(60, 30, 80)),
                    BorderBrush = Brushes.Gold,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 12),
                    Cursor = Cursors.Hand
                };

                var text = new TextBlock
                {
                    Text = phrase,
                    Foreground = Brushes.White,
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    TextAlignment = TextAlignment.Center
                };
                phraseBlock.Child = text;

                phraseBlock.MouseLeftButtonDown += (s, e) =>
                {
                    var data = new DataObject(DataFormats.Text, phrase);
                    DragDrop.DoDragDrop(phraseBlock, data, DragDropEffects.Copy);
                };

                rightPanel.Children.Add(phraseBlock);
            }

            Grid.SetColumn(rightPanel, 1);
            grid.Children.Add(rightPanel);

            _window.Content = grid;
            _window.Closed += (s, e) => ForceEnd();
            _window.Show();
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.Text)) return;

            var text = (string)e.Data.GetData(DataFormats.Text);
            var zone = (Border)sender;
            var index = (int)zone.Tag;

            _currentAnswers[index] = text;

            zone.Child = new TextBlock
            {
                Text = text,
                Foreground = Brushes.White,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                TextAlignment = TextAlignment.Center
            };
        }

        private void CheckSolution()
        {
            var selected = new System.Collections.Generic.HashSet<string>();
            foreach (var answer in _currentAnswers)
            {
                if (!string.IsNullOrEmpty(answer))
                {
                    selected.Add(answer);
                }
            }

            var correctSet = new System.Collections.Generic.HashSet<string>(_correctOrder);

            bool isCorrect = correctSet.IsSubsetOf(selected) && selected.Count == 3;

            if (isCorrect)
            {
                PlayerScore = MaxScore;
                MessageBox.Show(
                    "Мотивация была поднята!\n\n Появился демон и согласился помочь с курсачом!",
                    "УСПЕХ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
                EndGame();
            }
            else
            {
                MessageBox.Show(
                    "Мотивация была понижена!\n\n Ноутбук Златы взорвался",
                    "ПРОВАЛ",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                PlayerScore = 0;
                EndGame();
            }
        }

        private void EndGame()
        {
            if (_isCompleted) return;
            _isCompleted = true;

            _window?.Close();
            Completed?.Invoke(this);
        }

        private void ForceEnd()
        {
            if (!_isCompleted)
            {
                PlayerScore = 0;
                _isCompleted = true;
                Completed?.Invoke(this);
            }
        }
    }
}