using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// Cell.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Cell : UserControl
    {
        #region MinoStyle
        public Style LMinoStyle { get { return (Style)GetValue(LMinoStyleProperty); } set { SetValue(LMinoStyleProperty, value); } }
        public Style JMinoStyle { get { return (Style)GetValue(JMinoStyleProperty); } set { SetValue(JMinoStyleProperty, value); } }
        public Style ZMinoStyle { get { return (Style)GetValue(ZMinoStyleProperty); } set { SetValue(ZMinoStyleProperty, value); } }
        public Style SMinoStyle { get { return (Style)GetValue(SMinoStyleProperty); } set { SetValue(SMinoStyleProperty, value); } }
        public Style TMinoStyle { get { return (Style)GetValue(TMinoStyleProperty); } set { SetValue(TMinoStyleProperty, value); } }
        public Style OMinoStyle { get { return (Style)GetValue(OMinoStyleProperty); } set { SetValue(OMinoStyleProperty, value); } }
        public Style IMinoStyle { get { return (Style)GetValue(IMinoStyleProperty); } set { SetValue(IMinoStyleProperty, value); } }

        public static readonly DependencyProperty LMinoStyleProperty = DependencyProperty.Register(
            "LMinoStyle", typeof(Style), typeof(Cell), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty JMinoStyleProperty = DependencyProperty.Register(
            "JMinoStyle", typeof(Style), typeof(Cell), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ZMinoStyleProperty = DependencyProperty.Register(
            "ZMinoStyle", typeof(Style), typeof(Cell), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty SMinoStyleProperty = DependencyProperty.Register(
            "SMinoStyle", typeof(Style), typeof(Cell), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty TMinoStyleProperty = DependencyProperty.Register(
            "TMinoStyle", typeof(Style), typeof(Cell), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty OMinoStyleProperty = DependencyProperty.Register(
            "OMinoStyle", typeof(Style), typeof(Cell), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty IMinoStyleProperty = DependencyProperty.Register(
            "IMinoStyle", typeof(Style), typeof(Cell), new FrameworkPropertyMetadata());
        #endregion

        Point point;

        public Cell()
        {
            InitializeComponent();
        }

        public Cell(BitmapImage image) : this()
        {
            Background = new ImageBrush();
        }

        public Cell(Color backgroundColor) : this()
        {
            Background = new SolidColorBrush(backgroundColor);
        }

        public void SetCoord(Point point)
        {
            this.point = point;
        }

        public void SetColor(Color color)
        {
            if (Background is SolidColorBrush)
            {
                (Background as SolidColorBrush).Color = color;
            }
            else
                throw new Exception();
        }

        public void SetBackground()
        {
            throw new NotImplementedException();
        }
    }
}
