using ChessGame.ECS.Components;
using EnTTSharp.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.ECS
{
    public class RegistryManager
    {
        public readonly GameMain Game;
        public readonly Dictionary<string, EntityRegistry<EntityKey>> Registries = new();
        public EntityRegistry<EntityKey>? CurrentRegistry { get; private set; } = null;
        public string? CurrentRegistryId { get; private set; } = null;

        public RegistryManager(GameMain game)
        {
            Game = game;
        }

        public EntityRegistry<EntityKey> CreateRegistry(string id)
        {
            if (Registries.ContainsKey(id))
            {
                throw new InvalidOperationException($"Registry with id '{id}' already exists.");
            }
            var registry = new EntityRegistry<EntityKey>(int.MaxValue, (age, key) => new EntityKey(age, key));
            Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IComponent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList()
                .ForEach(t =>
                {
                    var method = typeof(EntityRegistry<EntityKey>).GetMethod("Register", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, [])!.MakeGenericMethod(t);
                    method.Invoke(registry, null);
                });
            registry.EntityCreated += OnEntityCreated;
            Registries[id] = registry;
            return registry;
        }

        private void OnEntityCreated(object? sender, EntityKey e)
        {
            var registry = (EntityRegistry<EntityKey>)sender!;
            registry.AssignComponent(e, new TagComponent());
            registry.AssignComponent(e, new TransformComponent());
        }

        public void SetCurrentRegistry(string id)
        {
            if (!Registries.TryGetValue(id, out EntityRegistry<EntityKey>? value))
            {
                throw new InvalidOperationException($"Registry with id '{id}' does not exist.");
            }
            CurrentRegistry = value;
            CurrentRegistryId = id;
        }

        public void RemoveRegistry(string id)
        {
            if (!Registries.ContainsKey(id))
            {
                throw new InvalidOperationException($"Registry with id '{id}' does not exist.");
            }
            Registries.Remove(id);
            if (CurrentRegistryId == id)
            {
                CurrentRegistry = null;
                CurrentRegistryId = null;
            }
        }

        public EntityRegistry<EntityKey> GetRegistry(string id)
        {
            if (!Registries.TryGetValue(id, out EntityRegistry<EntityKey>? value))
            {
                throw new InvalidOperationException($"Registry with id '{id}' does not exist.");
            }
            return value;
        }

        public bool HasRegistry(string id)
        {
            return Registries.ContainsKey(id);
        }

        public void Clear()
        {
            Registries.Clear();
            CurrentRegistry = null;
            CurrentRegistryId = null;
        }

        public void Update(GameTime time)
        {
            if (CurrentRegistry == null) return;
            var entities = CurrentRegistry.View<ScriptableComponent>();
            foreach (var entity in entities)
            {
                CurrentRegistry.GetComponent<ScriptableComponent>(entity, out var scriptable);
                scriptable?.OnUpdate?.Invoke(CurrentRegistry, entity, time);
            }
        }

        public void Draw(GameTime time)
        {
            if (CurrentRegistry == null) return;
            var spriteBatch = Game.SpriteBatch;
            Game.GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            var entities = CurrentRegistry.View<TransformComponent, Texture2DComponent>();
            foreach (var entity in entities)
            {
                CurrentRegistry.GetComponent<TransformComponent>(entity, out var transform);
                CurrentRegistry.GetComponent<Texture2DComponent>(entity, out var texture);
                if (transform is not null && texture?.Texture is not null)
                {
                    spriteBatch.Draw(texture.Texture, transform.Position, texture.Rectangle, texture.Color, transform.Rotation, transform.Origin * new Vector2(texture.Rectangle.Width, texture.Rectangle.Height), transform.Scale, texture.Effects, texture.LayerDepth);
                }
            }
            spriteBatch.End();
        }
    }
}
