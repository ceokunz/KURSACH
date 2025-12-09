using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public class Zlata : Character
    {
        public Zlata(string name, Dictionary<EmoteType, string> spritePaths) : base(name, spritePaths)
        {

        }

        public void OnSpeak()
        {
            //мяумяу я хочу на гильотину. 
        }
    }
}
