using System.Windows;
using System.Windows.Input;

namespace MoonGate
{
    /// <summary>
    /// FileBrowseWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderBrowseWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FolderBrowseWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 選択ノード変更時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SelectedNode.DataContext = e.NewValue;
        }


        /// <summary>
        /// ダイアログを閉じる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
