using Core;
using Core.Nodes.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace States;

public class UITestState : State
{
    public CanvasNode Canvas = new();
    public FixedNode MainControl = new();

    public UITestState(Game game) : base(game)
    {
        var tex = new Texture2D(game.GraphicsDevice, 256, 256);
        tex.SetData(Enumerable.Repeat(Color.Black, tex.Width * tex.Height).ToArray());

        var tex2 = new Texture2D(game.GraphicsDevice, 16, 16);
        tex2.SetData(Enumerable.Repeat(new Color(0f, 0f, 0f, 0.3f), tex2.Width * tex2.Height).ToArray());
        
        var font = game.Content.Load<SpriteFont>("Fonts/vcr");

        MainControl.Size = new Point(300, 300);
        Canvas.AddChild(MainControl);

        AddNode(Canvas);
        AddNode(MainControl);
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