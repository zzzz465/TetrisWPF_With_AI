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
using Tetris;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// TetrisGridTestWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TetrisGridTestWindow : Window
    {
        Tetromino[] tetrominos = new Tetromino[] { Tetromino.I, Tetromino.J, Tetromino.O, Tetromino.L, Tetromino.Z, Tetromino.S, Tetromino.T };
        int lastSelected = 0;
        public TetrisGridTestWindow()
        {
            InitializeComponent();
        }

        public void OnChangeTetromino(object sender, EventArgs e)
        {
            if(int.TryParse(this.TextBox_x.Text, out var x) && int.TryParse(this.TextBox_y.Text, out var y))
            {
                TetrisGrid.ChangeCellInGrid(x, y, tetrominos[lastSelected++]);
                if (lastSelected >= tetrominos.Length)
                    lastSelected = 0;
            }
        }

        private void OnChangeCurrentTetromino(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(this.TextBox_x_curCell.Text, out var x) && int.TryParse(this.TextBox_y_curCell.Text, out var y))
            {
                TetrisGrid.ChangeCellInGrid(x, y, tetrominos[lastSelected++]);
                if (lastSelected >= tetrominos.Length)
                    lastSelected = 0;
            }
        }
    }
}
