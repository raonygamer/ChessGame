using Core;
using Core.Nodes;
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
    public class UITestState2 : State
    {
        public SpriteNode Sprite;
        public CanvasNode Canvas;
        public ImageNode MainControl;
        public ImageNode SecondControl;
        public ImageNode ThirdControl;
        public ImageNode FourthControl;
        public ImageNode FifthControl;

        public UITestState2(Game game)
        {
            var quad = new Texture2D(game.GraphicsDevice, 10, 10);
            quad.SetData(Enumerable.Repeat(Color.Red, 10 * 10).ToArray());
            Canvas = new CanvasNode();
            
            var tex = new Texture2D(game.GraphicsDevice, 128, 128);
            tex.SetData(Enumerable.Repeat(Color.Red, tex.Width * tex.Height).ToArray());
            var tex2 = new Texture2D(game.GraphicsDevice, 64, 64);
            tex2.SetData(Enumerable.Repeat(Color.Green, tex2.Width * tex2.Height).ToArray());
            MainControl = new();
            MainControl.Texture = tex;
            MainControl.Transform.Parent = Canvas.Transform;
            MainControl.Transform.Anchor = Vector2.One / 2f;
            MainControl.Transform.Origin = new Vector2(0.5f, 0.5f);

            SecondControl = new();
            SecondControl.Texture = tex2;
            SecondControl.Transform.Parent = MainControl.Transform;
            SecondControl.Transform.Anchor = Vector2.Zero;
            SecondControl.Transform.Origin = new Vector2(1, 1);

            ThirdControl = new();
            ThirdControl.Texture = tex2;
            ThirdControl.Transform.Parent = MainControl.Transform;
            ThirdControl.Transform.Anchor = new Vector2(1, 0);
            ThirdControl.Transform.Origin = new Vector2(0, 1);

            FourthControl = new();
            FourthControl.Texture = tex2;
            FourthControl.Transform.Parent = MainControl.Transform;
            FourthControl.Transform.Anchor = new Vector2(0, 1);
            FourthControl.Transform.Origin = new Vector2(1, 0);

            FifthControl = new();
            FifthControl.Texture = tex2;
            FifthControl.Transform.Parent = MainControl.Transform;
            FifthControl.Transform.LocalPosition = FifthControl.Transform.Size / 2f;
            FifthControl.Transform.Anchor = new Vector2(1, 1);
            FifthControl.Transform.Origin = new Vector2(0.5f, 0.5f);

            Sprite = new SpriteNode
            {
                Texture = tex2,
                Color = Color.Green,
                LayerDepth = 0.5f,
            };
            Sprite.Transform.LocalScale = new Vector2(0.1f, 0.1f);
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
            MainControl.Transform.LocalPosition = new Vector2((float)Math.Cos(time.TotalGameTime.TotalSeconds * 5) * 64f, (float)Math.Sin(time.TotalGameTime.TotalSeconds * 5) * 64f);
            MainControl.Transform.LocalRotation = MathHelper.ToRadians((float)time.TotalGameTime.TotalSeconds * 20);
            SecondControl.Transform.LocalPosition = new Vector2((float)Math.Cos(time.TotalGameTime.TotalSeconds * 15) * 16f, (float)Math.Sin(time.TotalGameTime.TotalSeconds * 15) * 16f);
            ThirdControl.Transform.LocalPosition = new Vector2(0, (float)Math.Sin(time.TotalGameTime.TotalSeconds * 15) * 16f);
            FourthControl.Transform.LocalPosition = new Vector2((float)Math.Cos(time.TotalGameTime.TotalSeconds * 15) * 16f, 0);
            FifthControl.Transform.LocalRotation = MathHelper.ToRadians((float)-time.TotalGameTime.TotalSeconds * 40);
            base.Update(machine, game, time);
        }

        public override void Draw(StateMachine machine, Game game, GameTime time)
        {
            game.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(machine, game, time);
        }
    }
}
