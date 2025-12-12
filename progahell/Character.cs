using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace progahell
{
    public class Character
    {
        string name;
        EmoteType currentEmote;
        string id;
        private Dictionary<EmoteType, string> spritePaths = new();

        public event Action<string> SpeakRequested;

        public string Name
            { get { return name; } set { name = value; } }
        public EmoteType CurrentEmote 
            { get { return currentEmote; } set { currentEmote = value; } }
        public string Id
            { get { return id; } set { id = value; } }
        public Dictionary<EmoteType, string> SpritePaths => spritePaths;

        public string CurrentSpritePath => spritePaths.TryGetValue(CurrentEmote, out var path) ? path : "";

        public Character() { }

        public Character(string id, string name, Dictionary<EmoteType, string> spritePaths)
        {
            Id = id;
            Name = name;
            this.spritePaths = new Dictionary<EmoteType, string>(spritePaths);
        }

        public void InitializeFromJson(Dictionary<string, string> jsonSprites)
        {
            spritePaths.Clear();
            foreach (var kvp in jsonSprites)
            {
                if (Enum.TryParse<EmoteType>(kvp.Key, true, out var emote))
                {
                    spritePaths[emote] = kvp.Value;
                }
            }
        }

        public void SetEmote(EmoteType emote)
        {
                CurrentEmote = emote;
        }

        public void OnSpeak()
        {
            SpeakRequested?.Invoke(GetAnimationType());
            //короч при появлении разных персонажей будет разная анимация, я не знаю куда еще наследку пихать
        }

        public string GetAnimationType()
        {
            return "none";
        }
    }
}
