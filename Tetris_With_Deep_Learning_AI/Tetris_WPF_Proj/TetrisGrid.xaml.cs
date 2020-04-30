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
        /*
        width 10 height 23짜리 grid
        
        */
        static System.Windows.Media.Color BackgroundColor = System.Windows.Media.Color.FromRgb(50, 50, 50);
        Cell[] curPieceCells;
        Cell[] cells;
        CellColorSet cellColorSet = CellColorSet.GetDafultCellColorSet();

        public TetrisGrid()
        {
            InitializeComponent();
            InitializeGrid();
        }

        void InitializeGrid()
        {
            cells = new Cell[10 * 23];
            int RectSideLength = (int)(RootCanvas.Width / 10);
            
            for (int y = 0; y < 23; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    var cell = new Cell(BackgroundColor);
                    cell.Width = RectSideLength;
                    cell.Height = RectSideLength;
                    cells[x + y * 10] = cell;
                    GridCanvas.Children.Add(cell);
                    Canvas.SetZIndex(cell, 1);
                    Canvas.SetBottom(cell, y * RectSideLength);
                    Canvas.SetLeft(cell, x * RectSideLength);
                }
            }
            

            curPieceCells = new Cell[4];
            for (int i = 0; i < 4; i++)
            {
                var cell = new Cell(BackgroundColor);
                cell.Width = RectSideLength;
                cell.Height = RectSideLength;
                curPieceCells[i] = cell;
                GridCanvas.Children.Add(cell);
                Canvas.SetZIndex(cell, 2);
            }
        }

        public void DrawGame(TetrisGame tetrisGame)
        {
            DrawGrid(tetrisGame.Lines.ToList());
            DrawCurMinoPiece(tetrisGame.PosOfCurMinoBlocks?.Select(p => new System.Windows.Point(p.X, p.Y)).ToList(), tetrisGame.curMinoType);
        }

        void DrawGrid(List<TetrisLine> lines)
        {
            for (int y = 0; y < 23; y++)
            {
                var curLine = y < lines.Count ? lines[y] : new TetrisLine();
                for (int x = 0; x < 10; x++)
                {
                    var cell = cells[y * 10 + x];
                    var cellColor = cellColorSet.GetColor(curLine.line[x]);
                    cell.SetColor(cellColor);
                }
            }
        }

        void DrawCurMinoPiece(List<Point> curMinoBlocksPos, Tetromino minoType)
        {
            if(curMinoBlocksPos != null && curMinoBlocksPos.Count > 0)
            {
                int RectSideLength = (int)(RootCanvas.Width / 10);
                int i = 0;
                var cellColor = cellColorSet.GetColor(minoType);
                foreach(var p in curMinoBlocksPos)
                {
                    var cell = curPieceCells[i];
                    cell.SetColor(cellColor);
                    cell.Visibility = Visibility.Visible;
                    Canvas.SetLeft(cell, p.X * RectSideLength);
                    Canvas.SetBottom(cell, p.Y * RectSideLength);
                    i++;
                }
            }
            else
            {
                for(int i = 0; i < 4; i++)
                {
                    var cell = curPieceCells[i];
                    cell.Visibility = Visibility.Hidden;
                }
            }
        }
    }
}
