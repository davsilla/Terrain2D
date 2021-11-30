using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace Terrain
{
    public enum UIEState
    {
        None, Hover, Click, Drag, Hold
    }

    class UIElement
    {
        public UIEState state;
        public UIElementData data;
    }

    class UIElementButton : UIElement
    {
        public UIElementButton(string text, Rectangle rect)
        {
            state = UIEState.None;
            data = new UIElementData(text, rect, Color.White);
        }
        
        public void Update(SimpleMouseInput mouse)
        {
            if (mouse.IsInRect(data.Rect))
            {
                if (mouse.BtnClicked(0))
                {
                    state = UIEState.Click;
                }
                else
                {
                    state = UIEState.Hover;
                }
            }
            else
            {
                state = UIEState.None;
            }
        }

        public void Display(SpriteFont font, Color textColor, Color bgColor)
        {
            Vector2 stringSize = font.MeasureString(data.Text);
            float x = data.Rect.X + (data.Rect.Width / 2f) - (stringSize.X / 2f);
            float y = data.Rect.Y + (data.Rect.Height / 2f) - (stringSize.Y / 2f);
            Vector2 stringPosition = new Vector2(x, y);

            if (state == UIEState.Hover)
            {
                Color tmp = textColor;
                textColor = bgColor;
                bgColor = tmp;
            }

            Globals.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            Globals.spriteBatch.Draw(data.Texture, data.Rect, bgColor);
            Globals.spriteBatch.DrawString(font, data.Text, stringPosition, textColor);
            Globals.spriteBatch.End();
        }
    }


    class UIElementSlider : UIElement
    {
        public float Max { get; set; }
        public float Min { get; set; }
        public float Value { get; set; }

        Texture2D indicatorTexture;
        Vector2 indicatorPosition;
        Vector2 indicatorSize;

        public UIElementSlider(string text, Rectangle rect, float min, float max, float value)
        {
            if (rect.Width < 20 || rect.Height < 9)
                throw new ArgumentException("Slider height must be >= 9 and width must be >= 20");

            state = UIEState.None;
            Max = max;
            Min = min;
            Value = value;

            // horizontal slider bar texture setup
            Texture2D barTexture;
            Color[] btColorMap = new Color[rect.Width * rect.Height];
            for (int x = 0; x < rect.Width; x++)
            {
                for (int y = 0; y < rect.Height; y++)
                {
                    if ((y > rect.Height / 3f && y < rect.Height * 2f / 3f)) 
                    {
                        btColorMap[y * rect.Width + x] = Color.White;
                    }
                }
            }
            barTexture = TextureGenerator.TextureFromColorMap(rect.Width, rect.Height, btColorMap);
            data = new UIElementData(barTexture, text, rect);


            // slider knob texture setup
            Texture2D sliderTexture;

            indicatorSize = new Vector2(8, rect.Height);
            indicatorPosition = new Vector2(rect.X + (value / (max - min) * rect.Width) - (indicatorSize.X / 2), rect.Y);

            Color[] stColorMap = new Color[(int)(indicatorSize.X * indicatorSize.Y)];
            for (int i = 0; i < stColorMap.Length; i++)
            {
                stColorMap[i] = Color.White;
            }
            sliderTexture = TextureGenerator.TextureFromColorMap((int)indicatorSize.X, (int)indicatorSize.Y, stColorMap);
            indicatorTexture = sliderTexture;
        }

        public void Update(SimpleMouseInput mouse)
        {
            if (mouse.IsInRect(indicatorPosition, indicatorSize))
            {
                if (mouse.BtnHeld(0))
                {
                    UpdateIndicator(mouse);
                    state = UIEState.Drag;
                }
                else
                    state = UIEState.Hover;
            }
            else if (state == UIEState.Drag && mouse.BtnHeld(0))
            {
                UpdateIndicator(mouse);
            }
            else
            {
                state = UIEState.None;
            }
        }

        private void UpdateIndicator(SimpleMouseInput mouse)
        {
            indicatorPosition.X = mouse.GetPosition().X - indicatorSize.X / 2f;
            if (indicatorPosition.X < data.Rect.X)
                indicatorPosition.X = data.Rect.X;
            else if (indicatorPosition.X > data.Rect.X + data.Rect.Width)
                indicatorPosition.X = data.Rect.X + data.Rect.Width;

            Value = ((indicatorPosition.X - data.Rect.X) / data.Rect.Width) * Max;

            if (Value < Min) Value = Min;
            else if (Value > Max) Value = Max;
        }

        public void Display(SpriteFont font, Color primaryColor, Color secondaryColor, bool displayInts = false)
        {
            string minStr = Min.ToString();
            string maxStr = Max.ToString();
            string valueStr = data.Text + ": " + ((!displayInts) ? Value.ToString(): ((int)Value).ToString());

            Vector2 minStrSize = font.MeasureString(minStr);
            Vector2 maxStrSize = font.MeasureString(maxStr);
            Vector2 valueStrSize = font.MeasureString(valueStr);

            if (state == UIEState.Hover || state == UIEState.Drag)
            {
                Color temp = secondaryColor;
                secondaryColor = primaryColor;
                primaryColor = temp;
            }

            Globals.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
            //textures
            Globals.spriteBatch.Draw(data.Texture, data.Rect, primaryColor);
            Globals.spriteBatch.Draw(indicatorTexture, indicatorPosition, secondaryColor);
            // text
            Globals.spriteBatch.DrawString(font, valueStr, new Vector2(data.Rect.X, data.Rect.Y - valueStrSize.Y), secondaryColor);
            Globals.spriteBatch.DrawString(font, minStr, new Vector2(data.Rect.X, data.Rect.Y + minStrSize.Y), primaryColor); 
            Globals.spriteBatch.DrawString(font, maxStr, new Vector2(data.Rect.X + data.Rect.Width - maxStrSize.X, data.Rect.Y + maxStrSize.Y), primaryColor);

            Globals.spriteBatch.End();
        }
    }

    class UIElementRadioButton : UIElement
    {
        public UIElementRadioButton(string text, Rectangle rect)
        {

        }
    }

    class UIElementInputField : UIElement 
    {
    }
}
