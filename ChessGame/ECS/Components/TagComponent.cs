using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.ECS.Components
{
    public class TagComponent : IComponent
    {
        public string Name { get; set; } = "Empty Entity";

        public TagComponent()
        {
        }

        public TagComponent(string name)
        {
            Name = name;
        }
    }
}
