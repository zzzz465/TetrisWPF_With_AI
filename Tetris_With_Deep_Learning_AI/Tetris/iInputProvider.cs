using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris
{
    public enum InputState : uint
    {
        None = 0,
        LeftPressed = 1,
        RightPressed = 2,
        HardDrop = 4,
        SoftDrop = 8,
        CCW = 16,
        CW = 32,
        Hold = 64
    }
    public interface iInputProvider
    {
        InputState GetInputState();
    }

    public static class iInputProviderExtension
    {
        public static bool isTrue(this InputState state, InputState flag)
        {
            if(((int)state & (int)flag) != 0)
                return true;
            else
                return false;
        }
    }
}