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

namespace progahell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SceneManager sceneManager = new();
        private CharacterManager characterManager = new();
        private ScoreCalculator scoreCalculator = new();
        public MainWindow()
        {
            InitializeComponent();
            // запуск json-а с информацией о всех сценах и персонажах
            LoadGameData();
            sceneManager.SceneChanged += OnSceneChanged;
            sceneManager.Start("start");
        }

        private void LoadGameData()
        {
            // Персонажи
            var charList = JsonSerializer.Deserialize<List<CharacterJsonModel>>(
                File.ReadAllText("Res/characters.json"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            foreach (var c in charList)
            {
                var character = new Character(c.Id, c.Name, new Dictionary<EmoteType, string>());
                character.InitializeFromJson(c.SpritePaths); // передаём Dictionary<string, string>
                characterManager.AddCharacter(character);
            }

            // Сцены
            var sceneList = JsonSerializer.Deserialize<List<SceneJsonModel>>(
                File.ReadAllText("scenes/scenes.json"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            foreach (var s in sceneList)
            {
                var scene = new Scene(s.Id, s.Title, s.Description, s.BackgroundPath);

                scene.Dialogue = s.Dialogue ?? new List<DialogueLine>();

                scene.Choices.Clear();
                scene.Choices.AddRange(s.Choices ?? new List<Choice>());

                scene.InitializeLayoutFromJson(s.CharacterLayout);
                sceneManager.AddScene(scene);
            }
        }

        private void OnSceneChanged(Scene scene)
        {
            
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            // ymer
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
        public List<Choice> Choices { get; set; } = new();
        public List<DialogueLine> Dialogue { get; set; } = new();
        public Dictionary<string, string> CharacterLayout { get; set; } = new();
    }
}