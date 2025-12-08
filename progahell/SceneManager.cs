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

        public void AddScene(Scene scene)
        {
            scenes[scene.Id] = scene;
        }

        public void Start(string startSceneId)
        {
            CurrentScene = scenes[startSceneId];
            ShowCurrentScene();
        }

        public void GoTo(string sceneId)
        {
            if (scenes.TryGetValue(sceneId, out var scene))
            {
                CurrentScene = scene;
                ShowCurrentScene();
            }
            else
            {
                // обработка ошибки
            }
        }

        private void ShowCurrentScene()
        {
            // обновляет WPF UI: фон, текст, кнопки с выбором
        }
    }
}
