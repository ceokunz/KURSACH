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

namespace progahell
{
    public partial class MainWindow : Window
    {
        private SceneManager sceneManager = new();
        private CharacterManager characterManager = new();
        public ScoreCalculator ScoreCalculator { get; } = new();

        private List<DialogueLine> currentDialogue = new();
        private int currentDialogueIndex = 0;

        private bool isShowingChoices = false;
        public MainWindow()
        {
            InitializeComponent();
            LoadGameData(); //загрузка файлов игры

            sceneManager.SceneChanged += OnSceneChanged;
            sceneManager.Start("start");
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

                scene.Dialogue = s.Dialogue ?? new List<DialogueLine>();

                scene.Choices.Clear();
                scene.Choices.AddRange(s.Choices ?? new List<Choice>());

                scene.InitializeLayoutFromJson(s.CharacterLayout);
                sceneManager.AddScene(scene);
            }
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
                    character.SetEmote(EmoteType.Neutral); // ← сброс к нейтральной
                }
            }

            // Сбрасываем спрайты (скрываем)
            ClearCharacterPositions();

            // Показываем персонажей с НЕЙТРАЛЬНОЙ эмоцией
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

                SpeakerNameBlock.Text = speaker?.Name ?? "???";
                DialogueTextBlock.Text = line.Text;

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
            Scene scene = sceneManager.CurrentScene;
            if (scene.Choices.Count > 0)
            {
                ChoicesPanel.ItemsSource = scene.Choices;
                ChoicesPanel.Visibility = Visibility.Visible;
                NextButton.Visibility = Visibility.Collapsed; //убиваем кнопку во время того как выборы показывается
                isShowingChoices = true;
                Choices_Back.Visibility = Visibility.Visible; // ща как трайну
            }
            else
            {
                if (!string.IsNullOrEmpty(scene.NextSceneId))
                {
                    sceneManager.GoTo(scene.NextSceneId);
                }
                else
                {
                    ShowEnding();
                }
            }

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
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string BackgroundPath { get; set; } = "";
        public string NextSceneId { get; set; } = "";
        public List<Choice> Choices { get; set; } = new();
        public List<DialogueLine> Dialogue { get; set; } = new();
        public Dictionary<string, string> CharacterLayout { get; set; } = new();
    }
}