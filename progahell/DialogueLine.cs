using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public class DialogueLine
    {
        public string SpeakerId { get; set; }
        public string Text { get; set; }
        public EmoteType Emote { get; set; }
    }
}
