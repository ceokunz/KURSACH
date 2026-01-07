using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public class DialogueLine
    {
        string speakerId;
        string text;
        string emote;

        public string SpeakerId
        {
            get { return speakerId; }
            set { speakerId = value; }
        }
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public string Emote
        {
            get { return emote; }
            set { emote = value; }
        }
        public EmoteType EmoteEnum
        {
            get => Enum.TryParse<EmoteType>(Emote, true, out var e) ? e : EmoteType.Neutral;
        }
    }
}
