/**************************************************************************************
 *                                                                                    *
 * Program Name : MainWindow.xaml.cs                                                  *
 *                                                                                    *
 * Designed on 2013.02.01 by MayaTakimoto                                             *
 *                                                                                    *
 **************************************************************************************/

using System.Threading;
using System.Windows;

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
    }
}
