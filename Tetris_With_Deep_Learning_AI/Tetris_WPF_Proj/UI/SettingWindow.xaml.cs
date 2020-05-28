using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Tetris_WPF_Proj.UI
{
    /// <summary>
    /// SettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWindow : UserControl
    {
        public SettingWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            P1Setting.PlayerSettingInstance = GlobalSetting.Instance.Player1Setting;
            P2Setting.PlayerSettingInstance = GlobalSetting.Instance.Player2Setting;
        }

        private void OnExit(object sender, EventArgs e)
        {
            MainWindow.Instance.CloseWindow();
        }
    }
}
