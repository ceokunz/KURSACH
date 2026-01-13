using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace progahell
{
    public class Scene
    {
        string title;
        string description;
        string backgroundPath;
        string id;
        string nextSceneId;
        private Dictionary<CharacterPosition, string> characterLayout = new();
        public string MiniGameType { get; private set; } = "none"; // по умолчанию
        public List<Choice> Choices { get; } = new List<Choice>();
        public List<DialogueLine> Dialogue { get; set; }


        public Dictionary<CharacterPosition, string> CharacterLayout => characterLayout;

        public string Title
        { get { return title; } set { title = value; } }

        public string Description
        { get { return description; } set { description = value; } }

        public string BackgroundPath
        { get { return backgroundPath; } set { backgroundPath = value; } }

        public string Id
        { get { return id; } set { id = value; } } //попробуй тут везде приваточки влепить

        public string NextSceneId
        { get { return nextSceneId; } set { nextSceneId = value; } }

        public Scene(string id, string title, string description, string backgroundPath, string nextSceneId)
        {
            Id = id;
            Title = title;
            Description = description;
            BackgroundPath = backgroundPath;
            NextSceneId = nextSceneId;
            
        }
        public void SetMiniGameType(string type)
        {
            MiniGameType = type ?? "none";
        }
        public void SetCharacter(CharacterPosition position, string characterId)
        {
            CharacterLayout[position] = characterId; // ломай ломай код мы же миллионеры 
        }

        public void InitializeLayoutFromJson(Dictionary<string, string> jsonLayout)
        {
            characterLayout.Clear();
            foreach (var kvp in jsonLayout)
            {
                if (Enum.TryParse<CharacterPosition>(kvp.Key, true, out var pos))
                {
                    characterLayout[pos] = kvp.Value;
                }
            }
        }
    }

    public enum CharacterPosition //будем делать позиции чаров на сцене
    {
        Left,
        Center,
        Right
    }
}
