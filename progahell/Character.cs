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

        private readonly Dictionary<EmoteType, string> SpritePaths;

        public event Action<string> SpeakRequested;

        public string Name
            { get { return name; } set { name = value; } }
        public EmoteType CurrentEmote 
            { get { return currentEmote; } set { currentEmote = value; } }
        public string Id
            { get { return id; } set { id = value; } }
        public string CurrentSpritePath => SpritePaths[CurrentEmote];

        public Character(string id, string name, Dictionary<EmoteType, string> spritePaths)
        {
            Id = id;
            Name = name;
            SpritePaths = spritePaths;
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
