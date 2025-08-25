using EnTTSharp.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.ECS.Components
{
    public class ScriptableComponent : IComponent
    {
        public Action<EntityRegistry<EntityKey>, EntityKey, GameTime>? OnUpdate;

        public ScriptableComponent()
        {
        }
    }
}
