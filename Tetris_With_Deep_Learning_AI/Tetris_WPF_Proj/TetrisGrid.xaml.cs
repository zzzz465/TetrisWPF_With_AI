using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
    /// NewTetrisGrid.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TetrisGrid : UserControl
    {
        public ResourceDictionary CellStyles
        {
            get { return (ResourceDictionary)GetValue(CellStylesProperty); }
            set { SetValue(CellStylesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellStyles.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellStylesProperty =
            DependencyProperty.Register("CellStyles", typeof(ResourceDictionary), typeof(TetrisGrid), new PropertyMetadata(null, OnCellStylesChangedCallBack));

        List<Cell> Cells = new List<Cell>();
        List<Cell> CurTetrominoCells = new List<Cell>();

        private int cellLength = 0;


        public TetrisGrid()
        {
            InitializeComponent();

            InitializeGrid();
            ReAllocateCellPosition(this, SizeChangedEventArgs.Empty);
            SizeChanged += ReAllocateCellPosition;
        }

        void Test_DrawSome(object sender, EventArgs e)
        {
            var z = new Random().Next(0, 6);
            var x = new Tetromino[] { Tetromino.I, Tetromino.J, Tetromino.O, Tetromino.L, Tetromino.Z, Tetromino.S };
            for (int i = 13; i < 32; i++)
                this.Cells[i].curTetromino = x[z];
        }

        void InitializeGrid()
        {
            for(int y = 0; y < 20; y++)
            {
                for(int x = 0; x < 10; x++)
                {
                    var cell = new Cell();
                    RootCanvas.Children.Add(cell);
                    Canvas.SetZIndex(cell, 1);
                    cell.CellPos = new Point(x, y);
                    Cells.Add(cell);
                }
            }

            for(int i = 0; i < 4; i++)
            {
                var cell = new Cell();
                RootCanvas.Children.Add(cell);
                Canvas.SetZIndex(cell, 2);
                cell.Visibility = Visibility.Hidden;
                CurTetrominoCells.Add(cell);
            }
        }

        protected void ReAllocateCellPosition(object sender, EventArgs e)
        {
            this.cellLength = (int)(RootCanvas.ActualWidth / 10);
            foreach(var cell in Enumerable.Concat(Cells, CurTetrominoCells))
            {
                var pos = cell.CellPos;
                cell.Width = cellLength;
                cell.Height = cellLength;
                Canvas.SetBottom(cell, cellLength * pos.Y);
                Canvas.SetLeft(cell, cellLength * pos.X);
            }
        }

        static void OnCellStylesChangedCallBack(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var grid = obj as TetrisGrid;
            if (grid == null)
                return;

            foreach(var cell in Enumerable.Concat(grid.Cells, grid.CurTetrominoCells))
            {
                cell.MinoStyles = e.NewValue as ResourceDictionary;
            }
        }

        public void DrawGame(TetrisGame game)
        {
            DrawBoard(game);
            DrawCurTetrominoPiece(game);
        }

        void DrawBoard(TetrisGame game)
        {
            var lines = game.Lines.ToList();
            for(int y = 0; y < 20; y++)
            {
                var line = lines.Count > y ? lines[y] : new TetrisLine();
                for(int x = 0; x < 10; x++)
                {
                    var cell = Cells[y * 10 + x];
                    cell.curTetromino = line.line[x];
                }
            }
        }

        void DrawCurTetrominoPiece(TetrisGame game)
        {
            if (game.PosOfCurMinoBlocks != null)
            {
                var curMinoPos = game.PosOfCurMinoBlocks;

                foreach (var pair in Enumerable.Zip(curMinoPos, CurTetrominoCells, (x, y) => (pos: x, cell: y)))
                {
                    pair.cell.Visibility = Visibility.Visible;
                    pair.cell.CellPos = new Point(pair.pos.X, pair.pos.Y);
                    pair.cell.curTetromino = game.curMinoType;
                    Canvas.SetBottom(pair.cell, pair.cell.CellPos.Y * cellLength);
                    Canvas.SetLeft(pair.cell, pair.cell.CellPos.X * cellLength);
                }
            }
            else
            {
                foreach(var cell in CurTetrominoCells)
                {
                    cell.Visibility = Visibility.Hidden;
                }
            }
        }

        public void ChangeCellInGrid(int x, int y, Tetromino tetromino)
        { // test
            Cells[y * 10 + x].curTetromino = tetromino;
        }

        public void ChangeCurCellInGrid(int x, int y, Tetromino tetromino)
        { // test
            foreach(var cell in CurTetrominoCells)
            {
                cell.Visibility = Visibility.Visible;
                cell.CellPos = new Point(x, y);
                cell.curTetromino = tetromino;
                ReAllocateCellPosition(this, null);
            }
        }
    }
}
