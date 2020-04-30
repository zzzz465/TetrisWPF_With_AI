using System;
using System.Collections.Generic;
using System.Linq;
using MisaMinoNET;
using Tetris;

namespace MisaminoAI
{
    public class MisaMinoREPL
    {
        bool shouldExit = false;
        public void Loop()
        {
            while(shouldExit == false)
            {
                var line = Console.ReadLine();
                var data = REPLSerializer.Deserialize(line);
                
                int[] queue = data.bag.Select(i => TetrominoToInt(i)).ToArray();
                int current = TetrominoToInt(data.current);
                int? hold = data.hold != Tetromino.None ? (int?)TetrominoToInt(data.hold) : null;
                int height = data.field.Count();
                int[,] field = new int[10 * height];
            }
        }

        int TetrominoToInt(Tetromino mino)
        {
            switch(mino)
            {
                case Tetromino.Z: return 0;
                case Tetromino.S: return 1;
                case Tetromino.L: return 2;
                case Tetromino.J: return 3;
                case Tetromino.T: return 4;
                case Tetromino.O: return 5;
                case Tetromino.I: return 6;
                default:          return -1;
            }
        }

        int[,] ConvertFieldToArr(IEnumerable<TetrisLine> lines)
        {
            int height = lines.Count();
            int[,] arr = new int[10, height];
            for(int i = 0; i < height; i++)
            {
                
            }
        }
    }
}