using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var inputManager = new InputManager(InputManager.Setting.Default);
            while(true)
            {
                var curInputState = inputManager.GetInputState();
                if(curInputState.isTrue(InputState.CCW))
                    Console.WriteLine("CCW");

                if(curInputState.isTrue(InputState.LeftPressed))
                    Console.WriteLine("Left");

                if(curInputState.isTrue(InputState.RightPressed))
                    Console.WriteLine("Right");
            }
        }
    }
}