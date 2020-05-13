using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Reflection;
using Tetris;
using System.Text;

namespace XUnitTest_Tetris
{
    public class Test_TetrisGame
    {
        [Fact]
        public void TetrisGameEvent_Check()
        {
            var gameEventInstance = new TetrisGameEvent();
            var gameEventInterface = gameEventInstance as iTetrisGameEvent;

            bool[] shouldAllTrue = Enumerable.Repeat<bool>(false, 6).ToArray();
            Action<int> assertAction = (index) => shouldAllTrue[index] = true;
            gameEventInterface.BoardChanged += (sender, e) => assertAction(0);
            gameEventInterface.LineCleared += (sender, e) => assertAction(1);
            gameEventInterface.CurMinoMoved += (sender, e) => assertAction(2);
            gameEventInterface.CurMinoRotated += (sender, e) => assertAction(3);
            gameEventInterface.CurMinoHardDropped += (sender, e) => assertAction(4);
            gameEventInterface.Hold += (sender, e) => assertAction(5);
            
            gameEventInstance.OnBoardChanged(null);
            gameEventInstance.OnLineCleared(null);
            gameEventInstance.OnCurMinoMoved(null);
            gameEventInstance.OnCurMinoRotated(null);
            gameEventInstance.OnCurMinoHardDropped(null);
            gameEventInstance.OnHold(null);

            for(int i = 0; i < shouldAllTrue.Length; i++)
                Assert.True(shouldAllTrue[i], $"State is false in i = {i}");
        }
    }
}