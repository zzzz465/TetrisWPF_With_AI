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
using log4net;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// Cell.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Cell : UserControl
    {
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
