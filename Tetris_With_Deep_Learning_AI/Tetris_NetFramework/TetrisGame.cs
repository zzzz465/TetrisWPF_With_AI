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
    public abstract class TetrisGame
    {
        protected enum GameState
        {
            Idle,
            Playing,
            Paused,
            Dead
        }
        protected TetrisGrid tetrisGrid;
        public IEnumerable<TetrisLine> Lines { get { return tetrisGrid.getLines; } }
        public IEnumerable<Point> PosOfCurMinoBlocks { get { return currentPiece?.GetPosOfBlocks(); } }
        public IEnumerable<Point> PosOfGhostMinoBlocks { get { return currentPiece?.GetExpectedHardDropPosOfBlocks(); } }
        public Tetromino curMinoType { get { return currentPiece?.minoType ?? Tetromino.None; } }
        public Tetromino HoldMinoType { get { return Hold?.minoType ?? Tetromino.None; } }
        protected TetrominoBag tetrominoBag;
        protected readonly Point spawnOffset = new Point(4, 20);

        #region Time data about mino movement
        protected TimeSpan ARRDelay = TimeSpan.FromMilliseconds(100);
        protected TimeSpan DASDelay = TimeSpan.FromMilliseconds(250);
        protected TimeSpan minoSpawnDelay = TimeSpan.FromMilliseconds(400);
        protected TimeSpan lastMinoPlaced = TimeSpan.Zero;
        protected TimeSpan lastMinoMoveTime = TimeSpan.Zero;
        protected TimeSpan lastDropTime = TimeSpan.Zero;
        protected TimeSpan LastSoftDropTime = TimeSpan.Zero;
        protected TimeSpan SoftDropDelay = TimeSpan.FromMilliseconds(70);
        protected TimeSpan autoDropDelay = TimeSpan.FromMilliseconds(750);
        protected bool isContinousMoving = false; // 이게 True면, ARR에 의해 영향을 받는다는 뜻, 꾹누른 상태임
        #endregion
        #region Current & Hold Piece Data
        protected CurrentTetrominoPiece currentPiece;
        protected CurrentTetrominoPiece Hold;
        protected bool canHold = true;
        #endregion

        #region TetrisGame Events
        // public event Action 
        #endregion
        protected GameState gameState = GameState.Idle;
        
        public TetrisGame(TetrisGameSetting gameSetting, TetrominoBag bag = null)
        {
            tetrisGrid = new TetrisGrid();
            ResetGame(bag);
        }

        public void ApplySetting(TetrisGameSetting setting)
        {
            this.ARRDelay = setting.ARRDelay;
            this.DASDelay = setting.DASDelay;
            this.minoSpawnDelay = setting.minoSpawnDelay;
            this.SoftDropDelay = setting.softDropDelay;
            this.autoDropDelay = setting.autoDropDelay;
        }
        public virtual void ResetGame(TetrominoBag bag = null)
        {
            this.tetrominoBag = bag ?? new TetrominoBag();
            tetrisGrid.Reset();
        }

        public virtual void StartGame()
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

        public virtual void PauseGame()
        {
            this.gameState = GameState.Paused;
        }

        public virtual void ResumeGame(TimeSpan curTime)
        {
            this.gameState = GameState.Playing;
            LastSoftDropTime = curTime;
        }

        public void UpdateGame(TimeSpan curTime)
        {
            PreUpdate(curTime);
            Update(curTime);
            PostUpdate(curTime);
        }
        protected virtual void PreUpdate(TimeSpan curTime) { }

        protected virtual void Update(TimeSpan curTime) { }
        protected virtual void PostUpdate(TimeSpan curTime) { }

        protected void lockCurrentMinoToPlace()
        {
            if(currentPiece == null)
                throw new InvalidOperationException("setMinoToPlace Method shouldn't be called when the currentPiece is null...");

            var PosOfCurrentPiece = currentPiece.GetPosOfBlocks();
            var isValid = tetrisGrid.CanMinoExistHere(PosOfCurrentPiece);
            if(!isValid)
                throw new Exception("Unexpected behaviour of currentPiece, currentPiece's current pos should always valid");

            tetrisGrid.Set(PosOfCurrentPiece, currentPiece.minoType);
            canHold = true;
            currentPiece = null;
        }

        protected bool TrySwapHold() // return true if swapping success, if not, return false, does not check swapped whether the "current piece" can be placed to the spawn offset or not.
        {
            if(Hold == null)
            {
                Hold = currentPiece;
                currentPiece = new CurrentTetrominoPiece(tetrisGrid, tetrominoBag.GetNext(), spawnOffset);
                canHold = false;
                return true;
            }
            
            if(canHold)
            {
                var temp = Hold;
                Hold = currentPiece;
                currentPiece = temp;
                currentPiece.ResetOffsetToSpawnOffset();
                canHold = false;
                return true;
            }
            else
                return false;
        }

        public Tetromino PeekBag(int index)
        {
            return this.tetrominoBag.Peek(index);
        }
    }
}