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
    class MouseControl : DrawableGameComponent
    {
        #region komponenten

        public struct sAbstand
        {
            public int left;
            public int right;
            public int bottom;
            public int top;
        }


        public event EventHandler ZoomIn;
        public event EventHandler ZoomOut;
        public event EventHandler Drag;
        

        public sAbstand abstand;
        public int mausradval;

        public bool catchzoom = false;

        public Vector2 drag;

        public bool leftbtndown = false;

        public int MousePosX { get; set; }
        public int MousePosY { get; set; }

        public Vector2 MousePosRel;

        public int mapheight { get; set; }
        public int mapwidth { get; set; }


        #endregion

        #region konstruktor
        public MouseControl(Game game): base(game)
        {
            mausradval = 0;
            MousePosRel = Vector2.Zero;
            drag = Vector2.Zero;
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MouseState mousestate = Mouse.GetState();
            MousePosRel.X = mousestate.X - abstand.left;
            MousePosRel.Y = mousestate.Y - abstand.top;


            MousePosX = mousestate.X;
            MousePosY = mousestate.Y;

            if (catchzoom)
            {
                Zoom();
            }

            ifdrag();

        }

        private void ifdrag()
        {
            MouseState mouse = Mouse.GetState();
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (drag == Vector2.Zero)
                {
                    drag = new Vector2(mouse.X, mouse.Y);
                }
                else
                {
                    drag = 2*(new Vector2(mouse.X, mouse.Y) - drag);
                    OnDrag(EventArgs.Empty);
                    drag = Vector2.Zero;
                }
            }
            else
            {
                drag = Vector2.Zero;
            }
        }

        private void Zoom()
        {
            MouseState mousestate = Mouse.GetState();
            if (MousePosX > abstand.left && MousePosX < abstand.left + mapwidth && MousePosY > abstand.top && MousePosY < abstand.top + mapheight)
            {
                if (mousestate.ScrollWheelValue < mausradval)
                {
                    OnZoomOut(EventArgs.Empty);
                }
                else
                {
                    if (mousestate.ScrollWheelValue > mausradval)
                    {
                        OnZoomIn(EventArgs.Empty);
                    }
                }
            }
            mausradval = mousestate.ScrollWheelValue;
            
        }

        protected virtual void OnZoomIn(EventArgs e)
        {
            EventHandler myEvent = ZoomIn;
            if (myEvent != null)
            {
                myEvent(this, e);
            }
        }
        protected virtual void OnZoomOut(EventArgs e)
        {
            EventHandler myEvent = ZoomOut;
            if (myEvent != null)
            {
                myEvent(this, e);
            }
        }

        protected virtual void OnDrag(EventArgs e)
        {
            EventHandler myEvent = Drag;
            if (myEvent != null)
            {
                myEvent(this, e);
            }
        }


      
       

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
