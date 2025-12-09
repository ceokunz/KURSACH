using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progahell
{
    public class SceneManager
    {
        private readonly Dictionary<string, Scene> scenes = new();
        public Scene CurrentScene { get; private set; }

        public event Action<Scene> SceneChanged;

        public void AddScene(Scene scene)
        {
            scenes[scene.Id] = scene;
        }

        public void Start(string startSceneId)
        {
            CurrentScene = scenes[startSceneId];
            SceneChanged?.Invoke(CurrentScene);
        }

        public void GoTo(string sceneId)
        {
            if (scenes.TryGetValue(sceneId, out Scene scene))
            {
                CurrentScene = scene;
                SceneChanged?.Invoke(CurrentScene);
            }
            else
            {
                // обработка ошибки
            }
        }


    }
}
