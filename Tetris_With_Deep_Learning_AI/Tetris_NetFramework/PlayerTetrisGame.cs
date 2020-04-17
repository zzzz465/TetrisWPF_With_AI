using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Tetris
{
    public class PlayerTetrisGame : TetrisGame
    {
        iInputProvider inputProvider;
        InputSetting inputSetting;
        public PlayerTetrisGame(iInputProvider inputProvider, InputSetting keySetting, TetrisGameSetting gameSetting, TetrominoBag bag = null) : base(gameSetting, bag)
        {
            this.inputProvider = inputProvider;
            this.inputSetting = keySetting;
        }

        protected override void Update(TimeSpan curTime)
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

            if(inputProvider.GetState(InputType.Hold) == KeyState.ToggledDown)
            {
                if(TrySwapHold())
                {
                    var expectedPos = currentPiece.GetPosOfBlocks();
                    if(tetrisGrid.CanMinoExistHere(expectedPos) == false)
                        this.gameState = GameState.Dead;

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
                lockCurrentMinoToPlace();
                tetrisGrid.UpdateBoard();
            }
            else
            {
                if(inputProvider.GetState(InputType.CCW) == KeyState.ToggledDown)
                {
                    var success = currentPiece.TrySpin(InputType.CCW);
                }
                else if(inputProvider.GetState(InputType.CW) == KeyState.ToggledDown)
                {
                    var success = currentPiece.TrySpin(InputType.CW);
                }

                var leftState = inputProvider.GetState(InputType.LeftPressed);
                var rightState = inputProvider.GetState(InputType.RightPressed);

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
                
                if(inputProvider.GetState(InputType.SoftDrop) == KeyState.Down)
                {
                    if(curTime - LastSoftDropTime > SoftDropDelay)
                    {
                        if(currentPiece.TryShift(new Point(0, -1)))
                        {
                            LastSoftDropTime = curTime;
                        }
                    }
                }

                if(curTime - LastSoftDropTime > autoDropDelay)
                {
                    if (currentPiece.TryShift(new Point(0, -1)))
                        LastSoftDropTime = curTime;
                }
            }
        }
    }
}