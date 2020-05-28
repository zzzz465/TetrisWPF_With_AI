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
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using System.Globalization;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// InputBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InputBox : UserControl
    {
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(InputBox), new PropertyMetadata(0, null, ValidateValue));

        public string LabelContent
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelContentProperty =
            DependencyProperty.Register("LabelContent", typeof(string), typeof(InputBox), new PropertyMetadata(string.Empty));


        public InputBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private static object ValidateValue(DependencyObject d, object baseValue)
        {
            if (int.TryParse(baseValue.ToString(), out var result))
                return result;
            else
                return 0;
        }

        private void OnTextPrewview(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "[\\d]+");
        }
    }

    [ValueConversion(typeof(int), typeof(string))]
    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        { // 원형 PropertyData에서, Text로 변환
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value.ToString();
            if (int.TryParse(str, out var result))
                return result;
            else
                return 0;
        }
    }
}
