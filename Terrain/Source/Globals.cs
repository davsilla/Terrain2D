using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Terrain
{
    public enum DrawMode
    {
        NoiseMap,
        ColorMap,
    }

    class Globals
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;

        public static int SCREEN_WIDTH = 900;
        public static int SCREEN_HEIGHT = 600;
    }
}
