using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using States;
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
        public StateMachine StateMachine { get; private set; } = null!;
        private Point _lastSize;

        public GameMain()
        {
            Graphics = new GraphicsDeviceManager(this);
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsFixedTimeStep = false;
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            StateMachine = new StateMachine(SpriteBatch);
            StateMachine.AddState("main", new MainState(this));
            StateMachine.ChangeState("main");
        }

        protected override void Update(GameTime gameTime)
        {
            var currentSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
            if (currentSize != _lastSize)
            {
                _lastSize = currentSize;
                OnResize(currentSize.X, currentSize.Y);
            }

            base.Update(gameTime);
            StateMachine.Update(this, gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            StateMachine.Draw(this, gameTime);
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
