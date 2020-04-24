using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using ColdClear;

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
            

            /*
            ColdClear.ColdClear CC = ColdClear.ColdClear.CreateInstance();
            CC.CleanReset(new TetrominoBag());
            //CC.Reset(new bool[400], false, 0);
            CC.NotifyGridChanged(new List<TetrisLine>(), 0, false);
            CCPiece[] pieces = new CCPiece[5] { CCPiece.CC_I, CCPiece.CC_L, CCPiece.CC_J, CCPiece.CC_O, CCPiece.CC_S };
            foreach(var mino in pieces)
            {
                CC.AddMino(mino.ToTetromino());
            }

            CC.RequestNextInstructionSet(2);
            System.Threading.Thread.Sleep(10);
            var result = CC.TryGetInstructionSet(out var _);
            if(result)
                Log.Info("Success");
            else
                Log.Info("Fail");
            */
        }
    }
}   