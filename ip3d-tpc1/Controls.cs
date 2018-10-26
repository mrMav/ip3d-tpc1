﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ip3d_tpc1
{
    /// <summary>
    /// Basic controls wrapper for Monogame Input
    /// Self explanatory
    /// </summary>
    public static class Controls
    {
        public static Keys Forward = Keys.W;
        public static Keys Backward = Keys.S;
        public static Keys StrafeLeft = Keys.A;
        public static Keys StrafeRight = Keys.D;

        public static Keys CameraRotateYCW = Keys.Right;
        public static Keys CameraRotateYCCW = Keys.Left;
        public static Keys CameraRotateXCW = Keys.Up;
        public static Keys CameraRotateXCCW = Keys.Down;

        public static KeyboardState LastKeyboardState;
        public static KeyboardState CurrKeyboardState;

        public static MouseState LastMouseState;
        public static MouseState CurrMouseState;

        public static void Initilalize()
        {
            LastKeyboardState = Keyboard.GetState();
            CurrKeyboardState = Keyboard.GetState();

            LastMouseState = Mouse.GetState();
            CurrMouseState = Mouse.GetState();

        }

        public static void UpdateCurrentStates()
        {
            CurrKeyboardState = Keyboard.GetState();
            CurrMouseState = Mouse.GetState();
        }

        public static void UpdateLastStates()
        {
            LastKeyboardState = CurrKeyboardState;
            LastMouseState = CurrMouseState;
        }

        public static bool IsKeyDown(Keys key)
        {

            return CurrKeyboardState.IsKeyDown(key);

        }

        public static bool IsKeyPressed(Keys key)
        {

            return LastKeyboardState.IsKeyUp(key) && CurrKeyboardState.IsKeyDown(key);

        }

    }
}