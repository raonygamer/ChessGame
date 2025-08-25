using ChessGame.ECS;
using ChessGame.ECS.Components;
using EnTTSharp.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ChessGame
{
    internal static class Program
    {
        static void Main()
        {
            using var game = new GameMain();
            game.Run();
        }
    }

    public class GameMain : Game
    {
        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; } = null!;
        public RegistryManager RegistryManager { get; private set; }
        public ChessGame ChessGame { get; private set; }
        private Point _lastSize;

        public GameMain()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            IsMouseVisible = true;
            RegistryManager = new RegistryManager(this);
            ChessGame = new ChessGame();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            ChessGame.Load(this, RegistryManager);
        }

        protected override void Update(GameTime gameTime)
        {
            var currentSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            if (currentSize != _lastSize)
            {
                _lastSize = currentSize;
                OnResize(currentSize.X, currentSize.Y);
            }

            RegistryManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            RegistryManager.Draw(gameTime);
            base.Draw(gameTime);
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            _lastSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            base.Initialize();
        }

        private void OnResize(int width, int height)
        {
            GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
        }
    }
}
