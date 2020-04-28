using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Tetris;
using log4net;

namespace Tetris_WPF_Proj
{
    public class CellColorSet
    {
        ILog Log = LogManager.GetLogger("CellColorSet");
        Dictionary<Tetromino, Color> tetrominoColorSet;

        public static CellColorSet GetDafultCellColorSet()
        {
            var set = new CellColorSet();

            set.SetColor(Tetromino.Garbage, Color.FromRgb(100, 100, 100));
            set.SetColor(Tetromino.I, Color.FromRgb(170, 210, 15));
            set.SetColor(Tetromino.O, Color.FromRgb(10, 200, 220));
            set.SetColor(Tetromino.S, Color.FromRgb(30, 200, 50));
            set.SetColor(Tetromino.Z, Color.FromRgb(40, 50, 200));
            set.SetColor(Tetromino.L, Color.FromRgb(200, 150, 15));
            set.SetColor(Tetromino.J, Color.FromRgb(190, 40, 40));
            set.SetColor(Tetromino.T, Color.FromRgb(180, 30, 170));
            set.SetColor(Tetromino.None, Color.FromArgb(128, 0, 0, 0));

            return set;
        }

        public CellColorSet()
        {
            tetrominoColorSet = new Dictionary<Tetromino, Color>();
        }

        public Color GetColor(Tetromino mino)
        {
            if(tetrominoColorSet.TryGetValue(mino, out var value))
            {
                return value;
            }
            else
            {
                Log.Warn($"Tried to get {mino} color but it was not configured... returning default value");
                return Color.FromRgb(100, 100, 100); // gray
            }
        }

        public void SetColor(Tetromino mino, Color color)
        {
            this.tetrominoColorSet[mino] = color;
        }
    }
}
