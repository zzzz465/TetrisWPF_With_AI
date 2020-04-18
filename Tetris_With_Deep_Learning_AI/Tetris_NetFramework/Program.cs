using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace Tetris
{
    class Program
    {
        static ILog Log = LogManager.GetLogger("Program");
        [STAThread]
        static void Main(string[] args)
        {
            Log.Info("Starting Program");
            Tetris.Renderer.BoardRenderer renderer = new Renderer.BoardRenderer();
            renderer.syncUpdateLoop();
        }
    }
}   