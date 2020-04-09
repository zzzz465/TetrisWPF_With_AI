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

            if(!currentState.isTrue(InputState.HardDrop))
            {
                if(currentState.isTrue(InputState.CCW))
                {

                }
                else if(currentState.isTrue(InputState.CW))
                {

                }

                if(currentState.isTrue(InputState.LeftPressed))
                {

                }
                else if(currentState.isTrue(InputState.RightPressed))
                {

                }
                
                if(currentState.isTrue(InputState.SoftDrop))
                {

                }
            }
            else
            {

            }
        }
    }
}