using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Terrain
{
    class SimpleMouseInput
    {
        MouseState state;
        MouseState prevState;

        public SimpleMouseInput()
        {
            state = Mouse.GetState();
            prevState = state;
        }

        public Point GetPosition()
        {
            return Mouse.GetState().Position;
        }

        public bool IsInRect(Vector2 position, Vector2 size)
        {
            Point pos = GetPosition();
            return (pos.X >= position.X && pos.X <= position.X + size.X) && (pos.Y >= position.Y && pos.Y <= position.Y + size.Y);
        }

        public bool IsInRect(Rectangle rect)
        {
            Point pos = GetPosition();
            return (pos.X >= rect.X && pos.X <= rect.X + rect.Width) && (pos.Y >= rect.Y && pos.Y <= rect.Y + rect.Height);
        }

        public bool BtnHeld(int mouseBtn)
        {
            if (mouseBtn < 0 || mouseBtn > 1)
                throw new ArgumentException("BtnClicked: input must be 0: right mouse btn or 1: left mouse btn");

            if (prevState.LeftButton == state.LeftButton || prevState.RightButton == state.RightButton)
            {
                return mouseBtn == 0 ? state.LeftButton == ButtonState.Pressed : state.RightButton == ButtonState.Pressed;
            }

            return false;
        }

        public bool BtnClicked(int mouseBtn)
        {
            if (mouseBtn < 0 || mouseBtn > 1)
                throw new ArgumentException("BtnClicked: input must be 0: right mouse btn or 1: left mouse btn");

            if (prevState.LeftButton != state.LeftButton || prevState.RightButton != state.RightButton)
            {
                return mouseBtn == 0 ? state.LeftButton == ButtonState.Released : state.RightButton == ButtonState.Released;
            }

            return false;
        }
        public void UpdateNewState()
        {
            state = Mouse.GetState();
        }

        public void UpdatePrevState()
        {
            prevState = state;
        }

    }
}
