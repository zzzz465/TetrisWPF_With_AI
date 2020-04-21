using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Tetris;
using OpenCvSharp;
using OpenCvSharp.Util;
using System.Diagnostics;
using ColdClear;

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
        TetrisGame[] tetrisGames = new TetrisGame[2];
        MouseCallback _mouseCallback;
        UserInputManager inputManager;
        (int width, int height) singleRectSize = (25, 25);
        public BoardRenderer()
        {
            image = new Mat(720, 1280, MatType.CV_8UC3);
            window = new Window("Window", image);
            inputManager = new UserInputManager(InputSetting.Default);
            var setting = InputSetting.Default;

            var playerGame = new PlayerTetrisGame(inputManager, InputSetting.Default, TetrisGameSetting.Default, new TetrominoBag()); // Player
            var FastAISetting = new AIGameSetting(TimeSpan.FromMilliseconds(8), TimeSpan.FromMilliseconds(16), TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(14));
            var SlowAiSetting = new AIGameSetting(TimeSpan.FromMilliseconds(92), TimeSpan.FromMilliseconds(40), TimeSpan.FromMilliseconds(150), TimeSpan.FromMilliseconds(64));
            var AIGame = new AITetrisGame(ColdClear.ColdClear.CreateInstance(), SlowAiSetting, new TetrominoBag());
            var AIGame2 = new AITetrisGame(ColdClear.ColdClear.CreateInstance(), SlowAiSetting, new TetrominoBag());

            // tetrisGames[0] = playerGame;
            tetrisGames[0] = AIGame;
            // tetrisGames[1] = AIGame2;

            /*
            playerGame.SetApponent(AIGame);
            AIGame.SetApponent(playerGame);
            */
            
            AIGame.SetApponent(AIGame2);
            // AIGame2.SetApponent(AIGame);

            // playerGame.StartGame();
            AIGame.InitializeGame();
            // AIGame2.InitializeGame();
            AIGame.StartGame();
            // AIGame2.StartGame();

            inputManager.ObserveKey(setting.CCW, setting.CW, setting.HardDrop, setting.SoftDrop, setting.Hold, setting.Left, setting.Right);
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
            Rect player1 = new Rect(0, 0, 640, 720);
            Rect player2 = new Rect(640, 0, 640, 720);
            var borderColor = new Scalar(70, 70, 70);
            while (true)
            {
                if(tetrisGames[0] != null)
                {
                    inputManager.Update();
                    DrawGrid(tetrisGames[0].Lines, player1, borderColor);
                    DrawGhostMino(player1, tetrisGames[0]);
                    DrawCurrentPiece(player1, tetrisGames[0]);
                    // DrawHold();
                    // DrawNext();
                    // DrawExpected();
                    window.ShowImage(image);
                    tetrisGames[0].UpdateGame(sw.Elapsed);
                }

                if(tetrisGames[1] != null)
                {
                    DrawGrid(tetrisGames[1].Lines, player2, borderColor);
                    DrawGhostMino(player2, tetrisGames[1]);
                    DrawCurrentPiece(player2, tetrisGames[1]);
                    // DrawHold();
                    // DrawNext();
                    // DrawExpected();
                    window.ShowImage(image);
                    tetrisGames[1].UpdateGame(sw.Elapsed);
                }

                Cv2.WaitKey(1);
                yield return null;
            }
        }

        static void mouseCallBack(MouseEventTypes @event, int x, int y, MouseEventFlags flags, IntPtr userData)
        {
            if(@event == MouseEventTypes.LButtonDown)
                Console.WriteLine($"{x} {y}");
        }

        void DrawGrid(IEnumerable<TetrisLine> lines, Rect rootRect, Scalar gridLineColor)
        {
            int y;

            var rectWidth = singleRectSize.width;
            var rectHeight = singleRectSize.height;
            var gridLeftTop_x = (rootRect.Width - rectWidth * 10) / 2f;
            var gridLeftTop_y = (image.Height - rectHeight * 20) / 2f;

            var gridLeftTop = new OpenCvSharp.Point(rootRect.X + gridLeftTop_x, rootRect.Y + gridLeftTop_y);
            var gridRightDown = new OpenCvSharp.Point(gridLeftTop.X + rectWidth * 10, gridLeftTop.Y + rectWidth * 20);

            //draw row line
            for(y = 0; y <= 21; y++)
            {
                var p1 = new OpenCvSharp.Point(gridLeftTop.X, gridLeftTop.Y + y * rectHeight);
                var p2 = new OpenCvSharp.Point(gridRightDown.X, p1.Y);

                Cv2.Line(image, p1, p2, gridLineColor);
            }

            //draw column line
            for(int x = 0; x <= 10; x++)
            {
                var p1 = new OpenCvSharp.Point(gridLeftTop.X + x * rectWidth, gridLeftTop.Y);
                var p2 = new OpenCvSharp.Point(p1.X, gridRightDown.Y + rectHeight);

                Cv2.Line(image, p1, p2, gridLineColor);
            }

            y = 0;
            foreach(var line in lines)
            {
                for(int x = 0; x < 10; x++)
                {
                    Rect rect = new Rect(
                        (gridLeftTop.X + singleRectSize.width * x + 1), 
                        (gridRightDown.Y - rectHeight * (y) + 1), 
                        rectWidth - 1, 
                        rectHeight - 1);
                    
                    var cellColor = minoColor[line.line[x]];
                    Cv2.Rectangle(image, rect, cellColor, -1);
                }
                y++;
            }
        }
        
        void DrawCurrentPiece(Rect rootRect, TetrisGame game)
        {
            var posOfCurMinoBlocks = game.PosOfCurMinoBlocks;
            if(posOfCurMinoBlocks == null)
                return;

            var rectWidth = singleRectSize.width;
            var rectHeight = singleRectSize.height;
            var gridLeftTop_x = (rootRect.Width - rectWidth * 10) / 2f;
            var gridLeftTop_y = (image.Height - rectHeight * 20) / 2f;
            
            var gridLeftTop = new OpenCvSharp.Point(rootRect.X + gridLeftTop_x, rootRect.Y + gridLeftTop_y);
            var gridRightDown = new OpenCvSharp.Point(gridLeftTop.X + rectWidth * 10, gridLeftTop.Y + rectWidth * 20);

            foreach(var BlockPos in posOfCurMinoBlocks)
            {
                var blockLeftTop = new Point(gridLeftTop.X + rectWidth * BlockPos.X, gridRightDown.Y - rectHeight * BlockPos.Y);
                Rect cell = new Rect(blockLeftTop.X - 1, blockLeftTop.Y + 1, rectWidth - 1, rectHeight - 1);
                var cellColor = minoColor[game.curMinoType];
                Cv2.Rectangle(image, cell, cellColor, -1);
            }
        }

        void DrawGhostMino(Rect rootRect, TetrisGame game)
        {
            var ghostMino = game.PosOfGhostMinoBlocks;

            if(ghostMino == null)
                return;

            var rectWidth = singleRectSize.width;
            var rectHeight = singleRectSize.height;
            var gridLeftTop_x = (rootRect.Width - rectWidth * 10) / 2f;
            var gridLeftTop_y = (image.Height - rectHeight * 20) / 2f;

            var gridLeftTop = new OpenCvSharp.Point(rootRect.X + gridLeftTop_x, rootRect.Y + gridLeftTop_y);
            var gridRightDown = new OpenCvSharp.Point(gridLeftTop.X + rectWidth * 10, gridLeftTop.Y + rectWidth * 20);

            foreach(var BlockPos in ghostMino)
            {
                var blockLeftTop = new Point(gridLeftTop.X + rectWidth * BlockPos.X, gridRightDown.Y - rectHeight * BlockPos.Y);
                Rect cell = new Rect(blockLeftTop.X - 1, blockLeftTop.Y + 1, rectWidth - 1, rectHeight - 1);
                var cellColor = minoColor[game.curMinoType];
                Cv2.Rectangle(image, cell, cellColor, 1);
            }
        }

        void DrawNext()
        {
            /*
            string next = "";
            foreach(var curMino in tetrisGame.PeekBag())
            {
                next = next + curMino + " ";
            }

            Cv2.PutText(image, next, new Point(660, 150), HersheyFonts.HersheySimplex, 1, Scalar.White);
            */
        }

        void DrawExpected()
        {
            /*
            if(tetrisGame is AITetrisGame AITetris)
            {
                var expectedMinoPoints = AITetris.expectedMinoEndPoints;
                if(expectedMinoPoints == null)
                    return;

                (int width, int height) rectSize = (25, 25);
                var leftTop = new Point(390, 110);
                var rightBottom = new Point(640, 610);
                var leftBottom = new Point(leftTop.X, rightBottom.Y);

                foreach(var BlockPos in expectedMinoPoints)
                {
                    var blockLeftTop = new Point(leftBottom.X + rectSize.width * BlockPos.X, leftBottom.Y - rectSize.height * BlockPos.Y);
                    Rect cell = new Rect(blockLeftTop.X - 2, blockLeftTop.Y + 2, rectSize.width - 2, rectSize.height - 2);
                    var cellColor = minoColor[tetrisGame.curMinoType];
                    Cv2.Rectangle(image, cell, Scalar.White, 1);
                }
            }
            */
        }

        void DrawHold()
        {
            /*
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
            */
        }
    }
}