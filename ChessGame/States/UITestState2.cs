using System;
using System.Linq;
using Core;
using Core.Nodes.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace States;

public class UITestState2 : State
{
    public CanvasNode Canvas;
    public ImageNode FifthControl;
    public ImageNode FourthControl;
    public ImageNode MainControl;
    public ImageNode SecondControl;
    public ImageNode ThirdControl;

    public UITestState2(Game game) : base(game)
    {
        var quad = new Texture2D(game.GraphicsDevice, 10, 10);
        quad.SetData(Enumerable.Repeat(Color.Red, 10 * 10).ToArray());
        Canvas = new CanvasNode();

        var tex = new Texture2D(game.GraphicsDevice, 128, 128);
        tex.SetData(Enumerable.Repeat(Color.Red, tex.Width * tex.Height).ToArray());
        var tex2 = new Texture2D(game.GraphicsDevice, 64, 64);
        tex2.SetData(Enumerable.Repeat(Color.Green, tex2.Width * tex2.Height).ToArray());
        MainControl = new ImageNode();
        MainControl.Texture = tex;
        MainControl.Transform.Parent = Canvas.Transform;
        MainControl.Transform.AnchorMin = Vector2.One / 2f;
        MainControl.Transform.Pivot = new Vector2(0.5f, 0.5f);

        SecondControl = new ImageNode();
        SecondControl.Texture = tex2;
        SecondControl.Transform.Parent = MainControl.Transform;
        SecondControl.Transform.AnchorMin = Vector2.Zero;
        SecondControl.Transform.Pivot = new Vector2(1, 1);

        ThirdControl = new ImageNode();
        ThirdControl.Texture = tex2;
        ThirdControl.Transform.Parent = MainControl.Transform;
        ThirdControl.Transform.AnchorMin = new Vector2(1, 0);
        ThirdControl.Transform.Pivot = new Vector2(0, 1);

        FourthControl = new ImageNode();
        FourthControl.Texture = tex2;
        FourthControl.Transform.Parent = MainControl.Transform;
        FourthControl.Transform.AnchorMin = new Vector2(0, 1);
        FourthControl.Transform.Pivot = new Vector2(1, 0);

        FifthControl = new ImageNode();
        FifthControl.Texture = tex2;
        FifthControl.Transform.Parent = MainControl.Transform;
        FifthControl.Transform.Position = FifthControl.Transform.Size / 2f;
        FifthControl.Transform.AnchorMin = new Vector2(1, 1);
        FifthControl.Transform.Pivot = new Vector2(0.5f, 0.5f);

        AddNode(Canvas);
        AddNode(MainControl);
        AddNode(SecondControl);
        AddNode(ThirdControl);
        AddNode(FourthControl);
        AddNode(FifthControl);
    }

    public override void OnEnter(StateMachine machine, Game game)
    {
        base.OnEnter(machine, game);
    }

    public override void OnExit(StateMachine machine, Game game)
    {
        base.OnExit(machine, game);
    }

    public override void Update(StateMachine machine, Game game, GameTime time)
    {
        var halfWidth = game.Window.ClientBounds.Width / 2;
        var halfHeight = game.Window.ClientBounds.Height / 2;
        MainControl.Transform.Position = new Vector2((float)Math.Cos(time.TotalGameTime.TotalSeconds * 5) * 64f,
            (float)Math.Sin(time.TotalGameTime.TotalSeconds * 5) * 64f);
        MainControl.Transform.Rotation = MathHelper.ToRadians((float)time.TotalGameTime.TotalSeconds * 20);
        SecondControl.Transform.Position = new Vector2((float)Math.Cos(time.TotalGameTime.TotalSeconds * 15) * 16f,
            (float)Math.Sin(time.TotalGameTime.TotalSeconds * 15) * 16f);
        ThirdControl.Transform.Position =
            new Vector2(0, (float)Math.Sin(time.TotalGameTime.TotalSeconds * 15) * 16f);
        FourthControl.Transform.Position =
            new Vector2((float)Math.Cos(time.TotalGameTime.TotalSeconds * 15) * 16f, 0);
        FifthControl.Transform.Rotation = MathHelper.ToRadians((float)-time.TotalGameTime.TotalSeconds * 40);
        base.Update(machine, game, time);
    }

    public override void Draw(StateMachine machine, Game game, GameTime time)
    {
        game.GraphicsDevice.Clear(Color.CornflowerBlue);
        base.Draw(machine, game, time);
    }
}