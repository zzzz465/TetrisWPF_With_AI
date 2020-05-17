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
    /// TetrominoPreview.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TetrominoPreview : UserControl
    {
        public Tetromino CurTetromino
        {
            get { return (Tetromino)GetValue(CurTetrominoProperty); }
            set { SetValue(CurTetrominoProperty, value); }
        }

        public ResourceDictionary MinoStyles
        {
            get { return (ResourceDictionary)GetValue(MinoStylesProperty); }
            set { SetValue(MinoStylesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurTetromino.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurTetrominoProperty =
            DependencyProperty.Register("CurTetromino", typeof(Tetromino), typeof(TetrominoPreview), new PropertyMetadata(Tetromino.None, OnCurTetrominoChangedCallback));

        // Using a DependencyProperty as the backing store for MinoStyles.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinoStylesProperty =
            DependencyProperty.Register("MinoStyles", typeof(ResourceDictionary), typeof(TetrominoPreview), new PropertyMetadata(null, OnMinoStylesChangedCallback));

        List<Cell> cells = new List<Cell>();
        readonly Dictionary<Tetromino, (float x, float y)[]> keyPoints = new Dictionary<Tetromino, (float x, float y)[]>()
        { // grid는 4x3 짜리 (가로x세로)
            { Tetromino.None, new (float x, float y)[0] },
            { Tetromino.I, new (float x, float y)[] { (0, 0.5f), (1, 0.5f), (2, 0.5f), (3, 0.5f) } },
            { Tetromino.J, new (float x, float y)[] { (0.5f, 0), (0.5f, 1), (1.5f, 0), (2.5f, 0) } },
            { Tetromino.L, new (float x, float y)[] { (0.5f, 0), (1.5f, 0), (2.5f, 0), (2.5f, 1) } },
            { Tetromino.Z, new (float x, float y)[] { (0.5f, 1), (1.5f, 1), (1.5f, 0), (2.5f, 0) } },
            { Tetromino.S, new (float x, float y)[] { (0.5f, 0), (1.5f, 0), (1.5f, 1), (2.5f, 1) } },
            { Tetromino.O, new (float x, float y)[] { (1, 0), (1, 1), (2, 0), (2, 1) } },
            { Tetromino.T, new (float x, float y)[] { (0.5f, 0), (1.5f, 0), (2.5f, 0), (1.5f, 1) } }
        };

        readonly double cellLength = 100;

        public TetrominoPreview()
        {
            InitializeComponent();

            for (int i = 0; i < 4; i++)
            {
                var cell = new Cell();
                RootCanvas.Children.Add(cell);
                cells.Add(cell);
            }
            ReAllocateCellSizeAndPosition();
        }

        protected void ReAllocateCellSizeAndPosition()
        {
            foreach(var cell in cells)
            {
                cell.Width = cellLength;
                cell.Height = cellLength;
                var cellPos = cell.CellPos;
                Canvas.SetLeft(cell, cellPos.X * cellLength);
                Canvas.SetBottom(cell, cellPos.Y * cellLength);
            }
        }

        static void OnCurTetrominoChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Tetromino newTetromino)
            {
                var preview = obj as TetrominoPreview;
                var keyPoints = preview.keyPoints[newTetromino];
                if (keyPoints != null || keyPoints.Length > 0)
                {
                    foreach (var pair in Enumerable.Zip(preview.cells, keyPoints, (x, y) => (cell: x, keypoint: y)))
                    {
                        pair.cell.Visibility = Visibility.Visible;
                        pair.cell.CellPos = new Point(pair.keypoint.x, pair.keypoint.y);
                        pair.cell.curTetromino = newTetromino;
                    }
                    preview.ReAllocateCellSizeAndPosition();
                }
                else
                {
                    /*
                    foreach (var cell in preview.cells)
                        cell.Visibility = Visibility.Hidden;
                        */
                }
            }
            else
                throw new Exception();
        }

        static void OnMinoStylesChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ResourceDictionary newStyleDictionary)
            {
                var preview = obj as TetrominoPreview;
                preview.cells.ForEach(cell => cell.MinoStyles = newStyleDictionary);
            }
            else
                throw new Exception();
        }
    }
}
