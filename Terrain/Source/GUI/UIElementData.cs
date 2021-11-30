using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Terrain
{
    class UIElementData
    {
        public Texture2D Texture { get; set; }
        public Rectangle Rect { get; set; }
        public string Text { get; set; }

        public UIElementData(string text, Rectangle rect, Color bgColor)
        {
            Text = text;
            Rect = rect;

            Color[] colorMap = new Color[rect.Width * rect.Height];
            for (int i = 0; i < colorMap.Length; i++)
            {
                colorMap[i] = bgColor;
            }

            Texture = TextureGenerator.TextureFromColorMap(rect.Width, rect.Height, colorMap);
        }

        public UIElementData(Texture2D backgroundTexture, string text, Rectangle rect)
        {
            Text = text;
            Rect = rect;
            Texture = backgroundTexture;
        }
    }
}
