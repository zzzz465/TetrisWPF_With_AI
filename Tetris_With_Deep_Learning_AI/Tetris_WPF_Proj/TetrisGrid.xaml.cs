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
        TetrisGame tetrisGame;
        iInputProvider inputProvider;

        public TetrisGrid()
        {
            InitializeComponent();
            InitializeGrid();
            InitializeGame();
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

        void InitializeGame()
        {
            UserInputManager userInputManager = new UserInputManager();
            InputSetting inputSetting = InputSetting.Default;
            userInputManager.ObserveKey(inputSetting.CCW);
            userInputManager.ObserveKey(inputSetting.CW);
            userInputManager.ObserveKey(inputSetting.Left);
            userInputManager.ObserveKey(inputSetting.Right);
            userInputManager.ObserveKey(inputSetting.Hold);
            userInputManager.ObserveKey(inputSetting.HardDrop);
            userInputManager.ObserveKey(inputSetting.SoftDrop);
            inputProvider = userInputManager;
            tetrisGame = new TetrisGame(userInputManager, inputSetting, new TetrominoBag());
        }

        public void StartGame()
        {
            this.tetrisGame.StartGame();
        }

        public void ResetGame()
        {
            this.tetrisGame.ResetGame();
        }

        public void OnUpdated(object sender, TickUpdateEventArgs e)
        {
            inputProvider.Update();
            this.tetrisGame.Update(TimeSpan.FromMilliseconds(e.currentMilliSecond));

            var lines = this.tetrisGame.Lines.ToList();

            for(int y = 0; y < 20; y++)
            {
                var curLine = lines.Count - 1 >= y ? lines[y] : new TetrisLine();
                for(int x = 0; x < 10; x++)
                {
                    var cell = cells[y * 10 + x];
                    throw new NotImplementedException("미노 타입에 맞는 색 반환 딕셔너리 추가하기");
                    /*
                    if(curLine.line[x])
                    {
                        cell.SetColor(Color.FromRgb(150, 150, 150));
                    }
                    else
                    {
                        cell.SetColor(Color.FromRgb(50, 50, 50));
                    }
                    */
                }
            }
        }
    }

    public delegate void TickUpdateEvent(object sender, TickUpdateEventArgs e);

    public class TickUpdateEventArgs : EventArgs
    {
        public double currentMilliSecond { get; private set; }
        public TickUpdateEventArgs(double currentMilliSecond)
        {
            this.currentMilliSecond = currentMilliSecond;
        }
    }
}
