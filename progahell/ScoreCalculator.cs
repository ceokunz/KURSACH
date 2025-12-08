using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public class ScoreCalculator
    {
        int totalScore;
        public int TotalScore { get { return totalScore; } private set { totalScore = value; } }

        public void AddScore(int value)
        {
            TotalScore += value;
        }
    }
}
