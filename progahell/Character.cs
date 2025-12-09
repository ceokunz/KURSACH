using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace progahell
{
    public abstract class Character
    {
        string name;
        EmoteType currentEmote;

        private readonly Dictionary<EmoteType, string> SpritePaths;

        public event Action<string> SpeakRequested;

        public string Name
            { get { return name; } set { name = value; } }
        public EmoteType CurrentEmote { get { return currentEmote; } set { currentEmote = value; } }

        public string CurrentSpritePath => SpritePaths[CurrentEmote];

        public Character(string name, Dictionary<EmoteType, string> spritePaths)
        {
            Name = name;
            SpritePaths = spritePaths;
        }

        public void SetEmote(EmoteType emote)
        {
                CurrentEmote = emote;
        }

        public virtual void OnSpeak()
        {
            SpeakRequested?.Invoke(GetAnimationType());
            //короч при появлении разных персонажей будет разная анимация, я не знаю куда еще наследку пихать
        }

        protected virtual string GetAnimationType() => "none";

    }
}
