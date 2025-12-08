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

        private readonly Dictionary<EmoteType, Image> _sprites;

        public string Name
            { get { return name; } set { name = value; } }
        public EmoteType CurrentEmote { get { return currentEmote; } set { currentEmote = value; } }

        public Image CurrentSprite => _sprites[CurrentEmote];

        public Character(string name, Dictionary<EmoteType, Image> sprites)
        {
            Name = name;
            _sprites = sprites;
            CurrentEmote = EmoteType.Neutral;
        }

        public void SetEmote(EmoteType emote)
        {
            if (_sprites.ContainsKey(emote))
            {
                CurrentEmote = emote;
                // обновить UI спрайта на CurrentSprite
            }
        }

        // какие функции должны быть у персонажа? Чето пока даже не догоняю. 

        // может, для создания отдельных персонажей Златы, Валеры и остальных сделать отдельные классы? Типо
        // public class Zlata : Character и пишем ей там конструктор и все такое, добавляем ей какую нибудь особенность
    }
}
