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
using System.Diagnostics;
using System.Drawing.Imaging;



namespace InterfaceXNA
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        #region defvar

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        public struct sAbstand
        {
            public int left;
            public int right;
            public int bottom;
            public int top;
        }

        //Spielmenü
        bool pause;
        bool menu;

        fpsCounter fpsCounter;

        bool linkeMaustasteGedrückt;

        //für Grafikberechnungen
        int windowwidth;
        int windowheight;
        int mausposX;
        int mausposY;


        int XMAX = 1000;//Map-größe, wird wenn gezoomt teilweise nicht ganz angezeigt
        int YMAX = 1000;

        double zoom;//Zoom Standard: 1
        public double dragx;//Anzahl realer Pixel die die Map in -X-Richtung verschoben wird
        public double dragy;

        //für die Berechnungen von Zoom und Drag

        int mapwidth;
        int mapheight;

        int liniendicke;

       
        //für Benutzereingaben
        Keys lastKey = Keys.None;
        String InputText = "";

        public sAbstand abstand;

        //map
        float[,] terrain;
        Color[] mapColors;


        SpriteFont textfont;
        SpriteFont headfont;
        Button btnPause;
        Button btnSpStart;
        Button btnBeenden;
        Button btnSpFortsetzen;
        Button btnMenu;
        Button btnSpBeitreten;
        Texture2D Line;
        Texture2D Pause;
        Texture2D Pixel;
        Texture2D map;
        MouseControl mousecontrol;


        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsFixedTimeStep = false;
            graphics.SynchronizeWithVerticalRetrace = false;

            fpsCounter = new fpsCounter(this);
            Components.Add(fpsCounter);
        }

        #region Initialize
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            IsMouseVisible = true;

            graphics.ToggleFullScreen();
            windowwidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            windowheight = GraphicsDevice.PresentationParameters.BackBufferHeight;


            //Buttons
            //Schließen-Button
            btnPause = new Button(this);
            btnPause.Texture = Content.Load<Texture2D>("btnPause");
            btnPause.TextureSel = Content.Load<Texture2D>("btnPauseSel");
            btnPause.Position = new Vector2(windowwidth - 30, 0);
            btnPause.IsVisible = false;
            btnPause.Clicked += btnclose_Clicked;
            this.Components.Add(btnPause);

            //Spiel-Starten-Button
            btnSpStart = new Button(this);
            btnSpStart.Texture = Content.Load<Texture2D>("btnSpielStarten");
            btnSpStart.TextureSel = Content.Load<Texture2D>("btnSpielStartenSel");
            btnSpStart.Position = new Vector2((windowwidth / 2) - 150, 200);
            btnSpStart.IsVisible = true;
            btnSpStart.Clicked += btnSpStarten_Clicked;
            this.Components.Add(btnSpStart);

            //Beenden-Button
            btnBeenden = new Button(this);
            btnBeenden.Texture = Content.Load<Texture2D>("btnBeenden");
            btnBeenden.TextureSel = Content.Load<Texture2D>("btnBeendenSel");
            btnBeenden.Position = new Vector2((windowwidth / 2) - 150, 320);
            btnBeenden.IsVisible = true;
            btnBeenden.Clicked += btnBeenden_Clicked;
            this.Components.Add(btnBeenden);

            //Spiel-Fortsetzen-Button
            btnSpFortsetzen = new Button(this);
            btnSpFortsetzen.Texture = Content.Load<Texture2D>("btnSpielFortsetzen");
            btnSpFortsetzen.TextureSel = Content.Load<Texture2D>("btnSpielFortsetzenSel");
            btnSpFortsetzen.Position = new Vector2((windowwidth / 2) - 150, 230);
            btnSpFortsetzen.IsVisible = true;
            btnSpFortsetzen.Clicked += btnSpFortsetzen_Clicked;
            this.Components.Add(btnSpFortsetzen);

            //Menü-Button
            btnMenu = new Button(this);
            btnMenu.Texture = Content.Load<Texture2D>("btnMenu");
            btnMenu.TextureSel = Content.Load<Texture2D>("btnMenuSel");
            btnMenu.Position = new Vector2((windowwidth / 2) - 150, 290);
            btnMenu.IsVisible = true;
            btnMenu.Clicked += btnMenu_Clicked;
            this.Components.Add(btnMenu);

            //Spiel-Beitreten-Button
            btnSpBeitreten = new Button(this);
            btnSpBeitreten.Texture = Content.Load<Texture2D>("btnSpielBeitreten");
            btnSpBeitreten.TextureSel = Content.Load<Texture2D>("btnSpielBeitretenSel");
            btnSpBeitreten.Position = new Vector2((windowwidth / 2) - 150, 260);
            btnSpBeitreten.IsVisible = true;
            btnSpBeitreten.Clicked += btnSpBeitreten_Clicked;
            this.Components.Add(btnSpBeitreten);





            pause = true;
            menu = true;

            abstand.bottom = 10;
            abstand.top = 100;
            abstand.left = 10;
            abstand.right = 400;

            mapwidth = windowwidth - abstand.left - abstand.right;
            mapheight = windowheight - abstand.top - abstand.bottom;

            mousecontrol = new MouseControl(this);
            mousecontrol.abstand.bottom = abstand.bottom;
            mousecontrol.abstand.top = abstand.top;
            mousecontrol.abstand.right = abstand.right;
            mousecontrol.abstand.left = abstand.left;

            mousecontrol.mapwidth = mapwidth;
            mousecontrol.mapheight = mapheight;
            mousecontrol.ZoomIn += mousecontrol_ZoomIn;
            mousecontrol.ZoomOut += mousecontrol_ZoomOut;
            mousecontrol.Drag += mousecontrol_Drag;
            this.Components.Add(mousecontrol);

            liniendicke = 2;

            dragx = 0;
            dragy = 0;
            zoom = 1.0;

            mapColors = new Color[XMAX * YMAX];


            base.Initialize();
        }
        #endregion

        #region LoadContent
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            // TODO: use this.Content to load your game content here 
            textfont = Content.Load<SpriteFont>("textFont");
            headfont = Content.Load<SpriteFont>("HeadFont");
            Line = Content.Load<Texture2D>("blackrect");
            Pause = Content.Load<Texture2D>("Pause");
            Pixel = Content.Load<Texture2D>("pixel");



        }
        #endregion
        #region UnloadContent
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion
        #region Update

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            //Allgemeines

            mousestat();

            if (menu)
            {
                //Menü
                btnPause.IsVisible = false;
                btnSpStart.IsVisible = true;
                btnBeenden.IsVisible = true;
                btnSpFortsetzen.IsVisible = false;
                btnMenu.IsVisible = false;
                btnSpBeitreten.IsVisible = true;
            }
            else
            {
                if (pause)
                {
                    //Pause
                    btnPause.IsVisible = false;
                    btnSpStart.IsVisible = false;
                    btnBeenden.IsVisible = false;
                    btnSpFortsetzen.IsVisible = true;
                    btnMenu.IsVisible = true;
                    btnSpBeitreten.IsVisible = false;
                }
                else
                {
                    //Spiel
                    btnPause.IsVisible = true;
                    btnSpStart.IsVisible = false;
                    btnBeenden.IsVisible = false;
                    btnSpFortsetzen.IsVisible = false;
                    btnMenu.IsVisible = false;
                    btnSpBeitreten.IsVisible = false;

                    grafikber();

                    
                }
            }

            base.Update(gameTime);

        }

        public void grafikber()
        {
            //dragx = Convert.ToInt32(Math.Min(dragx, XMAX - (mapwidth * zoom)));
            //dragy = Convert.ToInt32(Math.Min(dragy, YMAX - (mapheight * zoom)));
            dragx = Math.Max(dragx, 0);
            dragy = Math.Max(dragy, 0);
        }

        public void mousestat()
        {
            //Mausstatus für allgemeine Grafik
            MouseState mouseState = Mouse.GetState();
            linkeMaustasteGedrückt = (mouseState.LeftButton == ButtonState.Pressed);
            mausposX = System.Windows.Forms.Cursor.Position.X;
            mausposY = System.Windows.Forms.Cursor.Position.Y;            

            windowwidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            windowheight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            mapwidth = windowwidth - abstand.left - abstand.right;
            mapheight = windowheight - abstand.top - abstand.bottom;

        }

        public void mousecontrol_ZoomIn(Object objSender, EventArgs e)
        {
            MouseState ms = Mouse.GetState();
            double zoomalt = zoom;
            zoom += 0.05;           
            dragx += (ms.X - abstand.left) * (zoom - zoomalt);
            dragy += (ms.Y - abstand.top) * (zoom - zoomalt);
            grafikber();
        }

        public void mousecontrol_ZoomOut(Object objSender, EventArgs e)
        {
            MouseState ms = Mouse.GetState();        
            double zoomalt = zoom;
            zoom -= 0.05;
            dragx -= (ms.X - abstand.left) * (zoomalt - zoom);
            dragy -= (ms.Y - abstand.top) * (zoomalt - zoom);
            grafikber();
        }

        public void mousecontrol_Drag(Object objSender, EventArgs e)
        {
            dragx -= mousecontrol.drag.X;
            dragy -= mousecontrol.drag.Y;
            grafikber();
        }

        public void benutzereingabe()
        {
            if (Keyboard.GetState().IsKeyUp(lastKey))
            {
                lastKey = Keys.None;
            }
            if (Keyboard.GetState().GetPressedKeys().Length > 0 && lastKey == Keys.None)
            {
                lastKey = Keyboard.GetState().GetPressedKeys()[0];
                if (lastKey == Keys.Back)
                {
                    if (InputText.Length != 0)
                        InputText = InputText.Substring(0, InputText.Length - 1);
                }
                else if (InputText.Length < 25)
                {
                    if (lastKey.GetHashCode() == 13)
                    {
                        //isbenutzereingabe = false;
                    }
                    else
                    {
                        InputText += (char)lastKey.GetHashCode();
                    }
                }
            }
        }

        //Button_Clicked-EventHandler
        public void btnclose_Clicked(Object objSender, EventArgs e)
        {
            menu = false;
            pause = true;
        }
        public void btnSpStarten_Clicked(Object objSender, EventArgs e)
        {
            menu = false;
            pause = false;

            zoom = 1;
            dragx = 0;
            dragy = 0;

            terrain = GenerateWhiteNoise(XMAX, YMAX);
            terrain = GeneratePerlinNoise(terrain, 9);//Standard(terrain,8)


            map = new Texture2D(GraphicsDevice, XMAX,YMAX, false, SurfaceFormat.Color);
            for (int x = 0; x < XMAX; x++)
            {
                for (int y = 0; y < YMAX; y++)
                {
                    mapColors[x + y * XMAX] = getcolor(x, y);
                }
            }
            map.SetData(mapColors);

            mousecontrol.catchzoom = true;


        }
        public void btnBeenden_Clicked(Object objSender, EventArgs e)
        {
            Environment.Exit(2);
        }
        public void btnSpFortsetzen_Clicked(Object objSender, EventArgs e)
        {
            menu = false;
            pause = false;
        }
        public void btnMenu_Clicked(Object objSender, EventArgs e)
        {
            menu = true;
            pause = false;
        }
        public void btnSpBeitreten_Clicked(Object objSender, EventArgs e)
        {
            menu = true;
            pause = false;
        }


        #endregion
        #region Draw

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {


            GraphicsDevice.Clear(Color.LightSeaGreen);


            // TODO: Add your drawing code here


            spriteBatch.Begin();



            //Test-Variablen
            DrawText();


            if (!menu)
            {
                //Spiel 
                DrawFields();
                DrawMap();
                DrawText();
                if (pause)
                {
                    //Pause
                    DrawPause();
                }
            }
            else
            {
                //Menu
                DrawMenu();
            }
            spriteBatch.End();


            base.Draw(gameTime);


        }

        private void DrawMenu()
        {
            spriteBatch.DrawString(headfont, "Menü", new Vector2(420, 85), Color.White);
        }

        private void DrawPause()
        {
            spriteBatch.Draw(Pause, new Rectangle(windowwidth / 2 - windowwidth / 5, 80, Convert.ToInt32(windowwidth / 2.5), windowheight - 160), Color.LightGray);
        }

        private void DrawText()
        {//Draw Text
            spriteBatch.DrawString(textfont, Convert.ToString(dragx + "   " + dragy + "   " + zoom), new Vector2(20, 45), Microsoft.Xna.Framework.Color.White);
            //spriteBatch.DrawString(textfont, Convert.ToString(2), new Vector2(20, 0), Color.White);
        }

        private void DrawFields()
        {
            //Rahmen
            spriteBatch.Draw(Line, new Rectangle(abstand.left - liniendicke, abstand.top - liniendicke, windowwidth - abstand.left - abstand.right + (2 * liniendicke), liniendicke), Color.Black);
            spriteBatch.Draw(Line, new Rectangle(abstand.left - liniendicke, abstand.top - liniendicke, liniendicke, windowheight - abstand.top - abstand.bottom + (2 * liniendicke)), Color.Black);
            spriteBatch.Draw(Line, new Rectangle(abstand.left - liniendicke, windowheight - abstand.bottom, windowwidth - abstand.left - abstand.right + (2 * liniendicke), liniendicke), Color.Black);
            spriteBatch.Draw(Line, new Rectangle(windowwidth - abstand.right, abstand.top - liniendicke, liniendicke, windowheight - abstand.top - abstand.bottom + (2 * liniendicke)), Color.Black);
        }

        public void DrawMap()
        {
            Vector2 pos = new Vector2(0, 0);
            pos = getrealCoord(pos);
            spriteBatch.Draw(map, new Rectangle(Convert.ToInt32(pos.X), Convert.ToInt32(pos.Y), mapwidth, mapheight), new Rectangle(Convert.ToInt32(dragx/zoom), Convert.ToInt32(dragy/zoom),Convert.ToInt32(mapwidth/zoom),Convert.ToInt32(mapheight/zoom)), Color.White);
        }

        public Vector2 getrealCoord(Vector2 oldcoor)//Setzt zb. den Punkt(0,0) in die linke obere Ecke des Eingerahmten Spielfeldes
        {
            Vector2 spfieldstart = new Vector2(abstand.left, abstand.top);
            Vector2 realcorrd = new Vector2();
            Vector2 dif = new Vector2(abstand.left, abstand.top);

            realcorrd = oldcoor + dif;

            return realcorrd;
        }
        #endregion

        #region WorldGenerator

        public Color getcolor(int x, int y)//Gibt zurm angegebenem Punkt je nach Höhe die passende Farbe zurück
        {
            Color backcol;

            const double waterline = 0.3;
            const double sandline = 0.37;
            const double greenlandline = 0.42;
            const double forestline = 0.5;
            const double stoneline = 0.6;
            const double snowline = 0.7;

            Color forestcol = Color.DarkGreen;
            Color stonecol = Color.Gray;
            Color snowcol = Color.White;
            Color deepwatercol = Color.DarkBlue;
            Color watercol = Color.Blue;
            Color sandcol = Color.SandyBrown;
            Color grascol = Color.Green;

            if (terrain[x, y] > waterline)
            {
                if (terrain[x, y] > sandline)
                {
                    if (terrain[x, y] > greenlandline)
                    {
                        if (terrain[x, y] > forestline)
                        {
                            if (terrain[x, y] > stoneline)
                            {
                                if (terrain[x, y] > snowline)
                                {
                                    backcol = snowcol;
                                }
                                else
                                {
                                    backcol = stonecol;
                                }
                            }
                            else
                            {
                                backcol = forestcol;
                            }
                        }
                        else
                        {
                            backcol = grascol;
                        }
                    }
                    else
                    {
                        backcol = sandcol;
                    }
                }
                else
                {
                    backcol = watercol;
                }
            }
            else
            {
                backcol = deepwatercol;
            }

            return backcol;
        }




        float[,] GenerateWhiteNoise(int width, int height)
        {
            Random random = new Random(); //Seed to 0 for testing
            float[,] noise = new float[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    noise[i, j] = (float)random.NextDouble() % 1;
                }
            }

            return noise;
        }




        float[,] GenerateSmoothNoise(float[,] baseNoise, int octave)
        {
            int width = baseNoise.GetLength(0);
            int height = baseNoise.GetLength(1);

            float[,] smoothNoise = new float[width, height];

            int samplePeriod = 2 << octave; // calculates 2 ^ k
            float sampleFrequency = 1.0f / samplePeriod;

            for (int i = 0; i < width; i++)
            {
                //calculate the horizontal sampling indices
                int sample_i0 = (i / samplePeriod) * samplePeriod;
                int sample_i1 = (sample_i0 + samplePeriod) % width; //wrap around
                float horizontal_blend = (i - sample_i0) * sampleFrequency;

                for (int j = 0; j < height; j++)
                {
                    //calculate the vertical sampling indices
                    int sample_j0 = (j / samplePeriod) * samplePeriod;
                    int sample_j1 = (sample_j0 + samplePeriod) % height; //wrap around
                    float vertical_blend = (j - sample_j0) * sampleFrequency;

                    //blend the top two corners
                    float top = Interpolate(baseNoise[sample_i0, sample_j0],
                       baseNoise[sample_i1, sample_j0], horizontal_blend);

                    //blend the bottom two corners
                    float bottom = Interpolate(baseNoise[sample_i0, sample_j1],
                       baseNoise[sample_i1, sample_j1], horizontal_blend);

                    //final blend
                    smoothNoise[i, j] = Interpolate(top, bottom, vertical_blend);
                }
            }

            return smoothNoise;
        }



        float Interpolate(float x0, float x1, float alpha)
        {
            return x0 * (1 - alpha) + alpha * x1;
        }



        float[,] GeneratePerlinNoise(float[,] baseNoise, int octaveCount)
        {
            int width = baseNoise.GetLength(0);
            int height = baseNoise.GetLength(1);

            float[, ,] smoothNoise = new float[octaveCount, width, height]; //an array of 2D arrays containing

            float persistance = 0.65f;//Standard:; 0.5f    (Zoom)

            //generate smooth noise
            float[,] dummy = new float[width, height];

            for (int i = 1; i < octaveCount; i++)//Standard: i = 1; i < octaveCount
            {
                dummy = GenerateSmoothNoise(baseNoise, i);
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < height; k++)
                    {
                        smoothNoise[i, j, k] = dummy[j, k];
                    }
                }
            }

            float[,] perlinNoise = new float[width, height];
            float amplitude = 1.0f;
            float totalAmplitude = 0.0f;

            //blend noise together
            for (int octave = octaveCount - 1; octave >= 0; octave--)
            {
                amplitude *= persistance;
                totalAmplitude += amplitude;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        perlinNoise[i, j] += smoothNoise[octave, i, j] * amplitude;
                    }
                }
            }

            //normalisation
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    perlinNoise[i, j] /= totalAmplitude;
                }
            }

            return perlinNoise;
        }





        #endregion
    }
}
