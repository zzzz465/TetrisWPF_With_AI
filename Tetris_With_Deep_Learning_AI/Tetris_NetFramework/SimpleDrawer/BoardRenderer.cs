using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Tetris;
using OpenCvSharp;
using OpenCvSharp.Util;
using System.Diagnostics;

namespace Tetris.Renderer
{
    public class BoardRenderer
    {
        static Dictionary<Tetromino, Scalar> minoColor = new Dictionary<Tetromino, Scalar>()
        {
            { Tetromino.I, new Scalar(170, 210, 15) },
            { Tetromino.T, new Scalar(180, 30, 170) },
            { Tetromino.O, new Scalar(10, 200, 220) },
            { Tetromino.S, new Scalar(30, 200, 50) },
            { Tetromino.Z, new Scalar(40, 50, 200) },
            { Tetromino.L, new Scalar(200, 150, 15) },
            { Tetromino.J, new Scalar(190, 40, 40) },
            { Tetromino.Garbage, Scalar.Gray },
            { Tetromino.None, new Scalar(0, 0, 0) }
        };
        Window window;
        Mat image;
        TetrisGame tetrisGame;
        MouseCallback _mouseCallback;
        UserInputManager inputManager;
        public BoardRenderer()
        {
            image = new Mat(720, 1280, MatType.CV_8UC3);
            window = new Window("Window", image);
            inputManager = new UserInputManager(InputSetting.Default);
            var setting = InputSetting.Default;
            tetrisGame = new PlayerTetrisGame(inputManager, InputSetting.Default, TetrisGameSetting.Default, new TetrominoBag());
            inputManager.ObserveKey(setting.CCW, setting.CW, setting.HardDrop, setting.SoftDrop, setting.Hold, setting.Left, setting.Right);
            tetrisGame.StartGame();
            _mouseCallback = mouseCallBack;
            Cv2.SetMouseCallback("Window", _mouseCallback);
        }
        public void syncUpdateLoop()
        {
            var enumerator = UpdateOnce();
            while(true)
            {
                enumerator.MoveNext();
            }
        }
        public IEnumerator UpdateOnce()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                inputManager.Update();
                DrawBoard();
                DrawGhostMino();
                DrawCurrentPiece();
                DrawHold();
                window.ShowImage(image);
                tetrisGame.UpdateGame(sw.Elapsed);
                Cv2.WaitKey(3);
                yield return null;
            }
        }

        static void mouseCallBack(MouseEventTypes @event, int x, int y, MouseEventFlags flags, IntPtr userData)
        {
            if(@event == MouseEventTypes.LButtonDown)
                Console.WriteLine($"{x} {y}");
        }

        void DrawBoard()
        {
            // 1 box = 25 * 25
            // width = 250, height = 500
            // x 390 ~ x 640, y 110 ~ y 610

            Cv2.Rectangle(image, new Rect(0, 0, 720, 1280), Scalar.Black, -1);

            var leftTop = new Point(390, 110);
            var rightBottom = new Point(640, 610);

            (int width, int height) rectSize = (25, 25);

            // draw border
            Cv2.Rectangle(image, leftTop, rightBottom, Scalar.White, 2);

            // draw grid
            for(int y = 0; y <= 20; y++) // row
                Cv2.Line(image, new Point(leftTop.X, leftTop.Y + rectSize.height * y), new Point(rightBottom.X, leftTop.Y + rectSize.height * y), new Scalar(10 * y, 10 * y, 200), 2);

            for(int x = 0; x <= 10; x++) // column
                Cv2.Line(image, new Point(leftTop.X + rectSize.width * x, leftTop.Y), new Point(leftTop.X + rectSize.width * x, rightBottom.Y), new Scalar(200, 10 * x, 10 * x), 2);

            var lines = tetrisGame.Lines;

            for(int y = 0; y < 20; y++)
            { // 좌측 하단부터 시작해서, 우측 상단으로 올라감
                var line = lines.ElementAt(y);
                for(int x = 0; x < 10; x++)
                {
                    var blockExist = line.line[x] != 0;
                    Rect cell = new Rect( (leftTop.X + rectSize.width * x + 1), (rightBottom.Y - rectSize.height * y - 1), rectSize.width - 1, rectSize.height - 1);
                    var cellColor = minoColor[line.line[x]];
                    Cv2.Rectangle(image, cell, cellColor, -1);
                }
            }
        }

        void DrawCurrentPiece()
        {
            var posOfCurMinoBlocks = tetrisGame.PosOfCurMinoBlocks;
            if(posOfCurMinoBlocks == null)
                return;

            (int width, int height) rectSize = (25, 25);
            var leftTop = new Point(390, 110);
            var rightBottom = new Point(640, 610);
            var leftBottom = new Point(leftTop.X, rightBottom.Y);

            foreach(var BlockPos in posOfCurMinoBlocks)
            {
                var blockLeftTop = new Point(leftBottom.X + rectSize.width * BlockPos.X, leftBottom.Y - rectSize.height * BlockPos.Y);
                Rect cell = new Rect(blockLeftTop.X - 1, blockLeftTop.Y + 1, rectSize.width - 1, rectSize.height - 1);
                var cellColor = minoColor[tetrisGame.curMinoType];
                Cv2.Rectangle(image, cell, cellColor, -1);
            }
        }

        void DrawGhostMino()
        {
            var ghostMino = tetrisGame.PosOfGhostMinoBlocks;

            if(ghostMino == null)
                return;

            (int width, int height) rectSize = (25, 25);
            var leftTop = new Point(390, 110);
            var rightBottom = new Point(640, 610);
            var leftBottom = new Point(leftTop.X, rightBottom.Y);

            foreach(var BlockPos in ghostMino)
            {
                var blockLeftTop = new Point(leftBottom.X + rectSize.width * BlockPos.X, leftBottom.Y - rectSize.height * BlockPos.Y);
                Rect cell = new Rect(blockLeftTop.X - 1, blockLeftTop.Y + 1, rectSize.width - 1, rectSize.height - 1);
                var cellColor = minoColor[tetrisGame.curMinoType];
                Cv2.Rectangle(image, cell, cellColor, 1);
            }
        }

        void DrawNext()
        {
            
        }

        void DrawHold()
        {
            // 390 640 이 시작..
            // 225 * 250 ~ 320 * 240 에서 만들어주자
            Point leftTop = new Point(225, 250);
            Point rightDown = new Point(320, 240);
            var width = rightDown.X - leftTop.X;
            var height = rightDown.Y - leftTop.Y;

            (int width, int height) boxSize = (width / 4, height / 4);

            var holdMinoType = tetrisGame.HoldMinoType;

            string c;
            switch(holdMinoType)
            {
                case Tetromino.J:
                    c = "J";
                    break;
                case Tetromino.I:
                    c = "I";
                    break;
                case Tetromino.L:
                    c = "L";
                    break;
                case Tetromino.O:
                    c = "O";
                    break;
                case Tetromino.S:
                    c = "S";
                    break;
                case Tetromino.T:
                    c = "T";
                    break;
                case Tetromino.Z:
                    c = "Z";
                    break;
                case Tetromino.Garbage:
                    c = "??";
                    break;
                default:
                    c = "None";
                    break;
            }

            Cv2.PutText(image, c.ToString(), leftTop, HersheyFonts.HersheySimplex, 1, Scalar.White);
        }
    }
}