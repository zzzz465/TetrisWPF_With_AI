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
using Tetris;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// NextMino.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NextMino : UserControl
    {
        public Brush CellBrush
        {
            get { return (Brush)GetValue(CellBrushProperty); }
            set { SetValue(CellBrushProperty, value); }
        }

        public Tetromino CurTetromino
        {
            get { return (Tetromino)GetValue(CurTetrominoProperty); }
            set { SetValue(CurTetrominoProperty, value); }
        }

        public Brush TBrush;


        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(int), typeof(NextMino), new PropertyMetadata(0));

        public static readonly DependencyProperty CurTetrominoProperty =
            DependencyProperty.Register("CurTetromino", typeof(Tetromino), typeof(NextMino), new PropertyMetadata(Tetromino.None, CurTetrominoCallBack));

        public static readonly DependencyProperty CellBrushProperty =
            DependencyProperty.Register("CellBrush", typeof(Brush), typeof(NextMino), new PropertyMetadata(null));

        public static void CurTetrominoCallBack(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public NextMino()
        {
            InitializeComponent();
        }

        public void SetTetromino(Tetromino tetromino)
        {

        }

    }
}
