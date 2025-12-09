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
        string backgroundPath;
        string id;

        public List<Choice> Choices { get; } = new List<Choice>();
        public List<DialogueLine> Dialogue { get; set; }

        public string Title
        { get { return title; } set { title = value; } }

        public string Description
        { get { return description; } set { description = value; } }

        public string BackgroundPath
        { get { return backgroundPath; } set { backgroundPath = value; } }

        public string Id
        { get { return id; } set { id= value; } }

        public Scene(string id, string title, string description, string backgroundPath)
        {
            Id = id;
            Title = title;
            Description = description;
            BackgroundPath = backgroundPath;
            
        }
    }
}
