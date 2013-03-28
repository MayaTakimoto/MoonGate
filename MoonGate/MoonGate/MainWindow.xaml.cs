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

        private void BtnSetting_Click_1(object sender, RoutedEventArgs e)
        {
            ComboItemEntity c = this.MenuItemUpload.Items.CurrentItem as ComboItemEntity;
            //ComboItemEntity i = this.MenuItemUpload.Items[0] as ComboItemEntity;
            MessageBox.Show(c.Key);
        }

        private void CommandBinding_Executed_1(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }
    }
}
