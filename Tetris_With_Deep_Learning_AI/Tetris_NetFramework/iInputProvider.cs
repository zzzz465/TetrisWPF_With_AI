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
    public static class InputTypeExtension
    {
        public static Key ConvertInputToKey(this InputSetting inputSetting, InputType inputType)
        {
            switch(inputType)
            {
                case InputType.CCW:
                    return inputSetting.CCW;
                
                case InputType.CW:
                    return inputSetting.CW;

                case InputType.SoftDrop:
                    return inputSetting.SoftDrop;

                case InputType.HardDrop:
                    return inputSetting.HardDrop;

                case InputType.Hold:
                    return inputSetting.Hold;
                
                case InputType.LeftPressed:
                    return inputSetting.Left;
                
                case InputType.RightPressed:
                    return inputSetting.Right;

                default:
                    return Key.None;
            }
        }
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
        KeyState GetState(InputType inputType);
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