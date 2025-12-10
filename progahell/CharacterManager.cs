using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public class CharacterManager
    {
        private readonly Dictionary<string, Character> characters = new();

        public void AddCharacter(Character character)
        {
            if (character == null) throw new ArgumentNullException(nameof(character));
            characters[character.Id] = character;
        }

        public Character GetCharacter(string id)
        {
            return characters.GetValueOrDefault(id);
        }
    }
}
