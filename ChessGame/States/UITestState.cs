using System;
using System.Linq;
using Core;
using Core.Nodes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace States;

public class UITestState : State
{
    public CanvasNode Canvas = new();
    public ImageNode MainControl = new();
    public ImageNode Quad = new();
    public TextNode Text = new();

    public UITestState(Game game) : base(game)
    {
        var tex = new Texture2D(game.GraphicsDevice, 256, 256);
        tex.SetData(Enumerable.Repeat(Color.Black, tex.Width * tex.Height).ToArray());

        var tex2 = new Texture2D(game.GraphicsDevice, 16, 16);
        tex2.SetData(Enumerable.Repeat(new Color(0f, 0f, 0f, 0.3f), tex2.Width * tex2.Height).ToArray());
        
        var font = game.Content.Load<SpriteFont>("Fonts/vcr");

        MainControl.Texture = tex;
        MainControl.Transform.Parent = Canvas.Transform;
        MainControl.Transform.Rotation = MathHelper.ToRadians(0);
        MainControl.Color = new Color(0, 0, 0, 0.5f);
        MainControl.Transform.AnchorMin = new Vector2(0f, 0f);
        MainControl.Transform.AnchorMax = new Vector2(1f, 1f);
        MainControl.Transform.Margin = new Rectangle(10, 10, 10, 10);
        MainControl.Transform.Padding = new Rectangle(10, 10, 10, 10);
        MainControl.Transform.StretchWithAnchors = true;

        Quad.Texture = tex2;
        Quad.Transform.Parent = MainControl.Transform;
        // Quad.ClipsToBounds = true;
        Quad.Transform.AnchorMin = new Vector2(0f, 0f);
        Quad.Transform.AnchorMax = new Vector2(1f, 1f);

        Quad.Transform.StretchWithAnchors = true;

        Text.Text = "Hello, World! This is a test of the UI system. New line here.";
        Text.Transform.Parent = MainControl.Transform;
        Text.Font = font;
        Text.Transform.Position = new Vector2(10, 10);
        Text.Transform.Scale = new Vector2(0.3f, 0.3f);
        //Text.Transform.Anchor = new Vector2(0.5f, 0.5f);
        //Text.Transform.Origin = new Vector2(0.5f, 0.5f);
        Text.ClipsToBounds = true;
        Text.AutoBreakLines = true;

        AddNode(Canvas);
        AddNode(MainControl);
        AddNode(Quad);
        AddNode(Text);
    }

    public override void OnEnter(StateMachine machine, Game game)
    {
        base.OnEnter(machine, game);
    }

    public override void OnExit(StateMachine machine, Game game)
    {
        base.OnExit(machine, game);
    }

    float xVel = 1f;
    float yVel = 1f;
    
    float xVel2 = 1f;
    float yVel2 = 1f;
    
    public override void Update(StateMachine machine, Game game, GameTime time)
    {
        const float mult = 2f;
        var cos = Remap((float)Math.Cos(time.TotalGameTime.TotalSeconds * mult));
        var sin = Remap((float)Math.Sin(time.TotalGameTime.TotalSeconds * mult));
        
        // Quad.Transform.Anchor = new Vector2(cos, sin);
        // Quad.Transform.Origin = new Vector2(cos, sin);
        //MainControl.Transform.AnchorMin = new Vector2(0f + cos / 16f, 0f + sin / 16f);
        //MainControl.Transform.AnchorMax = new Vector2(0.5f + cos / 16f, 0.5f + sin / 16f);
        //MainControl.Transform.Pivot = new Vector2(0.5f, 0.5f);
        //Quad.Transform.AnchorMin += new Vector2(xVel2, yVel2) * (float)time.ElapsedGameTime.TotalSeconds * 1.5f;
        //Quad.Transform.Pivot = Quad.Transform.AnchorMin;
        //if (Quad.Transform.AnchorMin.X >= 1f)
        //    xVel2 = -1 + Random.Shared.NextSingle() / 5f;
        //if (Quad.Transform.AnchorMin.X <= 0f)
        //    xVel2 = 1 + Random.Shared.NextSingle() / 5f;
        //if (Quad.Transform.AnchorMin.Y >= 1f)
        //    yVel2 = -1 + Random.Shared.NextSingle() / 5f;
        //if (Quad.Transform.AnchorMin.Y <= 0f)
        //    yVel2 = 1 + Random.Shared.NextSingle() / 5f;
        //MainControl.Transform.LocalRotation -= MathHelper.ToRadians((float)time.ElapsedGameTime.TotalSeconds * 20);
        base.Update(machine, game, time);
    }

    public override void Draw(StateMachine machine, Game game, GameTime time)
    {
        game.GraphicsDevice.Clear(Color.CornflowerBlue);
        base.Draw(machine, game, time);
    }

    private float Remap(float x)
    {
        return (x + 1f) * 0.5f;
    }
}