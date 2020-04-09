using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Tetris
{
    public class InputManager : iInputProvider
    {
        public struct InputManagerSetting
        {
            //ConsoleKey
        }
        long LastARRTime;
        long LastDASTime;
        long LastSoftDropTime;
        readonly int ARR_Delay;
        readonly int DAS_Delay;
        readonly int Drop_Delay;
        /*
        public InputManager(int ARR, int DAS, int DropDelay, InputManager)
        {

        }
        */
        public InputState GetInputState()
        {
            throw new NotImplementedException();
        }
    }

}