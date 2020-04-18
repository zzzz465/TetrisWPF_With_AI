using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Input;
using log4net;

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
        protected readonly Point spawnOffset = new Point(4, 19);

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
        public event Action BoardChanged; // unsupported yet
        public event Action LineCleared; // unsupported yet
        #endregion

        #region Game rules and states
        protected GameState gameState = GameState.Idle;
        protected uint IncomingGarbageLine = 0;
        #endregion

        
        public TetrisGame(TetrisGameSetting gameSetting, TetrominoBag bag = null)
        {
            ApplySetting(gameSetting);
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

        public abstract void StartGame();

        public abstract void PauseGame();

        public abstract void ResumeGame(TimeSpan curTime);

        public void UpdateGame(TimeSpan curTime)
        {
            PreUpdate(curTime);
            Update(curTime);
            PostUpdate(curTime);
        }
        protected virtual void PreUpdate(TimeSpan curTime) { }

        protected virtual void Update(TimeSpan curTime) { }
        protected virtual void PostUpdate(TimeSpan curTime)
        {
            if(gameState != GameState.Playing)
                return;
                
            tetrisGrid.UpdateBoard();
        }

        public abstract IEnumerable<Tetromino> PeekBag();
    }
}