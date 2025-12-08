using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public interface IChoice
    {
        string Text { get; }
        string Type { get; }
        int Score { get; }

        void OnClick(); // функция для выбора в самой игре
    }

    public enum EmoteType // типы эмоций спрайтов персонажей
    {
        Happy = 0,
        Sad = 1,
        Angry = 2,
        Neutral = 3
    }
}
