using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Input;

namespace Tetris
{
    /*
    테트리스는 60프레임 게임 -> 프레임당 약 16.6ms
    */
    public class TetrisGame
    {
        enum GameState
        {
            Idle,
            Playing,
            Paused,
            Dead
        }
        iInputProvider InputProvider;
        InputSetting KeySetting;

        TetrisGrid tetrisGrid;
        public IEnumerable<TetrisLine> Lines { get { return tetrisGrid.getLines; } }
        CurrentTetrominoPiece currentPiece;
        TetrominoBag tetrominoBag;
        Queue<Tetromino> next;
        readonly Point spawnOffset = new Point(4, 20);

        #region Time data about mino movement
        TimeSpan ARRDelay = TimeSpan.FromMilliseconds(100);
        TimeSpan DASDelay = TimeSpan.FromMilliseconds(250);
        TimeSpan minoSpawnDelay = TimeSpan.FromMilliseconds(50);
        TimeSpan lastMinoPlaced = TimeSpan.Zero;
        TimeSpan lastMinoMoveTime = TimeSpan.Zero;
        TimeSpan lastDropTime = TimeSpan.Zero;
        TimeSpan LastSoftDropTime = TimeSpan.Zero;
        TimeSpan DropDelay = TimeSpan.FromMilliseconds(16);
        bool isContinousMoving = false; // 이게 True면, ARR에 의해 영향을 받는다는 뜻, 꾹누른 상태임
        #endregion

        GameState gameState = GameState.Idle;
        
        public TetrisGame(iInputProvider inputProvider, InputSetting keySetting, TetrominoBag bag = null)
        {
            this.KeySetting = keySetting;
            this.InputProvider = inputProvider;
            tetrisGrid = new TetrisGrid();
            ResetGame(bag);
        }
        public void ResetGame(TetrominoBag bag = null)
        {
            this.tetrominoBag = bag ?? new TetrominoBag();
            next = new Queue<Tetromino>(5);
            while(next.Count < 5)
                next.Enqueue(tetrominoBag.GetNext());
        }

        public void StartGame()
        {

            ResetGame();
            var firstPiece = tetrominoBag.GetNext();
            currentPiece = new CurrentTetrominoPiece(tetrisGrid, firstPiece, spawnOffset);
            var expectedPos = currentPiece.GetPosOfBlocks();
            if(tetrisGrid.CanMinoExistHere(expectedPos) == false)
            {
                throw new InvalidOperationException("Game is initialized but cannot spawn the first mino to the spawn offset!");
            }
            
            this.gameState = GameState.Playing;
        }

        public void PauseGame()
        {
            this.gameState = GameState.Paused;
        }

        public void ResumeGame(long curTick)
        {
            this.gameState = GameState.Playing;
        }

        public void Update(TimeSpan curTime)
        {
            if(this.gameState != GameState.Playing)
                return;

            if (currentPiece == null)
            {
                if (curTime - lastMinoPlaced > minoSpawnDelay)
                {
                    currentPiece = new CurrentTetrominoPiece(tetrisGrid, tetrominoBag.GetNext(), spawnOffset);
                    var expectedPos = currentPiece.GetPosOfBlocks();
                    if(tetrisGrid.CanMinoExistHere(expectedPos) == false)
                    {
                        this.gameState = GameState.Dead;
                    }
                }

                if(currentPiece == null)
                    return;
            }

            // 업데이트당 한번의 이동, 또는 스핀만 해야함?

            if(InputProvider.GetState(KeySetting.HardDrop) == KeyState.ToggledDown)
            {
                while(currentPiece.TryShift(new Point(0, -1))); // 가능한 밑까지 쭉 보냄
                
                LastSoftDropTime = curTime;
                lastMinoPlaced = curTime;
                tetrisGrid.UpdateBoard();

                currentPiece = null;
            }
            else
            {
                if(InputProvider.GetState(KeySetting.CCW) == KeyState.ToggledDown)
                {
                    var success = currentPiece.TrySpin(InputType.CCW);
                }
                else if(InputProvider.GetState(KeySetting.CW) == KeyState.ToggledDown)
                {
                    var success = currentPiece.TrySpin(InputType.CW);
                }

                //if(currentState.isTrue(InputType.LeftPressed))
                var leftState = InputProvider.GetState(KeySetting.Left);
                var rightState = InputProvider.GetState(KeySetting.Right);

                if(leftState == KeyState.ToggledDown)
                {
                    if(currentPiece.TryShift(new Point(-1, 0)))
                    {
                        lastMinoMoveTime = curTime;
                    }
                }
                else if(rightState == KeyState.ToggledDown)
                {
                    if(currentPiece.TryShift(new Point(1, 0)))
                    {
                        lastMinoMoveTime = curTime;
                    }
                }
                else if((leftState == KeyState.Down || rightState == KeyState.Down) && !(leftState == KeyState.Down && rightState == KeyState.Down))
                { // 꾹눌렀을때
                    if(leftState == KeyState.Down)
                    {
                        if(isContinousMoving)
                        { // ARR에 의해 결정
                            if(ARRDelay < curTime - lastMinoMoveTime && currentPiece.TryShift(new Point(-1, 0)))
                            {
                                lastMinoMoveTime = curTime;
                                isContinousMoving = true;
                            }
                        }
                        else
                        { // 아직 연속 이동상태가 아님
                            if(DASDelay < curTime - lastMinoMoveTime && currentPiece.TryShift(new Point(1, 0)))
                            {
                                lastMinoMoveTime = curTime;
                                isContinousMoving = true;
                            }
                        }
                    }
                }
                else
                {
                    isContinousMoving = false;
                }
                
                if(InputProvider.GetState(KeySetting.SoftDrop) == KeyState.Down)
                {
                    if(curTime - lastDropTime > DropDelay)
                    {
                        if(currentPiece.TryShift(new Point(0, -1)))
                        {
                            LastSoftDropTime = curTime;
                        }
                    }
                }
            }
            
        }

        bool TryMove(Point localMoveOffset, out Point newOffsetPos)
        {
            throw new NotImplementedException();
            /*
            var newOffset = new Point(localMoveOffset.X + currentPiece.offset.X, localMoveOffset.Y + currentPiece.offset.Y);
            var expectedPos = currentPiece.minoType.GetPos(newOffset, currentPiece.rotState);
            var success = TryShift(currentPiece.GetPos(), expectedPos, currentPiece.minoType);

            if (success)
                newOffsetPos = newOffset;
            else
                newOffsetPos = new Point();
            
            return success;
            */
        }

        bool TryShift(IEnumerable<Point> before, IEnumerable<Point> after, Tetromino mino)
        {
            throw new NotImplementedException();
            /*
            if (!after.All(p => isValidPosition(p)))
                return false;

            var canMove = !after.Any(p => tetrisGrid.Get(p));
            if(canMove)
            {
                tetrisGrid.Set(before, false, mino);
                tetrisGrid.Set(after, true, mino);
                return true;
            }
            else
                return false;
                */
        }

        bool isValidPosition(Point point)
        {
            throw new NotImplementedException();
            /*
            if (point.X < 0 || point.X > 9)
                return false;

            if (point.Y < 0 || point.Y > 23)
                return false;

            return true;
            */
        }

        bool TrySpin(InputType inputState, out Point newOffsetPos)
        {
            throw new NotImplementedException();
            /*
            var mino = currentPiece.minoType;

            if(inputState.isTrue(InputType.CCW) && inputState.isTrue(InputType.CW))
                throw new InvalidOperationException("Cannot have CCW and CW at the same time!");

            IEnumerable<Point> expectedLocalOffsetPos;

            var before = currentPiece.rotState;
            RotationState after;

            if (inputState.isTrue(InputType.CCW))      after = currentPiece.rotState.CCW();
            else if (inputState.isTrue(InputType.CW))  after = currentPiece.rotState.CW();
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
            */
        }
    }
}