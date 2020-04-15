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
            Tetris.Renderer.BoardRenderer renderer = new Renderer.BoardRenderer();
            renderer.syncUpdateLoop();
        }
    }
}   