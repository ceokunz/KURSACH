using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public class ClickerMiniGame : IMiniGame
    {
        string name;
        int maxScore;
        int playerScore;

        public string Name
        {
            get { return name; }
            private set { name = value; }
        }

        public int MaxScore
        {
            get { return maxScore; }
            private set { maxScore = value; }
        }

        public int PlayerScore 
        { 
            get { return playerScore; }
            private set {  playerScore = value; }
        }

        public ClickerMiniGame (string name, int maxScore, int playerScore)     
        {
            name = "Clicker";
            maxScore = 10;
            PlayerScore = playerScore;
        }

        public event Action<IMiniGame> Completed;

        public void Start()
        {
            //запускаем окно мини‑игры?
        }
    }
}

