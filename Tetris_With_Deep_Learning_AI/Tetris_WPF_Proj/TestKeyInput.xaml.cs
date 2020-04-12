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
using System.Windows.Threading;
using System.Diagnostics;
using Tetris;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// TestKeyInput.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TestKeyInput : Window
    {
        
        DispatcherTimer timer;
        Stopwatch sw = new Stopwatch();
        TimeSpan lastUpdateTime = TimeSpan.FromSeconds(0);
        int maxWidth;
        Tetris.InputManager.Setting Setting
        {
            get
            {
                return inputManager.setting;
            }
            set
            {
                inputManager.setting = value;
            }
        }
        Tetris.InputManager inputManager;
        public TestKeyInput()
        {
            InitializeComponent();
            sw.Start();
            inputManager = new Tetris.InputManager(Tetris.InputManager.Setting.Default);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(0);
            timer.Tick += OnTickUpdate;
            timer.Start();
            maxWidth = (int)CCW_Rect.Width;
        }

        void OnTickUpdate(object sender, EventArgs e)
        {
            var curState = inputManager.GetInputState();

            var green = Color.FromRgb(0, 255, 0);
            var black = Color.FromRgb(0, 0, 0);

            var greenBrush = new SolidColorBrush(green);
            var blackBrush = new SolidColorBrush(black);

            CCW_Rect.Fill = curState.isTrue(InputState.CCW) ? greenBrush : blackBrush;

            CCW_Rect.Fill = curState.isTrue(InputState.CW) ? greenBrush : blackBrush;

            CCW_Rect.Fill = curState.isTrue(InputState.HardDrop) ? greenBrush : blackBrush;

            SoftDrop_Rect.Fill = curState.isTrue(InputState.SoftDrop) ? greenBrush : blackBrush;
            Left_Rect.Fill = curState.isTrue(InputState.LeftPressed) ? greenBrush : blackBrush;
            Right_Rect.Fill = curState.isTrue(InputState.RightPressed) ? greenBrush : blackBrush;
            Hold_Rect.Fill = curState.isTrue(InputState.Hold) ? greenBrush : blackBrush;

            lastUpdateTime = sw.Elapsed;
        }
    }
}
