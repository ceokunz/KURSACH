using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            var backgroundBrush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/Elements/Background_Puzzle.png")),
                Stretch = Stretch.UniformToFill
            };
            _window = new Window
            {
                Title = "Мини-игра: Мотивация",
                Width = 800,
                Height = 550,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Background =  backgroundBrush
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

      
            var leftPanel = new StackPanel { Margin = new Thickness(20) };

            leftPanel.Children.Add(new TextBlock
            {
                Text = "Восстанови мотивацию Валеры и Златы",
                HorizontalAlignment = HorizontalAlignment.Center,
                Style = (Style)Application.Current.FindResource("TextBlockGameStyleTitle"),
                Margin = new Thickness(0, 0, 0, 20)
            });

            for (int i = 0; i < 3; i++)
            {
                var label = new TextBlock
                {
                    Text = $"Ответ {i + 1}:",
                    Style = (Style)Application.Current.FindResource("TextBlockGameStyleString"),
                    Margin = new Thickness(0, 0, 0, 5)
                };
                leftPanel.Children.Add(label);

                var dropZone = new Border
                {
                    Width = 320,
                    MinHeight = 50,
                    Padding = new Thickness(10),
                    Tag = i,
                    Style = (Style)Application.Current.FindResource("BorderStyle"),
                    AllowDrop = true
                };

                var placeholder = new TextBlock
                {
                    Text = "Перетащи сюда фразу",
                    Style = (Style)Application.Current.FindResource("TextBlockGameStyle"),
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
                Style = (Style)Application.Current.FindResource("ChoiceButtonStyle"),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 30, 0, 0),
                
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
                Style = (Style)Application.Current.FindResource("TextBlockGameStyleTitle"),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            });

            foreach (var phrase in _allOptions)
            {
                var phraseBlock = new Border
                {
                    Width = 320,
                    MinHeight = 50,

                    Padding = new Thickness(10),
                    Style = (Style)Application.Current.FindResource("BorderStyleRed"),
                    Margin = new Thickness(0, 0, 0, 12),
                    Cursor = Cursors.Hand
                };

                var text = new TextBlock
                {
                    Text = phrase,
                    Style = (Style)Application.Current.FindResource("TextBlockGameStyle"),
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
                Style = (Style)Application.Current.FindResource("TextBlockGameStyleGreen"),
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