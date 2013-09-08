using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace InterfaceXNA
{
    class Button:DrawableGameComponent
    {
        #region komponenten

        public event EventHandler Clicked;

        public SpriteBatch SpriteBatch {get; set;}
        public Texture2D Texture { get; set; }
        public Texture2D TextureSel { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }
        public SpriteEffects Effect { get; set; }
        public Microsoft.Xna.Framework.Rectangle? SourceRectangle { get; set; }
        public Microsoft.Xna.Framework.Color Color { get; set; }
        public float Rotation { get; set; }
        public float LayerDepth { get; set; }
        public Color color { get; set; }
        public bool pressed { get; set; }
        public bool IsVisible { get; set; }

        #endregion

        #region konstruktor
        public Button(Game game):base(game)
        {
            SpriteBatch = new SpriteBatch(game.GraphicsDevice);

            Position = Vector2.Zero;
            SourceRectangle = null;
            color = Color.White;
            Scale = Vector2.One;
            IsVisible = false;
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            buttoncheck();
            base.Update(gameTime);
        }

        protected virtual void OnClicked(EventArgs e)
        {
            EventHandler myEvent = Clicked;
            if (myEvent != null)
            {
                myEvent(this, e);
            }
        }

        public void buttoncheck()
        {
            MouseState mouseState = Mouse.GetState();
            if (!buttonpressed())
            {
                if (pressed == true)
                {
                    pressed = false;
                    if (mouseState.LeftButton != ButtonState.Pressed && IsVisible == true)
                    {
                        OnClicked(EventArgs.Empty);
                    }
                }
            }
            else
            {
                pressed = true;
            }
        }

        public bool buttonpressed()
        {
            MouseState mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (System.Windows.Forms.Cursor.Position.X > Position.X && System.Windows.Forms.Cursor.Position.X < Position.X + Texture.Width)
                {
                    if( System.Windows.Forms.Cursor.Position.Y > Position.Y &&  System.Windows.Forms.Cursor.Position.Y < Position.Y + Texture.Height){
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch.Begin();
            if (IsVisible)
            {
                if (pressed)
                {
                    SpriteBatch.Draw(TextureSel, Position, SourceRectangle, color, Rotation, Origin, Scale, Effect, LayerDepth);
                }
                else
                {
                    SpriteBatch.Draw(Texture, Position, SourceRectangle, color, Rotation, Origin, Scale, Effect, LayerDepth);
                }
            }
            
            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
