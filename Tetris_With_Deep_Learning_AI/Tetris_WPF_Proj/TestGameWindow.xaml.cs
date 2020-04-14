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
using System.Windows.Shapes;
using System.Timers;
using System.Diagnostics;
using System.Windows.Threading;
using log4net;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// TestGameWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TestGameWindow : Window
    {
        ILog Log = LogManager.GetLogger("TestGameWindow");
        DispatcherTimer globalGameTimer = new DispatcherTimer();
        Stopwatch sw = new Stopwatch();
        public TestGameWindow()
        {
            InitializeComponent();
            ResetGame();
        }

        public void ResetGame()
        {
            Log.Info("Resetting game...");
            Player1Grid.ResetGame();
            Player2Grid.ResetGame();

            globalGameTimer.Stop();
            globalGameTimer.Tick -= OnUpdate;

            globalGameTimer = new DispatcherTimer();
            globalGameTimer.Interval = TimeSpan.FromMilliseconds(1);
            globalGameTimer.Tick += OnUpdate;
        }

        public void StartGame()
        {
            Log.Info("Game started!");
            globalGameTimer.Start();
            sw.Start();
        }

        void OnUpdate(object sender, EventArgs e)
        {
            var updateEventArgs = new TickUpdateEventArgs(sw.Elapsed.TotalMilliseconds);
            Player1Grid.OnUpdated(this, updateEventArgs);
        }
    }
}
