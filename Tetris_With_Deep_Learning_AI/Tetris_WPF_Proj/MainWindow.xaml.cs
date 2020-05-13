using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Tetris_WPF_Proj.UI;

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
            RootCC.Content = new MainMenu();
        }

        public void OpenWindow(Control control)
        {
            UIStack.Push(RootCC.Content as Control);
            RootCC.Content = control;
        }

        public void CloseWindow()
        {
            var curStackTrace = new StackTrace();
            var type = curStackTrace.GetFrame(0).GetMethod().ReflectedType;
            if (type != UIStack.Peek().GetType())
                throw new InvalidOperationException("Content can be closed by itself, do not call CloseWindow() outside Content object");

            RootCC.Content = null;
            var content = UIStack.Pop();
            RootCC.Content = content;
        }
    }
}
