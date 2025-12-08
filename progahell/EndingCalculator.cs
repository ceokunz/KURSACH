using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public class EndingCalculator
    {
        int totalScore;
        
        public int TotalScore
        { get { return totalScore; } set { totalScore = value; } }

        public EndingType CalculateEnding(ScoreCalculator score)
        {
            if (score.TotalScore >= 100)
                return EndingType.Perfect;
            if (score.TotalScore >= 60)
                return EndingType.Good;
            if (score.TotalScore < 30)
                return EndingType.Bad;

            return EndingType.Secret;
        }
    }
    public enum EndingType
        {
            Perfect = 0,
            Good = 1,
            Bad = 2,
            Secret = 3
        }
}
