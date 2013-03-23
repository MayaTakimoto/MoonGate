/**************************************************************************************
 *                                                                                    *
 * Program Name : MainWindow.xaml.cs                                                  *
 *                                                                                    *
 * Designed on 2013.02.01 by MayaTakimoto                                             *
 *                                                                                    *
 **************************************************************************************/

using Microsoft.Win32;
using MoonGate.Component;
using MoonGate.Component.Entity;
using MoonGate.Component.Message;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace MoonGate
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            Thread.Sleep(1500);
            InitializeComponent();
        }


        /// <summary>
        /// "Menu" → "Settling" クリック時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuSetting_Click(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// "Menu" → "Exit" クリック時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            // MoonGateを終了する
            this.Close();
        }
    }
}
