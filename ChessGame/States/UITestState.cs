using Core;
using Core.Components;
using Core.Nodes.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

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
        
        MainControl.Transform.SizeMode = new SizeMode2D
        {
            X = SizeMode.Content,
            Y = SizeMode.Content
        };
        MainControl.Transform.PaddingMin = new Vector2(10f, 10f);
        MainControl.Transform.PaddingMax = new Vector2(10f, 10f);
        MainControl.Transform.AnchorMin = new Vector2(0.5f, 0.5f);
        MainControl.Transform.Pivot = MainControl.Transform.AnchorMin;
        Quad.Texture = tex;
        Quad.Transform.Parent = MainControl.Transform;
        Quad.Color = new Color(0, 0, 0, 0.5f);
        Quad.Transform.AnchorMin = new Vector2(0f, 0f);
        Quad.Transform.Size = new Vector2(200, 100);
        //Quad.Transform.AnchorMax = new Vector2(1f, 1f);
        Quad.Transform.PaddingMin = new Vector2(5f, 5f);
        Quad.Transform.PaddingMax = new Vector2(5f, 5f);
        Quad.Transform.SizeMode = new SizeMode2D
        {
            X = SizeMode.Fixed,
            Y = SizeMode.Fixed
        };

        Text.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
        Text.Transform.Parent = Quad.Transform;
        Text.Font = font;
        Text.Transform.Scale = new Vector2(0.2f, 0.2f);
        Text.Transform.AnchorMin = new Vector2(0f, 0f);
        Text.Transform.Pivot = new Vector2(0f, 0f);
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
        const float mult = 10f;
        var cos = Remap((float)Math.Cos(time.TotalGameTime.TotalSeconds * mult));
        var sin = Remap((float)Math.Sin(time.TotalGameTime.TotalSeconds * mult));

        const float margin = 10;
        //Quad.Transform.Size = new Vector2(200 + cos * 75f, 100 + sin * 75f);
        Quad.Transform.Position = new Vector2((1 - cos) * 10f, (1 - sin) * 10f);

        //Quad.Transform.MarginMin = new Vector2(margin * cos, margin * sin);
        //Quad.Transform.MarginMax = new Vector2(margin * (1 - cos), margin * (1 - sin));

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