using ColdClear;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Tetris;
using Tetris.AudioModule;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// TestWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
        }

        Tetromino[] tetrominos = new Tetromino[] { Tetromino.I, Tetromino.J, Tetromino.L, Tetromino.Z, Tetromino.S, Tetromino.T, Tetromino.O };
        int _lastClicked = 0;
        int lastClicked
        {
            get { return _lastClicked; }
            set { _lastClicked = value < tetrominos.Length ? value : 0; }
        }

        List<EventHandler> updateEventHandlers = new List<EventHandler>();
        List<EventHandler> soundEventHandlers = new List<EventHandler>();

        void StartNewGame(object sender, EventArgs e)
        {
            //GameView.tetrisGame = new AITetrisGame(ColdClearAI.CreateInstance(), AIGameSetting.Default);
            var userInputProvider = new UserInputManager(InputSetting.Default);
            GameView.tetrisGame = new PlayerTetrisGame(userInputProvider, TetrisGameSetting.Default);
            GameView.tetrisGame.InitializeGame();
            GameView.tetrisGame.StartGame();

            foreach(var eventHandler in updateEventHandlers)
            {
                CompositionTarget.Rendering -= eventHandler;
            }
            updateEventHandlers.Clear();

            RegisterSoundEffect(GameView.tetrisGame);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            EventHandler updateEventHandler = (obj, _) =>
            {
                userInputProvider.Update();
                GameView.tetrisGame.UpdateGame(sw.Elapsed);
                GameView.UpdateGameView();
            };

            CompositionTarget.Rendering += updateEventHandler;
            this.updateEventHandlers.Add(updateEventHandler);
        }

        void StartNewAIGame(object sender, EventArgs e)
        {

            foreach (var eventHandler in updateEventHandlers)
            {
                CompositionTarget.Rendering -= eventHandler;
            }
            updateEventHandlers.Clear();

            GameView.tetrisGame = new AITetrisGame(ColdClearAI.CreateInstance(), TetrisGameSetting.Default);
            GameView.tetrisGame.InitializeGame();
            GameView.tetrisGame.StartGame();

            RegisterSoundEffect(GameView.tetrisGame);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            EventHandler updateEventHandler = (obj, _) =>
            {
                GameView.tetrisGame.UpdateGame(sw.Elapsed);
                GameView.UpdateGameView();
            };

            CompositionTarget.Rendering += updateEventHandler;
            this.updateEventHandlers.Add(updateEventHandler);
        }

        void RegisterSoundEffect(TetrisGame game)
        {
            Func<string, string> getURL = (fileName) => $"./Resources/SoundEffect/Tetris99/{fileName}.wav";
            var moveSound = new CachedSound(getURL("se_game_move"));
            var softdropSound = new CachedSound(getURL("se_game_softdrop"));
            var hardDropSound = new CachedSound(getURL("se_game_harddrop"));
            var holdSound = new CachedSound(getURL("se_game_hold"));
            var fixSound = new CachedSound(getURL("se_game_fixa"));

            var gameEvent = game.TetrisGameEvent;
            Action<CachedSound> playSound = (cachedSound) => AudioEngine.SoundEffect.PlaySound(cachedSound);

            EventHandler playMove = (obj, e) => playSound(moveSound);
            EventHandler playHardDrop = (obj, e) => playSound(hardDropSound);
            EventHandler playHold = (obj, e) => playSound(holdSound);
            EventHandler playFix = (obj, e) => playSound(fixSound);
            EventHandler playSoftDrop = (obj, e) => playSound(softdropSound);

            gameEvent.CurMinoMoved += playMove;
            gameEvent.CurMinoHardDropped += playHardDrop;
            gameEvent.Hold += playHold;
            gameEvent.MinoLocked += playFix;
            gameEvent.SoftDropped += playSoftDrop;
        }

        private void GameView_HardDrop(object sender, RoutedEventArgs e)
        {

        }

        void StartNewAIVersusGame(object sender, RoutedEventArgs e)
        {
            var game1 = new AITetrisGame(ColdClearAI.CreateInstance(), TetrisGameSetting.Default);
            var game2 = new AITetrisGame(ColdClearAI.CreateInstance(), TetrisGameSetting.Default);

            game1.SetApponent(game2);
            game2.SetApponent(game1);

            TetrisGameView.SetTetrisGame(new List<TetrisGame>() { game1, game2 }, new List<iInputProvider>());
            TetrisGameView.StartNewGame();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            return;
        }
    }
}
