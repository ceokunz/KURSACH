using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public class Choice
    {
        string text;

        string type;

        int score;

        string nextSceneId;

        public string Text
        { 
            get { return text; } 
            private set {  text = value; }       
        }
        public string Type
        {
            get { return type; }
            private set { type = value; }
        }
        public int Score
        {
            get { return score; }
            private set { score = value; }
        }
        public string NextSceneId
        {
            get { return nextSceneId; }
            private set { nextSceneId = value; }
        }

        public Choice(string text, string type, int score, string nextSceneId)
        {
            Text = text;
            Type = type;
            Score = score;
            NextSceneId = nextSceneId;
        }
    }
}
