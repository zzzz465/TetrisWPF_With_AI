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
using log4net;
using System.Diagnostics;
using System.Windows.Diagnostics;
using System.Runtime.CompilerServices;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// Cell.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Cell : UserControl
    {
        ILog Log;
        #region MinoStyle

        public Brush CellBrush
        {
            get { return (Brush)GetValue(CellBrushProperty); }
            set { SetValue(CellBrushProperty, value); }
        }
        public Tetromino curTetromino
        {
            get
            {
                return (Tetromino)GetValue(curTetrominoProperty);
            }
            set
            {
                SetValue(curTetrominoProperty, value);
            }
        }
        public ResourceDictionary MinoStyles 
        {
            get { return (ResourceDictionary)GetValue(MinoStylesProperty); } 
            set 
            {
                SetValue(MinoStylesProperty, value);
            }
        }

        public static readonly DependencyProperty CellBrushProperty = DependencyProperty.Register("CellBrush", typeof(Brush), typeof(Cell), new PropertyMetadata(null));
        public static readonly DependencyProperty curTetrominoProperty = DependencyProperty.Register("curTetromino", typeof(Tetromino), typeof(Cell), new PropertyMetadata(Tetromino.None, TetrominoChangeCallBack));
        public static readonly DependencyProperty MinoStylesProperty = DependencyProperty.Register("MinoStyle", typeof(ResourceDictionary), typeof(Cell), new PropertyMetadata());
        #endregion

        public int RectLength { get; set; } = 20;
        public Point CellPos { get; set; } = new Point(0, 0);

        public Cell()
        {
            InitializeComponent();
        }
        /*
        public Cell(ResourceDictionary minoStyles) : this()
        {
            this.MinoStyles = minoStyles;
            Log = LogManager.GetLogger("Cell");
            //SetTetromino(Tetromino.None);
            curTetromino = Tetromino.None;
        }
        */

        private static void TetrominoChangeCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cell = d as Cell;
            if (cell == null || cell.MinoStyles == null)
                return;

            var tetromino = (Tetromino)e.NewValue;
            if(cell.MinoStyles.Contains(tetromino))
            {
                var brush = cell.MinoStyles[tetromino];
                cell.CellBrush = brush as Brush;
            }
        }
    }
}
