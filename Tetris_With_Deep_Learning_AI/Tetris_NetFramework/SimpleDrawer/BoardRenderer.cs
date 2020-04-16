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

        Window window;
        Mat image;
        TetrisGame tetrisGame;
        MouseCallback _mouseCallback;
        UserInputManager inputManager;
        public BoardRenderer()
        {
            image = new Mat(720, 1280, MatType.CV_8UC3);
            window = new Window("Window", image);
            inputManager = new UserInputManager();
            var setting = InputSetting.Default;
            tetrisGame = new TetrisGame(inputManager, InputSetting.Default, new TetrominoBag());
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
                window.ShowImage(image);
                tetrisGame.Update(sw.Elapsed);
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
                    var blockExist = line.line[x];
                    Rect cell = new Rect( (leftTop.X + rectSize.width * x + 1), (rightBottom.Y - rectSize.height * y - 1), rectSize.width - 1, rectSize.height - 1);
                    if(blockExist)
                        Cv2.Rectangle(image, cell, Scalar.Green, -1);
                    else
                        Cv2.Rectangle(image, cell, Scalar.Black, -1);
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
                Cv2.Rectangle(image, cell, Scalar.Aqua, -1);
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
                Cv2.Rectangle(image, cell, Scalar.YellowGreen, 1);
            }
        }

        void DrawNext()
        {
            
        }
    }
}