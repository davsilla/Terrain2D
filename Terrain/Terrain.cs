using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;

namespace Terrain
{
    public class Terrain : Game
    {
        SpriteFont font;
        SimpleMouseInput mouse;

        #region GUI
        Color primary = Color.ForestGreen;
        Color secondary = Color.DarkOliveGreen;
        
        UIElementButton randSeedBtn;
        UIElementSlider octavesSlider;

        UIElementSlider lacunaritySlider;
        UIElementSlider persistenceSlider;

        UIElementSlider scaleSlider;

        #endregion

        #region TEXTURE_MAP
        Texture2D noiseTexture;
        float[] noiseMap;
        private bool updateTexture;

        #region MAP_MODIFIERS
        DrawMode drawMode = DrawMode.ColorMap;
        TerrainType[] regions = { 
            new TerrainType() { name ="water", height = 0.4f, color = Color.CornflowerBlue},
            new TerrainType() { name ="sand", height = 0.5f, color = Color.LightGoldenrodYellow},
            new TerrainType() { name ="land", height = 0.7f, color = Color.LawnGreen},
            new TerrainType() { name ="rocky", height = 0.8f, color = Color.SlateGray},
            new TerrainType() { name ="snow", height = 1f, color = Color.WhiteSmoke},
        };
 
        readonly int nMapWidth = 300;
        readonly int nMapHeight = 300;

        int nSeed = new Random().Next();
        float nScale = 50f;
        int nOctaves = 5;
        float nPercistence = 0.5f;
        float nLacunarity = 2.0f;
        Vector2 nOffset = new Vector2(50, 50);
        #endregion


        private void SetMapTexture()
        {
            noiseMap = TextureGenerator.GenerateNoiseMap(nMapWidth, nMapHeight, nSeed, nScale, nOctaves, nPercistence, nLacunarity, nOffset);

            if (drawMode == DrawMode.NoiseMap)
            {
                noiseTexture = TextureGenerator.TextureFromHeightMap(nMapWidth, nMapHeight, noiseMap);
            }
            else
            {
                Color[] colorMap = new Color[nMapWidth * nMapHeight];

                for (int x = 0; x < nMapWidth; x++)
                {
                    for (int y = 0; y < nMapHeight; y++)
                    {
                        int index = y * nMapWidth + x;
                        for (int r = 0; r < regions.Length; r++)
                        {
                            if (noiseMap[index] <= regions[r].height)
                            {
                                colorMap[index] = regions[r].color;
                                break;
                            }
                        }
                    }
                }

                noiseTexture = TextureGenerator.TextureFromColorMap(nMapWidth, nMapHeight, colorMap);
                GC.Collect(); // clean up the old noiseMap and noiseTexture
            }
        }
        #endregion

        #region INPUT_HELPERS
        KeyboardState prevState;
        KeyboardState currState;
        private bool IsKeyPressed(Keys key)
        {
            return prevState.IsKeyDown(key) && currState.IsKeyUp(key);
        }
        #endregion

        public Terrain()
        {
            Globals.graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef
            };

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Globals.graphics.PreferredBackBufferWidth = Globals.SCREEN_WIDTH;
            Globals.graphics.PreferredBackBufferHeight = Globals.SCREEN_HEIGHT;
            Globals.graphics.ApplyChanges();

            currState = prevState = Keyboard.GetState();
            drawMode = DrawMode.ColorMap;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");
            mouse = new SimpleMouseInput();

            SetMapTexture();

            octavesSlider = new UIElementSlider("Octaves", new Rectangle(Globals.SCREEN_HEIGHT + 15, 20 + 20, 270, 20), 1, 8, nOctaves);
            lacunaritySlider = new UIElementSlider("Lacunarity", new Rectangle(Globals.SCREEN_HEIGHT + 15, 80 + 20, 270, 20), 1, 10, nLacunarity);
            persistenceSlider = new UIElementSlider("Persistence", new Rectangle(Globals.SCREEN_HEIGHT + 15, 140 + 20, 270, 20), 0.2f, 2, nPercistence);
            scaleSlider = new UIElementSlider("Scale", new Rectangle(Globals.SCREEN_HEIGHT + 15, 200 + 20, 270, 20), 1, 200, nScale);
            randSeedBtn = new UIElementButton("SEED", new Rectangle(Globals.SCREEN_HEIGHT + 15, Globals.SCREEN_HEIGHT - 50, 270, 30));
        }

        protected override void Update(GameTime gameTime)
        {
            mouse.UpdateNewState();
            currState = Keyboard.GetState();

            octavesSlider.Update(mouse);
            lacunaritySlider.Update(mouse);
            persistenceSlider.Update(mouse);
            scaleSlider.Update(mouse);
            randSeedBtn.Update(mouse);

            prevState = currState;
            mouse.UpdatePrevState();

            #region CTRLS
            ////////////////////////////////////////////////////////////////////////////////////
            // seed
            if (randSeedBtn.state == UIEState.Click)
            {
                Random random = new Random(nSeed);
                nSeed = random.Next();

                updateTexture = true;
            }

            // octaves
            if (nOctaves != (int)octavesSlider.Value)
            {
                nOctaves = (int)octavesSlider.Value;

                updateTexture = true;
            }

            // lacunarity
            if (nLacunarity != lacunaritySlider.Value)
            {
                nLacunarity = lacunaritySlider.Value;

                updateTexture = true;
            }

            // persistence
            if (nPercistence != persistenceSlider.Value)
            {
                nPercistence = persistenceSlider.Value;

                updateTexture = true;
            }
            
            // scale
            if (nScale != scaleSlider.Value)
            {
                nScale = scaleSlider.Value;
                updateTexture = true;
            }
            ////////////////////////////////////////////////////////////////////////////////////
            #endregion

            if (updateTexture)
            {
                updateTexture = false;
                SetMapTexture();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.WhiteSmoke);

            Globals.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            Globals.spriteBatch.Draw(noiseTexture, new Rectangle(0, 0, Globals.SCREEN_HEIGHT, Globals.SCREEN_HEIGHT), Color.White);
            Globals.spriteBatch.DrawString(font, nSeed.ToString(), new Vector2(randSeedBtn.data.Rect.X, randSeedBtn.data.Rect.Y - 20), primary);
            Globals.spriteBatch.End();

            octavesSlider.Display(font, secondary, primary, true);
            lacunaritySlider.Display(font, secondary, primary);
            persistenceSlider.Display(font, secondary, primary);
            scaleSlider.Display(font, secondary, primary, true);
            randSeedBtn.Display(font, primary, Color.LawnGreen);

            base.Draw(gameTime);
        }
    }
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new Terrain();
            game.Run();
        }
    }
}
