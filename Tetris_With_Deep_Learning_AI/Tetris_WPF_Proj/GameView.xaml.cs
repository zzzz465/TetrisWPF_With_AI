using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Diagnostics;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tetris;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// GameView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GameView : UserControl
    {
        public ResourceDictionary MinoStyles
        {
            get { return (ResourceDictionary)GetValue(MinoStylesProperty); }
            set { SetValue(MinoStylesProperty, value); }
        }
        public Tetromino HoldTetromino
        {
            get { return (Tetromino)GetValue(HoldTetrominoProperty); }
            set { SetValue(HoldTetrominoProperty, value); }
        }
        public IEnumerable<Tetromino> NextTetrominos
        {
            get { return (IEnumerable<Tetromino>)GetValue(NextTetrominosProperty); }
            set { SetValue(NextTetrominosProperty, value); }
        }

        public Brush BackgroundBrush
        {
            get { return (Brush)GetValue(BackgroundBrushProperty); }
            set { SetValue(BackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty BackgroundBrushProperty =
            DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(GameView), new PropertyMetadata(null));

        public static readonly DependencyProperty HoldTetrominoProperty =
            DependencyProperty.Register("HoldTetromino", typeof(Tetromino), typeof(GameView), new PropertyMetadata(Tetromino.None));
        public static readonly DependencyProperty MinoStylesProperty =
            DependencyProperty.Register("MinoStyles", typeof(ResourceDictionary), typeof(GameView), new PropertyMetadata(null));
        public static readonly DependencyProperty NextTetrominosProperty =
            DependencyProperty.Register("NextTetrominos", typeof(IEnumerable<Tetromino>), typeof(GameView), new PropertyMetadata(null));

        #region Events

        #endregion
        TetrisGame _tetrisGame;
        public TetrisGame tetrisGame
        {
            get { return _tetrisGame; }
            set
            {
                _tetrisGame = value;
                this.tetrisGrid.tetrisGame = value;
                RegisterEvents();
            }
        }

        public GameView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void UpdateGameView()
        {
            if (this.tetrisGame == null)
                return;

            this.tetrisGrid.DrawGame();
            DrawIncomingStat();
            NextTetrominos = tetrisGame.PeekBag();
            HoldTetromino = tetrisGame.HoldMinoType;
        }

        void DrawIncomingStat()
        {
            var canvas = IncomingDamageCanvas;
            var width = canvas.ActualWidth;
            var height = canvas.ActualHeight;
            var game = tetrisGame;
            var standardHeight = height / 20;
            IncomingColorBar.Height = standardHeight * game.GarbageLine;
        }

        void RegisterEvents()
        {
            /*
            var animator = Resources["HardDropEffectAnimator"] as DoubleAnimationUsingKeyFrames;
            var gameEvent = this.tetrisGame.TetrisGameEvent;
            gameEvent.CurMinoHardDropped += (obj, e) => AnimatedTetrisGridTransform.BeginAnimation(TranslateTransform.YProperty, animator);
            */
        }
    }
}
