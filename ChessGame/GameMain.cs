using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using States;

internal static class Program
{
    private static void Main()
    {
        using var game = new GameMain();
        game.Run();
    }
}

public class GameMain : Game
{
    private Point lastSize;

    public GameMain()
    {
        Graphics = new GraphicsDeviceManager(this);
        Graphics.SynchronizeWithVerticalRetrace = false;
        Graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsFixedTimeStep = false;
        IsMouseVisible = true;
    }

    public GraphicsDeviceManager Graphics { get; }
    public SpriteBatch SpriteBatch { get; private set; } = null!;
    public StateMachine StateMachine { get; private set; } = null!;

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        StateMachine = new StateMachine(this, SpriteBatch);
        StateMachine.AddState("ui_test", new UITestState(this));
        StateMachine.AddState("ui_test2", new UITestState2(this));
        StateMachine.ChangeState("ui_test");
    }

    protected override void Update(GameTime gameTime)
    {
        var currentSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
        if (currentSize != lastSize)
        {
            lastSize = currentSize;
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
        lastSize = new Point(Window.ClientBounds.Width, Window.ClientBounds.Height);
        base.Initialize();
    }

    private void OnResize(int width, int height)
    {
        GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
        StateMachine.OnResize(new Vector2(width, height));
    }
}