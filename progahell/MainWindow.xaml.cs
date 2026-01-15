using System.Text;
using System.Text.Json;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace progahell
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private SceneManager sceneManager = new();
        private CharacterManager characterManager = new();
        private readonly MusicManager _musicManager = new();
        public ScoreCalculator ScoreCalculator { get; set; } = new();

        private List<DialogueLine> currentDialogue = new();
        private int currentDialogueIndex = 0;

        private bool isShowingChoices = false;

        private string speakerName = "";
        private string currentDialogueText = "";

        public string SpeakerName
        {
            get => speakerName;
            set
            {
                speakerName = value;
                OnPropertyChanged();
            }
        }

        public string CurrentDialogueText
        {
            get => currentDialogueText;
            set
            {
                currentDialogueText = value;
                OnPropertyChanged();
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LoadGameData();

            sceneManager.SceneChanged += OnSceneChanged;
            
            StartTitleAnimation();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadGameData()
        {
            // Чарики
            var charList = JsonSerializer.Deserialize<List<CharacterJsonModel>>(
                File.ReadAllText("Res/characters.json"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            foreach (var c in charList)
            {
                var character = new Character(c.Id, c.Name, new Dictionary<EmoteType, string>());
                character.InitializeFromJson(c.SpritePaths);
                characterManager.AddCharacter(character);
            }

            // Сцени
            var sceneList = JsonSerializer.Deserialize<List<SceneJsonModel>>(
                File.ReadAllText("scenes/scenes.json"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            foreach (var s in sceneList)
            {
                Scene scene = new Scene(s.Id, s.Title, s.Description, s.BackgroundPath, s.NextSceneId);
                scene.BackgroundMusic = s.BackgroundMusic;

                scene.Dialogue = s.Dialogue ?? new List<DialogueLine>();

                scene.Choices.Clear();
                scene.Choices.AddRange(s.Choices ?? new List<Choice>());

                scene.InitializeLayoutFromJson(s.CharacterLayout);
                scene.SetMiniGameType(s.MiniGameType);
                sceneManager.AddScene(scene);
            }
        }
        private void StartTitleAnimation()
        {
            var animation = new DoubleAnimation
            {
                From = -10,
                To = 10,
                Duration = TimeSpan.FromSeconds(2),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
            };

            TitleTransform.BeginAnimation(TranslateTransform.YProperty, animation);
        }
        private void OnSceneChanged(Scene scene)
        {
            if (!string.IsNullOrWhiteSpace(scene.BackgroundPath))
            {
                try
                {
                    string cleanPath = scene.BackgroundPath.TrimStart('/');
                    var uri = new Uri($"pack://application:,,,/{cleanPath}");
                    var bitmap = new BitmapImage(uri);
                    bitmap.Freeze();
                    MainGrid.Background = new ImageBrush(bitmap)
                    {
                        Stretch = Stretch.UniformToFill
                    };

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка фона: {ex}");
                    MainGrid.Background = Brushes.LightGray;
                }
            }
            else
            {
                MainGrid.Background = Brushes.White;
            }

            // сброс спрайтов
            foreach (var charId in scene.CharacterLayout.Values)
            {
                var character = characterManager.GetCharacter(charId);
                if (character != null)
                {
                    character.SetEmote(EmoteType.Neutral);
                }
            }

            ClearCharacterPositions();

            foreach (var layout in scene.CharacterLayout)
            {
                var position = layout.Key;
                var charId = layout.Value;
                var character = characterManager.GetCharacter(charId);

                if (character != null)
                {
                    SetCharacterSprite(position, character.CurrentSpritePath);
                }
            }

            _musicManager.PlayTrack(scene.BackgroundMusic); //кач кач делаем кач кач если не получится я убиваю себя пистолетом

            // сброс диалог
            currentDialogueIndex = 0; 
            currentDialogue = scene.Dialogue ?? new List<DialogueLine>();
            ShowNextDialogueLine();
        }

        private void ClearCharacterPositions()
        {
            LeftPos.Source = null;
            LeftPos.Visibility = Visibility.Hidden;

            CenterPos.Source = null;
            CenterPos.Visibility = Visibility.Hidden;

            RightPos.Source = null;
            RightPos.Visibility = Visibility.Hidden;
        }

        private void SetCharacterSprite(CharacterPosition position, string spritePath)
        {
            Image targetImage = position switch
            {
                CharacterPosition.Left => LeftPos,
                CharacterPosition.Center => CenterPos,
                CharacterPosition.Right => RightPos,
                _ => null
            };

            if (targetImage != null && !string.IsNullOrEmpty(spritePath))
            {
                try
                {
                    targetImage.Source = new BitmapImage(new Uri(spritePath, UriKind.Relative));
                    targetImage.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка загрузки спрайта: {ex.Message}");
                    targetImage.Visibility = Visibility.Hidden;
                }
            }
            else if (targetImage != null)
            {
                targetImage.Visibility = Visibility.Hidden;
            }
        }

        private void ShowNextDialogueLine()
        {
            if (currentDialogueIndex < currentDialogue.Count)
            {
                var line = currentDialogue[currentDialogueIndex];
                Character speaker = characterManager.GetCharacter(line.SpeakerId);

                SpeakerName = speaker?.Name ?? "???";
                CurrentDialogueText = line.Text;

                var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.4));
                DialogueTextBlock.BeginAnimation(UIElement.OpacityProperty, fadeIn);

                // обновляем эмоцию спикера
                if (speaker != null)
                {
                    // временный эмоут
                    speaker.SetEmote(line.EmoteEnum);

                    Scene currentScene = sceneManager.CurrentScene;

                    CharacterPosition? speakerPosition = null;
                    foreach (var layoutEntry in currentScene.CharacterLayout)
                    {
                        if (layoutEntry.Value == line.SpeakerId)
                        {
                            speakerPosition = layoutEntry.Key;
                            break;
                        }
                    }
                    if (speakerPosition.HasValue)
                    {
                        SetCharacterSprite(speakerPosition.Value, speaker.CurrentSpritePath);
                        AnimateJump(speakerPosition.Value);
                    }
                }
                currentDialogueIndex++;
                isShowingChoices = false;

                ChoicesPanel.Visibility = Visibility.Collapsed;
                NextButton.Visibility = Visibility.Visible;
            }
            else
            {
                ShowChoices();
            }
        }


        private void ShowChoices()
        {
            Scene currentScene = sceneManager.CurrentScene;

            NextButton.Visibility = Visibility.Collapsed;
            ChoicesPanel.Visibility = Visibility.Collapsed;
            Choices_Back.Visibility = Visibility.Collapsed;
            isShowingChoices = false;

            if (currentScene.MiniGameType != "none")
            {
                try
                {
                    var miniGame = MiniGameFactory.CreateGame(currentScene.MiniGameType);
                    if (miniGame != null)
                    {
                        miniGame.Completed += (game) =>
                        {
                            ScoreCalculator.AddScore(game.PlayerScore);
                            sceneManager.GoTo(currentScene.NextSceneId);
                        };
                        miniGame.Start();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка запуска мини-игры: {ex.Message}");
                }
            }

            if (currentScene.Choices.Count > 0)
            {
                ChoicesPanel.ItemsSource = currentScene.Choices;
                ChoicesPanel.Visibility = Visibility.Visible;
                Choices_Back.Visibility = Visibility.Visible;
                isShowingChoices = true;
            }
            else
            {
                if (currentScene.NextSceneId == "go_to_ending")
                {
                    ShowEnding();
                }
                else if (currentScene.NextSceneId == "show_credits_overlay")
                {
                    ShowCreditsOverlay();
                }
                else if (!string.IsNullOrEmpty(currentScene.NextSceneId))
                {
                    sceneManager.GoTo(currentScene.NextSceneId);
                }
            }
        }

        private void ShowEnding()
        {
            var endingType = new EndingCalculator().CalculateEnding(ScoreCalculator);
            string endingSceneId = endingType switch
            {
                EndingType.Perfect => "best",
                EndingType.Good => "good",
                EndingType.Bad => "bad",
                EndingType.Secret => "secret"
            };
            sceneManager.GoTo(endingSceneId);
        }

        private void ShowCreditsOverlay()
        {
            EndingOverlay.Visibility = Visibility.Visible;

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) }; //ну это же мега эпик? я дэб сорри
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                BackToMenuButton.Visibility = Visibility.Visible;
            };
            timer.Start();
        }
        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Скрываем стартовое меню
            StartOverlay.Visibility = Visibility.Collapsed;

            // Запускаем игру с первой сцены
            sceneManager.Start("start");
        }
        private void RestartGame()
        {
            ScoreCalculator = new ScoreCalculator();
            currentDialogue.Clear();
            currentDialogueIndex = 0;
            isShowingChoices = false;
            _musicManager.Stop();

            EndingOverlay.Visibility = Visibility.Collapsed;
            BackToMenuButton.Visibility = Visibility.Collapsed;

            StartOverlay.Visibility = Visibility.Visible;
            StartGameButton.Visibility = Visibility.Visible;
        }

        //=============================================================================== КЛИКИ

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (isShowingChoices)
            {
                ShowChoices();
            }
            else
            {
                ShowNextDialogueLine();
            }
        }

        private void ChoiceButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Choice choice)
            {
                ChoicesPanel.Visibility = Visibility.Collapsed;
                Choices_Back.Visibility = Visibility.Collapsed;
                NextButton.Visibility = Visibility.Visible;
                isShowingChoices = false;

                ScoreCalculator.AddScore(choice.Score);
                sceneManager.GoTo(choice.NextSceneId);
                
            }

        }
        

        private void BackToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            RestartGame();
        }

        //=============================================================================== АНИМАЦИИ
        private void AnimateJump(CharacterPosition position)
        {
            // Выбираем нужный Image
            Image targetImage = position switch
            {
                CharacterPosition.Left => LeftPos,
                CharacterPosition.Center => CenterPos,
                CharacterPosition.Right => RightPos,
                _ => null
            };

            if (targetImage == null) return;

            // Создаём свою трансформацию для этого изображения
            var translateTransform = new TranslateTransform();
            targetImage.RenderTransform = translateTransform;
            targetImage.RenderTransformOrigin = new Point(0.5, 1.0); // лучше прыгать от низа

            var bounceY = new DoubleAnimation(0, -20, TimeSpan.FromMilliseconds(100));
            var fallY = new DoubleAnimation(-20, 0, TimeSpan.FromMilliseconds(100));

            bounceY.Completed += (s, e) =>
            {
                translateTransform.BeginAnimation(TranslateTransform.YProperty, fallY);
            };

            translateTransform.BeginAnimation(TranslateTransform.YProperty, bounceY);
        }

        private void CloseGameButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
    public class CharacterJsonModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public Dictionary<string, string> SpritePaths { get; set; } = new();
    }

    public class SceneJsonModel
    {
        public string Id { get; set; } = "";
        public string BackgroundMusic { get; set; } = "";
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string BackgroundPath { get; set; } = "";
        public string NextSceneId { get; set; } = "";
        public List<Choice> Choices { get; set; } = new();
        public List<DialogueLine> Dialogue { get; set; } = new();
        public Dictionary<string, string> CharacterLayout { get; set; } = new();
        public string MiniGameType { get; set; } = "";
    }
}