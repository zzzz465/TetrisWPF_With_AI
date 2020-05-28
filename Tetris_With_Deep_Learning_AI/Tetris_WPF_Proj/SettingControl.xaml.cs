using ColdClear;
using log4net.Util;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
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
using Tetris;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// SettingControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public enum PlayerType
    {
        Player = 0,
        AI = 1
    }
    public partial class SettingControl : UserControl
    {
        #region PROPDP
        public GlobalSetting.PlayerSetting PlayerSettingInstance
        {
            get { return (GlobalSetting.PlayerSetting)GetValue(PlayerSettingInstanceProperty); }
            set { SetValue(PlayerSettingInstanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlayerSettingInstance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayerSettingInstanceProperty =
            DependencyProperty.Register("PlayerSettingInstance", typeof(GlobalSetting.PlayerSetting), typeof(SettingControl), new PropertyMetadata(null));

        public int AutoDropDelay
        {
            get { return (int)GetValue(AutoDropDelayProperty); }
            set { SetValue(AutoDropDelayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoDropDelay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoDropDelayProperty =
            DependencyProperty.Register("AutoDropDelay", typeof(int), typeof(SettingControl), new PropertyMetadata(80));

        public int SoftDropDelay
        {
            get { return (int)GetValue(SoftDropDelayProperty); }
            set { SetValue(SoftDropDelayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SoftDropDelay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SoftDropDelayProperty =
            DependencyProperty.Register("SoftDropDelay", typeof(int), typeof(SettingControl), new PropertyMetadata(22));

        public int MinoSpawnDelay
        {
            get { return (int)GetValue(MinoSpawnDelayProperty); }
            set { SetValue(MinoSpawnDelayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinoSpawnDelay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinoSpawnDelayProperty =
            DependencyProperty.Register("MinoSpawnDelay", typeof(int), typeof(SettingControl), new PropertyMetadata(50));

        public int DASDelay
        {
            get { return (int)GetValue(DASDelayProperty); }
            set { SetValue(DASDelayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DASDelay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DASDelayProperty =
            DependencyProperty.Register("DASDelay", typeof(int), typeof(SettingControl), new PropertyMetadata(16));

        public int ARRDelay
        {
            get { return (int)GetValue(ARRDelayProperty); }
            set { SetValue(ARRDelayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ARRDelay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ARRDelayProperty =
            DependencyProperty.Register("ARRDelay", typeof(int), typeof(SettingControl), new PropertyMetadata(26));


        public PlayerType SelectedPlayerType
        {
            get { return (PlayerType)GetValue(SelectedPlayerTypeProperty); }
            set { SetValue(SelectedPlayerTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedPlayerType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedPlayerTypeProperty =
            DependencyProperty.Register("SelectedPlayerType", typeof(PlayerType), typeof(SettingControl), new PropertyMetadata(PlayerType.AI));

        #endregion

        public SettingControl()
        {
            InitializeComponent();
        }

        private void OnApplyButtonClick(object sender, RoutedEventArgs e)
        {
            Action<string> Message = (str) => MessageBox.Show(str);
            if (this.PlayerSettingInstance == null)
            {
                MessageBox.Show("PlayerSettingInstance is not applied");
                return;
            }

            var setting = PlayerSettingInstance;
            ref var gameSetting = ref setting.PlayerGameSetting;
            gameSetting.ARRDelay = TimeSpan.FromMilliseconds(ARRDelay);
            gameSetting.DASDelay = TimeSpan.FromMilliseconds(DASDelay);
            gameSetting.minoSpawnDelay = TimeSpan.FromMilliseconds(MinoSpawnDelay);
            gameSetting.softDropDelay = TimeSpan.FromMilliseconds(SoftDropDelay);
            gameSetting.autoDropDelay = TimeSpan.FromMilliseconds(AutoDropDelay);

            var tagString = (PlayerSelectionComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
            if(tagString.Contains("Player"))
            {
                
                PlayerSettingInstance.ai = null;
                ref var inputsetting = ref PlayerSettingInstance.playerInputSetting;
                inputsetting.HardDrop = HardDropKeyScanner.SelectedKey;
                inputsetting.SoftDrop = SoftDropKeyScanner.SelectedKey;
                inputsetting.Hold = HoldScanner.SelectedKey;
                inputsetting.Left = LeftScanner.SelectedKey;
                inputsetting.Right = RightScanner.SelectedKey;
                inputsetting.CCW = CCWScanner.SelectedKey;
                inputsetting.CW = CWScanner.SelectedKey;
                
            }
        }

        private void OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            if(sender is ComboBox comboBox)
            {
                if(comboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    var TagString = selectedItem.Tag.ToString();
                    if(TagString.Contains("AI"))
                    {
                        InputSettingGrid.Visibility = Visibility.Hidden;
                        
                        if (TagString.Contains("ColdClear"))
                        {
                            this.PlayerSettingInstance.ai = ColdClearAI.CreateInstance();
                        }
                        
                    }
                    else if(TagString.Contains("Player"))
                    {
                        InputSettingGrid.Visibility = Visibility.Visible;
                    }
                }
            }
        }
    }
}
