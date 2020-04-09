using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Tetris
{
    /*
    테트리스는 60프레임 게임 -> 프레임당 약 16.6ms
    */
    public class TetrisGame
    {
        TetrisGrid tetrisGrid;
        iInputProvider inputProvider;
        CurrentTetrominoPiece currentPiece;
        TetrominoBag tetrominoBag;
        Queue<Tetromino> next;
        readonly Point spawnOffset = new Point(4, 20);
        
        public TetrisGame(iInputProvider inputProvider, TetrominoBag bag = null)
        {
            tetrisGrid = new TetrisGrid();
            ResetGame(bag);
            this.inputProvider = inputProvider;
        }

        public void ResetGame(TetrominoBag bag = null)
        {
            this.tetrominoBag = bag ?? new TetrominoBag();
            var firstPiece = tetrominoBag.GetNext();
            currentPiece = new CurrentTetrominoPiece(firstPiece, spawnOffset);
            next = new Queue<Tetromino>(5);
            while(next.Count < 5)
                next.Enqueue(tetrominoBag.GetNext());
        }

        public void Update(uint curMilliSecond)
        {
            var currentState = inputProvider.GetInputState();
            var mino = currentPiece.minoType;
            if(!currentState.isTrue(InputState.HardDrop))
            {
                if(currentState.isTrue(InputState.CCW) || currentState.isTrue(InputState.CW))
                {
                    if(TrySpin(currentState, out var newOffsetPos))
                        currentPiece.offset = newOffsetPos;
                }

                if(currentState.isTrue(InputState.LeftPressed))
                {
                    if(TryMove(new Point(-1, 0), out var newOffsetPos))
                        currentPiece.offset = newOffsetPos;
                }
                else if(currentState.isTrue(InputState.RightPressed))
                {
                    if(TryMove(new Point(1, 0), out var newOffsetPos))
                        currentPiece.offset = newOffsetPos;
                }
                
                if(currentState.isTrue(InputState.SoftDrop))
                {

                }
            }
            else
            {

            }
        }

        bool TryMove(Point localMoveOffset, out Point newOffsetPos)
        {
            var newOffset = new Point(localMoveOffset.X + currentPiece.offset.X, localMoveOffset.Y + currentPiece.offset.Y);
            var expectedPos = currentPiece.minoType.GetPos(newOffset, currentPiece.rotState);
            var success = TryShift(currentPiece.GetPos(), expectedPos, currentPiece.minoType);

            if (success)
                newOffsetPos = newOffset;
            else
                newOffsetPos = new Point();
            
            return success;
        }

        bool TryShift(IEnumerable<Point> before, IEnumerable<Point> after, Tetromino mino)
        {
            var canMove = !after.Any(p => tetrisGrid.Get(p));
            if(canMove)
            {
                tetrisGrid.Set(before, false);
                tetrisGrid.Set(after, true, mino);
                return true;
            }
            else
                return false;
        }

        bool TrySpin(InputState inputState, out Point newOffsetPos)
        {
            var mino = currentPiece.minoType;

            if(inputState.isTrue(InputState.CCW) && inputState.isTrue(InputState.CW))
                throw new InvalidOperationException("Cannot have CCW and CW at the same time!");

            IEnumerable<Point> expectedLocalOffsetPos;

            var before = currentPiece.rotState;
            RotationState after;

            if (inputState.isTrue(InputState.CCW))      after = currentPiece.rotState.CCW();
            else if (inputState.isTrue(InputState.CW))  after = currentPiece.rotState.CW();
            else                                        { newOffsetPos = new Point(); return false; }

            expectedLocalOffsetPos = SRS.Translation(currentPiece.minoType, before, after);

            foreach(var offsetPos in expectedLocalOffsetPos)
            {
                var expectedPos = mino.GetPos(offsetPos, currentPiece.rotState.CCW());
                bool success = TryShift(currentPiece.GetPos(), expectedPos, currentPiece.minoType);

                if(success)
                {
                    newOffsetPos = offsetPos;
                    return true;
                }
            }
            newOffsetPos = new Point();
            return false;
        }
    }
}