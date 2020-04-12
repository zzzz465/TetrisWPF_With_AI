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
using System.Windows.Shapes;
using System.Timers;
using Tetris;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// TetrisGrid.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TetrisGrid : UserControl
    {
        static Color BackgroundColor = Color.FromRgb(50, 50, 50);
        List<Cell> cells = new List<Cell>();
        TetrisGame tetris = new TetrisGame(Tetris.InputManager.Instance, new TetrominoBag());

        public TetrisGrid()
        {
            InitializeComponent();
            InitializeGrid();

            //테스트
            var timer = new System.Timers.Timer();
            timer.Interval = 1;
            timer.Elapsed += this.OnUpdated;
        }

        void InitializeGrid()
        {
            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    var cell = new Cell(BackgroundColor);
                    Grid_Root.Children.Add(cell);
                    cell.SetValue(Grid.RowProperty, 19 - y);
                    cell.SetValue(Grid.ColumnProperty, 9 - x);
                    cells.Add(cell);
                }
            }
        }

        public void OnUpdated(object sender, EventArgs e)
        {
            uint curTick; // = (e as TickUpdateEventArgs).currentTick;
            if (e is TickUpdateEventArgs eTick)
                curTick = eTick.currentTick;
            else if(e is ElapsedEventArgs ee)
                CursorType = ee.SignalTime.
            tetris.Update(curTick);
        }
    }

    public class TickUpdateEventArgs : EventArgs
    {
        public uint currentTick { get; private set; }
        public TickUpdateEventArgs(uint currentTick)
        {
            this.currentTick = currentTick;
        }
    }
}
