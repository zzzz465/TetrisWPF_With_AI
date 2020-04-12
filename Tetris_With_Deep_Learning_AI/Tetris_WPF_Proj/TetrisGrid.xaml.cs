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

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// TetrisGrid.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TetrisGrid : UserControl
    {
        List<Cell> images = new List<Cell>();
        public TetrisGrid()
        {
            InitializeComponent();

            for (int y = 0; y < 20; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    var cell = new Cell(Color.FromRgb((byte)(x * 10), (byte)(y * 10), (byte)(x * y)));
                    TetrisGrid_Grid.Children.Add(cell);
                    cell.SetValue(Grid.RowProperty, 19 - y);
                    cell.SetValue(Grid.ColumnProperty, 19 - x);
                    images.Add(cell);
                }
            }
        }
    }
}
