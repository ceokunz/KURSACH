using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public interface IMiniGame
    {
        string Name { get; }          // Отображаемое имя ("Бегство из Ада")
        string GameType { get; }      // Технический ID ("clicker")
        int MaxScore { get; }
        int PlayerScore { get; }

        event Action<IMiniGame> Completed;
        void Start();
    }
}
