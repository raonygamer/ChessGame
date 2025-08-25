using ChessGame.ECS;
using ChessGame.ECS.Components;
using EnTTSharp.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame
{
    public class ChessGame
    {
        public Texture2D? Board8x8Texture { get; private set; }

        public void Load(GameMain game, RegistryManager registryManager)
        {
            Board8x8Texture = game.Content.Load<Texture2D>("Boards/rect-8x8");
            SetupMainRegistry(registryManager);
            registryManager.SetCurrentRegistry("main");
        }

        public void SetupMainRegistry(RegistryManager registryManager)
        {
            var registry = registryManager.CreateRegistry("main");
            var boardEntity = registry.Create();
            registry.GetComponent<TransformComponent>(boardEntity, out var transform);
            if (transform == null) throw new InvalidOperationException("Transform component not found on board entity.");
            registry.AssignComponent(boardEntity, new Texture2DComponent(Board8x8Texture));
            registry.AssignComponent(boardEntity, new ScriptableComponent
            {
                OnUpdate = (registry, entity, time) =>
                {
                    transform.Position = new Vector2((registryManager.Game.Window.ClientBounds.Width / 2f), (registryManager.Game.Window.ClientBounds.Height / 2f));
                }
            });
            transform.Scale = Vector2.One / 2f;
            transform.Origin = Vector2.One / 2f;
        }
    }
}
