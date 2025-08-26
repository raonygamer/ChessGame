using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace States
{
    public class UITestState : State
    {
        public SpriteNode Sprite;
        public CanvasNode Canvas;
        public ControlNode MainControl;
        public ControlNode SecondControl;
        public ControlNode ThirdControl;
        public ControlNode FourthControl;
        public ControlNode FifthControl;

        public UITestState(Game game)
        {
            var quad = new Texture2D(game.GraphicsDevice, 10, 10);
            quad.SetData(Enumerable.Repeat(Color.Red, 10 * 10).ToArray());
            Canvas = new CanvasNode();
            
            var tex = new Texture2D(game.GraphicsDevice, 1, 1);
            tex.SetData(new[] { Color.Red });
            MainControl = new ControlNode();
            MainControl.Texture = tex;
            MainControl.Size = new Vector2(128, 128);
            MainControl.SetParent(Canvas);
            MainControl.Anchor = Vector2.One / 2f;
            MainControl.Transform.Origin = new Vector2(0.5f, 0.5f);
            MainControl.Transform.Scale = Vector2.One * 1.4f;

            SecondControl = new ControlNode();
            SecondControl.Texture = tex;
            SecondControl.Size = new Vector2(64, 64);
            SecondControl.SetParent(MainControl);
            SecondControl.Anchor = Vector2.Zero;
            SecondControl.Transform.Origin = new Vector2(1, 1);
            SecondControl.Color = Color.Blue;

            ThirdControl = new ControlNode();
            ThirdControl.Texture = tex;
            ThirdControl.Size = new Vector2(64, 64);
            ThirdControl.SetParent(MainControl);
            ThirdControl.Anchor = new Vector2(1, 0);
            ThirdControl.Transform.Origin = new Vector2(0, 1);
            ThirdControl.Color = Color.Blue;

            FourthControl = new ControlNode();
            FourthControl.Texture = tex;
            FourthControl.Size = new Vector2(64, 64);
            FourthControl.SetParent(MainControl);
            FourthControl.Anchor = new Vector2(0, 1);
            FourthControl.Transform.Origin = new Vector2(1, 0);
            FourthControl.Color = Color.Blue;

            FifthControl = new ControlNode();
            FifthControl.Texture = tex;
            FifthControl.Size = new Vector2(64, 64);
            FifthControl.SetParent(MainControl);
            FifthControl.Anchor = new Vector2(1, 1);
            FifthControl.Transform.Origin = new Vector2(0.5f, 0.5f);
            FifthControl.Transform.Position = new Vector2(32, 32);
            FifthControl.Color = Color.Blue;

            Sprite = new SpriteNode
            {
                Texture = tex,
                Color = Color.Green,
                LayerDepth = 0.5f,
            };
            Sprite.Transform.Scale = new Vector2(4, 4);
            Sprite.Transform.Origin = new Vector2(0.5f, 0.5f);
            AddNode(Canvas);
            AddNode(MainControl);
            AddNode(SecondControl);
            AddNode(ThirdControl);
            AddNode(FourthControl);
            AddNode(FifthControl);
            AddNode(Sprite);
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
            MainControl.Transform.Position = new Vector2((float)Math.Cos(time.TotalGameTime.TotalSeconds * 5) * 64f, (float)Math.Sin(time.TotalGameTime.TotalSeconds * 5) * 64f);
            MainControl.Transform.Rotation = MathHelper.ToRadians((float)time.TotalGameTime.TotalSeconds * 20);
            SecondControl.Transform.Position = new Vector2((float)Math.Cos(time.TotalGameTime.TotalSeconds * 15) * 16f, (float)Math.Sin(time.TotalGameTime.TotalSeconds * 15) * 16f);
            ThirdControl.Transform.Position = new Vector2(0, (float)Math.Sin(time.TotalGameTime.TotalSeconds * 15) * 16f);
            FourthControl.Transform.Position = new Vector2((float)Math.Cos(time.TotalGameTime.TotalSeconds * 15) * 16f, 0);
            FifthControl.Transform.Rotation = MathHelper.ToRadians((float)-time.TotalGameTime.TotalSeconds * 40);
            base.Update(machine, game, time);
        }

        public override void Draw(StateMachine machine, Game game, GameTime time)
        {
            game.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(machine, game, time);
        }
    }
}
