using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Tetris_WPF_Proj.UI;
using System.Runtime.CompilerServices;

namespace Tetris_WPF_Proj
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; set; }
        Stack<Control> UIStack = new Stack<Control>();
        public MainWindow()
        {
            InitializeComponent();
            if (MainWindow.Instance != null)
                throw new Exception("Unexpected behaviour, do not create mainWindow directly");
            MainWindow.Instance = this;
            OpenWindow(new MainMenu());
        }

        public void OpenWindow(Control control)
        {
            UIStack.Push(RootCC.Content as Control);
            RootCC.Content = control;
        }

        public void CloseWindow()
        {
            RootCC.Content = null;
            var content = UIStack.Pop();
            RootCC.Content = content;
        }
    }
}
