using System;
using System.Collections.Generic;

namespace progahell
{
    public static class MiniGameFactory
    {
        private static readonly Dictionary<string, Func<IMiniGame>> gameRegistry =
            new Dictionary<string, Func<IMiniGame>>(StringComparer.OrdinalIgnoreCase)
            {
                { "clicker", () => new ClickerMiniGame() }
                // Сюда можно добавлять: { "memory", () => new MemoryGame() }, и т.д.
            };

        public static IMiniGame CreateGame(string gameType)
        {
            if (gameType == null || gameType == "none")
                return null;

            if (gameRegistry.TryGetValue(gameType, out var creator))
            {
                return creator();
            }

            throw new ArgumentException($"Неизвестный тип мини-игры: {gameType}");
        }
    }
}