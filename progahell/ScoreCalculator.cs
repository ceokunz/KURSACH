using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public class ScoreCalculator : INotifyPropertyChanged
    {
        int totalScore;
        public int TotalScore 
        { 
            get { return totalScore; } 
            private set 
            { 
                totalScore = value;
                OnPropertyChanged();
            } 
        }

        public void AddScore(int value)
        {
            TotalScore += value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
