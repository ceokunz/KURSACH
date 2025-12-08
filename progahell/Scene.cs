using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace progahell
{
    public class Scene
    {
        string title;
        string description;
        Image background; 
        string id;
        public List<Choice> Choices { get; } = new List<Choice>();

       

        public string Title
        { get { return title; } set { title = value; } }

        public string Description
        { get { return description; } set { description = value; } }

        public Image Background
        { get { return background; } set { background = value; } }

        public string Id
        { get { return id; } set { id= value; } }

        public Scene(string id, string title, string description, Image background)
        {
            Id = id;
            Title = title;
            Description = description;
            Background = background;
            
        }

        public void Execute()
        {
            // показывает сцену в интерфейсе
        }

        // что ещё необходимо сцене?
    }
}
