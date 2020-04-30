using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Windows.Shapes;
using System.Timers;
using System.Threading;
using System.Diagnostics;
using Tetris;
using log4net;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// TetrisGame.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TetrisGameUserControl : UserControl
    {
        ILog Log = LogManager.GetLogger("x");
        Stopwatch sw = new Stopwatch();
        TetrisGame tetrisGame;
        UserInputManager UserInputManager;
        Task GameUpdateTask;
        public TetrisGameUserControl()
        {
            InitializeComponent();
            sw.Reset();
        }

        public void SetTetrisGame(TetrisGame tetrisGame, UserInputManager userInputManager)
        {
            this.tetrisGame = tetrisGame;
            this.UserInputManager = userInputManager;
        }

        public void StartNewGame()
        {
            this.tetrisGame.ResetGame();
            tetrisGame.InitializeGame();
            tetrisGame.StartGame();
            GameUpdateTask?.Dispose();

            sw.Start();
            /*
            var work = new Thread(() =>
            {
                Stopwatch sw2 = new Stopwatch();
                sw2.Start();
                while (true)
                {
                    UserInputManager.Update();
                    this.tetrisGame.UpdateGame(sw2.Elapsed);
                }
            });
            work.SetApartmentState(ApartmentState.STA);
            work.Start();
            */

            CompositionTarget.Rendering += OnUpdate;
        }

        public void PauseGame()
        {
            sw.Stop();
        }

        void OnUpdate(object sender, EventArgs e)
        {
            this.UserInputManager.Update();
            tetrisGame.UpdateGame(sw.Elapsed);
            TetrisGrid.DrawGame(tetrisGame);
            //UpdateLayout();
        }
    }
}
