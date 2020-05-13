using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using ColdClear;
using Tetris.Renderer;
using Tetris.AudioModule;

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
            // AIVersusTest();
            AudioTest();
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

        static void AudioTest()
        {
            
            var testSound = new CachedSound("./TestAudio.wav");
            for(int i = 0; i < 5; i++)
            {
                
                System.Threading.Thread.Sleep(200);
            }

            
        }
    }
}   