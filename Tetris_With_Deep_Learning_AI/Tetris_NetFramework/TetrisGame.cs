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
        public IEnumerable<Point> PosOfCurMinoBlocks { get { return currentPiece?.GetPosOfBlocks(); } }
        CurrentTetrominoPiece currentPiece;
        TetrominoBag tetrominoBag;
        Queue<Tetromino> next;
        readonly Point spawnOffset = new Point(4, 20);

        #region Time data about mino movement
        TimeSpan ARRDelay = TimeSpan.FromMilliseconds(100);
        TimeSpan DASDelay = TimeSpan.FromMilliseconds(250);
        TimeSpan minoSpawnDelay = TimeSpan.FromMilliseconds(400);
        TimeSpan lastMinoPlaced = TimeSpan.Zero;
        TimeSpan lastMinoMoveTime = TimeSpan.Zero;
        TimeSpan lastDropTime = TimeSpan.Zero;
        TimeSpan LastSoftDropTime = TimeSpan.Zero;
        TimeSpan DropDelay = TimeSpan.FromMilliseconds(150);
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
                lockCurrentMinoToPlace();
                tetrisGrid.UpdateBoard();
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
                            if(DASDelay < curTime - lastMinoMoveTime && currentPiece.TryShift(new Point(-1, 0)))
                            {
                                lastMinoMoveTime = curTime;
                                isContinousMoving = true;
                            }
                        }
                    }
                    else if(rightState == KeyState.Down)
                    {
                        if(isContinousMoving)
                        { // ARR에 의해 결정
                            if(ARRDelay < curTime - lastMinoMoveTime && currentPiece.TryShift(new Point(1, 0)))
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

            void lockCurrentMinoToPlace()
            {
                if(currentPiece == null)
                    throw new InvalidOperationException("setMinoToPlace Method shouldn't be called when the currentPiece is null...");

                var PosOfCurrentPiece = currentPiece.GetPosOfBlocks();
                var isValid = tetrisGrid.CanMinoExistHere(PosOfCurrentPiece);
                if(!isValid)
                    throw new Exception("Unexpected behaviour of currentPiece, currentPiece's current pos should always valid");

                tetrisGrid.Set(PosOfCurrentPiece, true, currentPiece.minoType);

                currentPiece = null;
            }
        }
    }
}