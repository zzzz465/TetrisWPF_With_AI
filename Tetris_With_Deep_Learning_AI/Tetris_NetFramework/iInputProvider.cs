using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Tetris
{
    public enum InputType : uint
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
    public enum KeyState : uint
    {
        NotAvailable = 0,
        Up = 1,
        Down = 2,
        ToggledDown = 4,
        ToggledUp = 8,
    }
    public interface iInputProvider
    {
        void Update();
        KeyState GetState(Key key);
    }

    public static class iInputProviderExtension
    {
        public static bool isTrue(this InputType state, InputType flag)
        {
            if(((int)state & (int)flag) != 0)
                return true;
            else
                return false;
        }
    }
}