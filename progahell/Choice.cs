using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public class Choice : IChoice
    {
        public string Text { get; }
        public string Type { get; }    
        public int Score { get; }
        public string NextSceneId { get; }

        public Choice(string text, string type, int score, string nextSceneId)
        {
            Text = text;
            Type = type;
            Score = score;
            NextSceneId = nextSceneId;
        }

        public void OnClick(SceneManager sceneManager, ScoreCalculator scoreCalc)
        {
            scoreCalc.AddScore(Score);
            sceneManager.GoTo(NextSceneId);
        }
    }
}
