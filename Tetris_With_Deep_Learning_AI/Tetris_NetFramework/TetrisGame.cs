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
        protected abstract ILog Log { get; set; }
        protected enum GameState
        {
            Idle,
            OnInitialize,
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
        TetrisGame opponentGame;
        protected int incomingGarbageLine = 0;
        protected int MaxGarbageLinePerBlockLock = 6;
        [Obsolete("Not implemented feature")]
        protected bool ProcessGarbageLineWhileCombo = false;
        Random garbageLineSelector = new Random();
        bool wasBlockLocked = false;
        protected bool GarbageLineReceived { get; private set; }
        bool GridChanged = false;
        protected int Combo { get; private set; }
        protected bool B2B { get; private set; }
        protected TSpinType tSpinType { get; private set; }
        iGarbageLineCalculator garbageLineCalculator;
        #endregion

        
        public TetrisGame(TetrisGameSetting gameSetting, TetrominoBag bag = null, iGarbageLineCalculator garbageLineCalculator = null)
        {
            ApplySetting(gameSetting);
            tetrisGrid = new TetrisGrid();
            this.garbageLineCalculator = garbageLineCalculator ?? new FallbackGarbageLineCalculator();
            ResetGame(bag);
        }

        public void SetLogName(string name)
        {
            this.Log = LogManager.GetLogger(name);
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
        public virtual void InitializeGame()
        {

        }

        public virtual void StartGame()
        {
            if(gameState != GameState.OnInitialize)
                throw new Exception("should call InitializeGame before calling StartGame");
            
            gameState = GameState.Playing;
        }

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

            tetrisGrid.UpdateBoard(out var gridUpdateResult);
            
            if(GridChanged)
            {
                ProcessGridChange(gridUpdateResult);
                GridChanged = false;
            }

            if(wasBlockLocked)
            {
                ProcessIncomingGarbageLine();
                wasBlockLocked = false;
            }
        }

        public void SetApponent(TetrisGame opponent)
        {
            if(opponent == this)
                throw new ArgumentException("Cannot set self as opponent!");
            this.opponentGame = opponent;
        }

        public void SendDamageToOpponent(int garbageLine)
        {
            if(garbageLine < 0)
                throw new ArgumentOutOfRangeException($"invalid argument : garbageLine {garbageLine}");

            if(this.opponentGame != null)
            {
                this.opponentGame.incomingGarbageLine += garbageLine;
            }
            else
            {
                Log.Warn($"Tried to send garbage line (amount : {garbageLine}) but opponent is not configured");
            }
        }

        void ProcessGridChange(GridUpdateResult gridUpdateResult)
        {
            var isPerfectClear = gridUpdateResult.isPerfectClear;
            var lineDeleted = gridUpdateResult.LineDeleted;

            if(lineDeleted > 0)
            {
                if(isPerfectClear)
                {
                    Log.Debug("Perfect Clear!");
                    int garbageLine = garbageLineCalculator.CalculatePerfectClear(lineDeleted, this.tSpinType, this.B2B, this.Combo);
                    SendDamageToOpponent(garbageLine);
                }
                else
                {
                    int garbageLine = garbageLineCalculator.Calculate(lineDeleted, this.tSpinType, this.B2B, this.Combo);
                    Log.Debug($"Sending Garbage line {garbageLine} to opponent lineDeleted : {lineDeleted}, TspinState : {this.tSpinType}, B2B : {this.B2B}, combo : {this.Combo}");
                    SendDamageToOpponent(garbageLine);
                }
                
                if(this.tSpinType != TSpinType.None || lineDeleted == 4)
                    B2B = true;
                else
                    B2B = false;

                this.Combo += 1;
            }
            else
            {
                this.Combo = 0;
            }

            GridChanged = false;
        }


        void ProcessIncomingGarbageLine()
        {
            GarbageLineReceived = false;

            if(this.incomingGarbageLine <= 0)
                return;

            // if(ProcessGarbageLineWhileCombo == false && Combo <= 0)
            //     return;

            var x_index_of_garbage_line = garbageLineSelector.Next(0, 9);

            if(incomingGarbageLine > MaxGarbageLinePerBlockLock)
            {
                tetrisGrid.AddGarbageLine(MaxGarbageLinePerBlockLock, x_index_of_garbage_line);
                incomingGarbageLine -= MaxGarbageLinePerBlockLock;
                if(incomingGarbageLine < 0)
                    incomingGarbageLine = 0;
            }
            else
            {
                tetrisGrid.AddGarbageLine(incomingGarbageLine, x_index_of_garbage_line);
                incomingGarbageLine = 0;
            }

            GarbageLineReceived = true;
        }

        protected virtual bool TryCreateCurrentPiece(TimeSpan curTime)
        {
            if(curTime - lastMinoPlaced > minoSpawnDelay)
            {
                var mino = this.tetrominoBag.GetNext();
                currentPiece = new CurrentTetrominoPiece(tetrisGrid, mino, spawnOffset);
                var expectedPos = currentPiece.GetPosOfBlocks();
                if(tetrisGrid.CanMinoExistHere(expectedPos))
                    return true;
                else
                {
                    Log.Info("Cannot spawn mino, set game state to Dead");
                    this.gameState = GameState.Dead;
                    return false;
                }
            }
            else
                return false;
        }

        protected virtual void lockCurrentMinoToPlace()
        {
            if(currentPiece == null)
                throw new InvalidOperationException("setMinoToPlace Method shouldn't be called when the currentPiece is null...");

            var PosOfCurrentPiece = currentPiece.GetPosOfBlocks();
            var isValid = tetrisGrid.CanMinoExistHere(PosOfCurrentPiece);
            if(!isValid)
                throw new Exception("Unexpected behaviour of currentPiece, currentPiece's current pos should always valid");

            tetrisGrid.Set(PosOfCurrentPiece, currentPiece.minoType);
            this.tSpinType = currentPiece.tSpinType;
            canHold = true;
            currentPiece = null;
            GridChanged = true;

            wasBlockLocked = true;
        }

        public abstract IEnumerable<Tetromino> PeekBag();
    }
}