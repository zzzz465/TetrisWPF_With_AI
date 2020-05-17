using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using log4net;

namespace Tetris
{
    public class PlayerTetrisGame : TetrisGame
    {
        protected override ILog Log { get; set; } = LogManager.GetLogger("PlayerTetrisGame");
        iInputProvider inputProvider;
        InputSetting inputSetting;
        public PlayerTetrisGame(iInputProvider inputProvider, InputSetting keySetting, TetrisGameSetting gameSetting) : base(gameSetting)
        {
            this.inputProvider = inputProvider;
            this.inputSetting = keySetting;
        }

        public override void InitializeGame()
        {
            base.InitializeGame();
        }

        public override void StartGame()
        {
            var firstPiece = tetrominoBag.GetNext();
            currentPiece = new CurrentTetrominoPiece(tetrisGrid, firstPiece, spawnOffset);
            var expectedPos = currentPiece.GetPosOfBlocks();
            if(tetrisGrid.CanMinoExistHere(expectedPos) == false)
            {
                throw new InvalidOperationException("Game is initialized but cannot spawn the first mino to the spawn offset!");
            }
            
            this.gameState = GameState.Playing;
        }

        public override void PauseGame()
        {
            this.gameState = GameState.Paused;
        }

        public override void ResumeGame(TimeSpan curTime)
        {
            this.gameState = GameState.Playing;
            LastSoftDropTime = curTime;
        }

        protected override void Update(TimeSpan curTime)
        {
            if(this.gameState != GameState.Playing)
                return;

            if (currentPiece == null)
            {
                if(TryCreateCurrentPiece(curTime) == false)
                    return;
            }

            if(inputProvider.GetState(InputType.Hold) == KeyState.ToggledDown)
            {
                if(TrySwapHold())
                {
                    var expectedPos = currentPiece.GetPosOfBlocks();
                    if(tetrisGrid.CanMinoExistHere(expectedPos) == false)
                        this.gameState = GameState.Dead;

                    _TetrisGameEvent.OnHold(new EventArgs());
                    return;
                }
            }

            // 업데이트당 한번의 이동, 또는 스핀만 해야함?

            // TODO : 홀드 추가하기

            if(inputProvider.GetState(InputType.HardDrop) == KeyState.ToggledDown)
            {
                while(currentPiece.TryShift(new Point(0, -1))); // 가능한 밑까지 쭉 보냄
                
                LastSoftDropTime = curTime;
                lastMinoPlaced = curTime;
                lockCurrentMinoToPlace(curTime);
                _TetrisGameEvent.OnCurMinoHardDropped(new EventArgs());
            }
            else
            {
                if(inputProvider.GetState(InputType.CCW) == KeyState.ToggledDown)
                {
                    var success = currentPiece.TrySpin(InputType.CCW);
                    _TetrisGameEvent.OnCurMinoRotated(new EventArgs());
                }
                else if(inputProvider.GetState(InputType.CW) == KeyState.ToggledDown)
                {
                    var success = currentPiece.TrySpin(InputType.CW);
                    _TetrisGameEvent.OnCurMinoRotated(new EventArgs());
                }

                var leftState = inputProvider.GetState(InputType.LeftPressed);
                var rightState = inputProvider.GetState(InputType.RightPressed);

                if(leftState == KeyState.ToggledDown)
                {
                    if(currentPiece.TryShift(new Point(-1, 0)))
                    {
                        lastMinoMoveTime = curTime;
                        _TetrisGameEvent.OnCurMinoMoved(new EventArgs());
                    }
                }
                else if(rightState == KeyState.ToggledDown)
                {
                    if(currentPiece.TryShift(new Point(1, 0)))
                    {
                        lastMinoMoveTime = curTime;
                        _TetrisGameEvent.OnCurMinoMoved(new EventArgs());
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
                                _TetrisGameEvent.OnCurMinoMoved(new EventArgs());
                            }
                        }
                        else
                        { // 아직 연속 이동상태가 아님
                            if(DASDelay < curTime - lastMinoMoveTime && currentPiece.TryShift(new Point(-1, 0)))
                            {
                                lastMinoMoveTime = curTime;
                                isContinousMoving = true;
                                _TetrisGameEvent.OnCurMinoMoved(new EventArgs());
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
                                _TetrisGameEvent.OnCurMinoMoved(new EventArgs());
                            }
                        }
                        else
                        { // 아직 연속 이동상태가 아님
                            if(DASDelay < curTime - lastMinoMoveTime && currentPiece.TryShift(new Point(1, 0)))
                            {
                                lastMinoMoveTime = curTime;
                                isContinousMoving = true;
                                _TetrisGameEvent.OnCurMinoMoved(new EventArgs());
                            }
                        }
                    }
                }
                else
                {
                    isContinousMoving = false;
                }
                
                if(inputProvider.GetState(InputType.SoftDrop) == KeyState.Down)
                {
                    if(curTime - LastSoftDropTime > SoftDropDelay)
                    {
                        if(currentPiece.TryShift(new Point(0, -1)))
                        {
                            LastSoftDropTime = curTime;
                            _TetrisGameEvent.OnSoftDropped(new EventArgs());
                            // _TetrisGameEvent.OnCurMinoMoved(new EventArgs()); FIXME : 
                        }
                    }
                }

                if(curTime - LastSoftDropTime > autoDropDelay)
                {
                    if (currentPiece.TryShift(new Point(0, -1)))
                    {
                        LastSoftDropTime = curTime;
                        _TetrisGameEvent.OnSoftDropped(new EventArgs());
                        // _TetrisGameEvent.OnCurMinoMoved(new EventArgs()); FIXME :
                    }
                }
            }
        }

        bool TrySwapHold() // return true if swapping success, if not, return false, does not check swapped whether the "current piece" can be placed to the spawn offset or not.
        {
            if(curHold == null)
            {
                curHold = currentPiece;
                currentPiece = new CurrentTetrominoPiece(tetrisGrid, tetrominoBag.GetNext(), spawnOffset);
                canHold = false;
                return true;
            }
            
            if(canHold)
            {
                var temp = curHold;
                curHold = currentPiece;
                currentPiece = temp;
                currentPiece.ResetToInitialState();
                canHold = false;
                return true;
            }
            else
                return false;
        }

        public override IEnumerable<Tetromino> PeekBag()
        {
            List<Tetromino> peekBag = new List<Tetromino>();
            for(int i = 0; i < 5; i++)
            {
                var nextMino = tetrominoBag.Peek(i);
                peekBag.Add(nextMino);
            }

            return peekBag;
        }
    }
}