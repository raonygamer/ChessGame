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
    public class MainState : State
    {
        public Sprite Quad;
        public Sprite Aim;

        public MainState(Game game)
        {
            var quad = new Texture2D(game.GraphicsDevice, 10, 10);
            quad.SetData(Enumerable.Repeat(Color.Red, 10 * 10).ToArray());
            Quad = new Sprite() 
            { 
                Texture = quad,
                Origin = new Vector2(0.5f, 0.5f)
            };
            Aim = new Sprite
            {
                Texture = game.Content.Load<Texture2D>("Pieces/white-pawn"),
                Origin = new Vector2(0.5f, 0.5f)
            };
            Aim.Transform.Scale = Vector2.One * 1f;
            AddNode(Aim);
            AddNode(Quad);
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void Update(StateMachine machine, Game game, GameTime time)
        {
            var halfWidth = game.Window.ClientBounds.Width / 2;
            var halfHeight = game.Window.ClientBounds.Height / 2;
            const float radius = 200f;
            base.Update(machine, game, time);
            Quad.Transform.Position = new Vector2(halfWidth, halfHeight);
            //Aim.Transform.Position = new Vector2(halfWidth - 32 + radius * (float)Math.Cos(time.TotalGameTime.TotalSeconds), halfHeight - 32 + radius * (float)Math.Sin(time.TotalGameTime.TotalSeconds));
            Aim.Transform.Position = new Vector2(halfWidth, halfHeight);
            var mouse = Mouse.GetState();
            Aim.Transform.Rotation = 90f * (float)Math.PI / 180f + (float)Math.Atan2(mouse.Y - Aim.Transform.Position.Y, mouse.X - Aim.Transform.Position.X);
        }

        public override void Draw(StateMachine machine, Game game, GameTime time)
        {
            game.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(machine, game, time);
        }
    }
}
