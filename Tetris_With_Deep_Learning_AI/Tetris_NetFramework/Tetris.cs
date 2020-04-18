using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Tetris
{
    public enum Tetromino : int
    {
        None = 0,
        I = 1,
        T = 2, 
        O = 4, 
        S = 8, 
        Z = 16, 
        L = 32, 
        J = 64,
        Garbage = 128
    }

    public static class TetrominoExtension
    {
        #region Tetris Block Position by offset and rotation data
        static readonly Dictionary<Tetromino, Dictionary<RotationState, Point[]>> minoData = new Dictionary<Tetromino, Dictionary<RotationState, Point[]>>()
        {
            { 
                Tetromino.I, new Dictionary<RotationState, Point[]>() 
                {
                    { RotationState.Zero, new Point[] { new Point(-1, 0), new Point(1, 0), new Point(2, 0) } },
                    { RotationState.R, new Point[] { new Point(0, 1), new Point(0, -1), new Point(0, -2) } },
                    { RotationState.Two, new Point[] { new Point(-2, 0), new Point(-1, 0), new Point(1, 0) } },
                    { RotationState.L, new Point[] { new Point(0, 2), new Point(0, 1), new Point(0, -1) } }
                }},
            {
                Tetromino.J, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(-1, 0), new Point(-1, 1), new Point(1, 0) } },
                    { RotationState.R, new Point[] { new Point(0, 1), new Point(1, 1), new Point(0, -1) } },
                    { RotationState.Two, new Point[] { new Point(1, 0), new Point(-1, 0), new Point(1, -1) } },
                    { RotationState.L, new Point[] { new Point(-1, -1), new Point(0, 1), new Point(0, -1) } }
                }},
            {
                Tetromino.L, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(-1, 0), new Point(1, 0), new Point(1, 1) } },
                    { RotationState.R, new Point[] { new Point(0, 1), new Point(0, -1), new Point(1, -1) } },
                    { RotationState.Two, new Point[] { new Point(-1, -1), new Point(-1, 0), new Point(1, 0) } },
                    { RotationState.L, new Point[] { new Point(-1, 1), new Point(0, 1), new Point(0, -1) } }
                }},
            {
                Tetromino.O, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(1, 0), new Point(0, 1), new Point(1, 1) } },
                    { RotationState.R, new Point[] { new Point(1, 0), new Point(0, 1), new Point(1, 1) } },
                    { RotationState.Two, new Point[] { new Point(1, 0), new Point(0, 1), new Point(1, 1) } },
                    { RotationState.L, new Point[] { new Point(1, 0), new Point(0, 1), new Point(1, 1) } }
                }},
            {
                Tetromino.S, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(-1, 0), new Point(0, 1), new Point(1, 1) } },
                    { RotationState.R, new Point[] { new Point(1, 0), new Point(0, 1), new Point(1, -1) } },
                    { RotationState.Two, new Point[] { new Point(1, 0), new Point(0, -1), new Point(-1, -1) } },
                    { RotationState.L, new Point[] { new Point(-1, 0), new Point(-1, 1), new Point(0, -1) } }
                }},
            {
                Tetromino.T, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(1, 0), new Point(-1, 0), new Point(0, 1) } },
                    { RotationState.R, new Point[] { new Point(1, 0), new Point(0, 1), new Point(0, -1) } },
                    { RotationState.Two, new Point[] { new Point(1, 0), new Point(0, -1), new Point(-1, 0) } },
                    { RotationState.L, new Point[] { new Point(-1, 0), new Point(0, 1), new Point(0, -1) } }
                }},
            {
                Tetromino.Z, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(-1, 1), new Point(0, 1), new Point(1, 0) } },
                    { RotationState.R, new Point[] { new Point(1, 0), new Point(0, -1), new Point(1, 1) } },
                    { RotationState.Two, new Point[] { new Point(-1, 0), new Point(0, -1), new Point(1, -1) } },
                    { RotationState.L, new Point[] { new Point(-1, 0), new Point(0, 1), new Point(-1, -1) } }
                }
            }
        };
        #endregion
        public static IEnumerable<Point> GetPos(this Tetromino mino, Point offset, RotationState rotState)
        {
            var positionData = minoData[mino][rotState];
            yield return offset;
            foreach(var localPos in positionData)
            {
                yield return offset.Add(localPos);
            }
        }
    }

    public enum Instruction
    {
        None, Left, Right, SoftDrop, HardDrop, CW, CCW, Hold, LockMino, InstructionDone, SkipToNextMino
    }
    
    public interface iMinoPlacementResult
    {
        int[,] GetPos();
    }

    public interface iMinoPath
    {
        IEnumerable<Instruction> movements { get; }
    }

    public struct MinoControlSequence : iMinoPath
    {
        public IEnumerable<Instruction> movements { get; private set; }
        public MinoControlSequence(IEnumerable<Instruction> movements)
        {
            this.movements = movements;
        }
    }

    public enum RotationState
    {
        None = -1,
        Zero = 0,
        R = 1,
        Two = 2,
        L = 3
    }
    public static class RotationStateMethod
    {
        public static RotationState CW(this RotationState _state)
        {
            if(_state == RotationState.L)
                return RotationState.Zero;
            else
                return ++_state;
        }
        public static RotationState CCW(this RotationState _state)
        {
            if(_state == RotationState.Zero)
                return RotationState.L;
            else
                return --_state;
        }
    }
}