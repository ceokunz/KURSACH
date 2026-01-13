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
            int total = score.TotalScore;

            if (total == 0)
                return EndingType.Secret;

            if (total >= 75)
                return EndingType.Perfect;

            if (total >= 30)
                return EndingType.Good;

            return EndingType.Bad;
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
