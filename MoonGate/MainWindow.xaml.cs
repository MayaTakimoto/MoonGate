/**************************************************************************************
 *                                                                                    *
 * Program Name : MainWindow.xaml.cs                                                  *
 *                                                                                    *
 * Designed on 2013.02.01 by MayaTakimoto                                             *
 *                                                                                    *
 **************************************************************************************/

using Microsoft.Win32;
using System.Text;
using System.Threading;
using System.Windows;

namespace MoonGate.util
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
            // スプラッシュ画像表示時は1.5秒待機
            Thread.Sleep(1500);

            // メイン画面描画
            InitializeComponent();

            // UIリスト部分とリストエンティティとのバインディング
            this.DataContext = ListEntity.EntList;

            // 件数表示
            dspListCount();
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


        /// <summary>
        /// ファイルをリストに追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFileOpen_Click(object sender, RoutedEventArgs e)
        {
            // ファイル選択ダイアログのインスタンスを生成
            OpenFileDialog dialogGetFiles = new OpenFileDialog();

            // 複数選択可に設定
            dialogGetFiles.Multiselect = true;

            // デフォルトの選択ファイルはなし
            dialogGetFiles.FileName = "";

            // すべてのファイルタイプをターゲットとする
            dialogGetFiles.DefaultExt = "*.*";

            // ファイル選択
            if (dialogGetFiles.ShowDialog() == true)
            {
                // 選択されたらリストに追加する
                foreach (string sFileName in dialogGetFiles.FileNames)
                {
                    ListEntity.EntList.ObsFileList.Add(sFileName);
                }
            }

            // 件数表示
            dspListCount();
        }

        
        /// <summary>
        /// ディレクトリをリストに追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFolderOpen_Click(object sender, RoutedEventArgs e)
        {
            
        }


        /// <summary>
        /// リストの破棄
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            // リストを破棄する
            ListEntity.EntList.ObsFileList.Clear();
        }


        /// <summary>
        /// "Start to Encrypt" クリック時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            // ファイルが1つ以上リストに登録されている場合のみ処理
            if (ListEntity.EntList.ObsFileList.Count > 0)
            {
                // 鍵設定ダイアログを生成
                InputPassWindow ipWnd = new InputPassWindow();

                // オーナーをメインウィンドウにし、中央に表示されるようにする
                ipWnd.Owner = this;
                ipWnd.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                
                // ダイアログを表示
                ipWnd.ShowDialog();
            }
        }


        /// <summary>
        /// "Start to Decrypt" クリック時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// "Encrypt and Upload" クリック時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpload_Click(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// "Download and Decrypt" クリック時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// "Get Connention" クリック時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// リストから選択されたアイテムを削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <!-- かなり苦肉の策。代替案を考えたい -->
        private void MenuItemRemove_Click(object sender, RoutedEventArgs e)
        {
            // 選択されたアイテムを別の配列へコピー
            string[] sItems = new string[this.ListMain.SelectedItems.Count];
            this.ListMain.SelectedItems.CopyTo(sItems, 0);

            // リストから削除する
            foreach (string sItem in sItems)
            {
                ListEntity.EntList.ObsFileList.Remove(sItem);
            }

            // 件数表示
            dspListCount();
        }


        /// <summary>
        /// リストに登録されたアイテムを開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {

        }


        /// <summary>
        /// ステータスバーにリスト件数を表示
        /// </summary>
        private void dspListCount()
        {
            StringBuilder sbCount = new StringBuilder(ListEntity.EntList.ObsFileList.Count.ToString(), 32);

            //sbCount.Insert(0, ListEntity.EntList.ObsFileList.Count);
            sbCount.Append(" files");

            this.txtListCount.Text = sbCount.ToString();
        }
    }
}
