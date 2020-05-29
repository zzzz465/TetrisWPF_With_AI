using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// KeyScanner.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyScanner : UserControl
    {
        #region PROPDP
        public Key SelectedKey
        {
            get { return (Key)GetValue(SelectedKeyProperty); }
            set { SetValue(SelectedKeyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedKeyProperty =
            DependencyProperty.Register("SelectedKey", typeof(Key), typeof(KeyScanner), new PropertyMetadata(Key.None));

        public string LabelContent
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelContentProperty =
            DependencyProperty.Register("LabelContent", typeof(string), typeof(KeyScanner), new PropertyMetadata("KeySetting"));
        #endregion
        bool isSelecting = false;

        public KeyScanner()
        {
            InitializeComponent();
            if (SelectedKey == Key.None)
                btn.Content = "Click here to configure key";
            else
                btn.Content = $"Key : {SelectedKey}";
        }

        private void OnClick(object sender, EventArgs e)
        {
            btn.Content = "Press any key...";
            isSelecting = true;
        }

        private void OnKeyDown(object sender, EventArgs e)
        {
            if(e is KeyEventArgs kd)
            {
                if(kd.IsDown && isSelecting)
                {
                    SelectedKey = kd.Key;
                    btn.Content = $"Key : {kd.Key}";
                    isSelecting = false;
                }
            }
        }
    }
}
