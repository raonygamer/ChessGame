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
    public class UITestState : State
    {
        public CanvasNode Canvas = new();
        public ImageNode MainControl = new();

        public UITestState(Game game)
        {
            var tex = new Texture2D(game.GraphicsDevice, 64, 64);
            tex.SetData(Enumerable.Repeat(Color.Black, tex.Width * tex.Height).ToArray());
            MainControl.Texture = tex;
            MainControl.Transform.Parent = Canvas.Transform;
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

        public override void Update(StateMachine machine, Game game, GameTime time)
        {
            const float mult = 2f;
            var cos = Remap((float)Math.Cos(time.TotalGameTime.TotalSeconds * mult));
            var sin = Remap((float)Math.Sin(time.TotalGameTime.TotalSeconds * mult));
            //MainControl.Transform.Anchor = new Vector2(cos, sin);
            //MainControl.Transform.Origin = new Vector2(cos, sin);
            MainControl.Transform.Anchor = new Vector2(0.5f, 0.5f);
            MainControl.Transform.Origin = new Vector2(0.5f, 0.5f);
            base.Update(machine, game, time);
        }

        public override void Draw(StateMachine machine, Game game, GameTime time)
        {
            game.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(machine, game, time);
        }

        float Remap(float x)
        {
            return (x + 1f) * 0.5f;
        }
    }
}
