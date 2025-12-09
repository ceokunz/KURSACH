using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public class Zlata : Character
    {
        public Zlata(string name, Dictionary<EmoteType, string> spritePaths) : base("Злата", new Dictionary<EmoteType, string>
        {
            { EmoteType.Happy, "Res/Z_happy.png" },
            { EmoteType.Sad, "Res/Z_sad.png" },
            { EmoteType.Angry, "Res/Z_angry.png" },
            { EmoteType.Neutral, "Res/Z_neutral.png" }
        })
        { }

        //ааааааааааааааааааааааааааааааааааааааааааааааа

        protected override string GetAnimationType() => "bounce";

    }
}
