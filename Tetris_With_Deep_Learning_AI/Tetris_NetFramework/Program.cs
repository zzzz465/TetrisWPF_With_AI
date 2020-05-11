using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using ColdClear;
using Tetris.Renderer;

namespace Tetris
{
    class Program
    {
        static ILog Log = LogManager.GetLogger("Program");
        [STAThread]
        static void Main(string[] args)
        {
            Log.Info("Starting Program");
            
            // TestPlayerTetrisGame();
            AIVersusTest();

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

        static void TestPlayerTetrisGame()
        {
            var PlayerSetting = TetrisGameSetting.Default;
            var inputManager = new UserInputManager(InputSetting.Default);
            var playerTetrisGame = new PlayerTetrisGame(inputManager, InputSetting.Default, PlayerSetting);
            var renderer = new BoardRenderer(playerTetrisGame, null, inputManager);
            renderer.syncUpdateLoop();
        }

        static void AIVersusTest()
        {
            var FastAISetting = new AIGameSetting(TimeSpan.FromMilliseconds(8), TimeSpan.FromMilliseconds(16), TimeSpan.FromMilliseconds(80), TimeSpan.FromMilliseconds(14));
            var AIPlayer_1 = new AITetrisGame(ColdClear.ColdClearAI.CreateInstance(), FastAISetting);
            var AIPlayer_2 = new AITetrisGame(ColdClear.ColdClearAI.CreateInstance(), FastAISetting);
            AIPlayer_1.SetApponent(AIPlayer_2);
            AIPlayer_2.SetApponent(AIPlayer_1);
            var renderer = new BoardRenderer(AIPlayer_1, AIPlayer_2);
            renderer.syncUpdateLoop();
        }
    }
}   