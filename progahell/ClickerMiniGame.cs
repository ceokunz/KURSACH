using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public class ClickerMiniGame : IMiniGame
    {
        public string Name => "Clicker";
        public int MaxScore => 10;
        public int PlayerScore { get; private set; }

        public event Action<IMiniGame> Completed;

        public void Start()
        {
            //запускаем окно мини‑игры?
        }
    }
}

