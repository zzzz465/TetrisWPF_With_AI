using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Tetris
{
    public enum Tetromino
    {
        I, T, O, S, Z, L, J, None
    }

    public enum Movement
    {
        Left, Right, SoftDrop, HardDrop, CW, CCW
    }
    
    public interface iMinoPlacementResult
    {
        int[,] GetPos();
    }

    public interface iMinoPath
    {
        IEnumerable<Movement> movements { get; }
    }

    public struct MinoControlSequence : iMinoPath
    {
        public IEnumerable<Movement> movements { get; private set; }
        public MinoControlSequence(IEnumerable<Movement> movements)
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
                return _state++;
        }
        public static RotationState CCW(this RotationState _state)
        {
            if(_state == RotationState.Zero)
                return RotationState.L;
            else
                return _state--;
        }
    }

    public class CurrentTetrominoPiece
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
                    { RotationState.L, new Point[] { new Point(-1, 0), new Point(-1, 1), new Point(-1, 0) } }
                }},
            {
                Tetromino.T, new Dictionary<RotationState, Point[]>()
                {
                    { RotationState.Zero, new Point[] { new Point(1, 0), new Point(-1, 0), new Point(1, 0) } },
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

        public Tetromino minoType { get; private set; }
        public Point offset { get; private set; }
        RotationState rotState = RotationState.None;
        public CurrentTetrominoPiece(Tetromino minoType, Point offset)
        {
            this.minoType = minoType;
            this.offset = offset;
        }

        public CurrentTetrominoPiece(Tetromino minoType, Point offset, RotationState rotState) : this(minoType, offset)
        {
            this.rotState = rotState;
        }

        public IEnumerable<Point> GetPos()
        {
            var positionData = minoData[minoType][rotState];
            yield return offset;
            foreach(var localPos in positionData)
            {
                yield return new Point(offset.X + localPos.X, offset.Y + localPos.Y);
            }
        }
    }

    public struct Current_Hold_Tetromino_Pair
    {
        public Tetromino hold;
        public Tetromino current;
        public Current_Hold_Tetromino_Pair(Tetromino current = Tetromino.None, Tetromino hold = Tetromino.None)
        {
            this.hold = hold;
            this.current = current;
        }
    }
}