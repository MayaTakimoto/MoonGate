using mgcrypt;
using mgcrypt.Rijndael;
using MoonGate.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MoonGate.util
{
    /// <summary>
    /// doCryptWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DoCryptWindow : Window
    {
        /* 定数 */
        private const int KEY_LENGTH = 256;     // 鍵長
        private const int BLOCK_SIZE = 128;     // 暗号化ブロックサイズ

        // 秘密鍵生成に利用する情報
        private string sPassInf;

        // 暗号化のモード
        private int iMode;

        // キャンセルフラグ
        private bool isCancel;

        // プログレスバー実装のためのBackgroundWorkerクラス
        private BackgroundWorker bwProgress;

        public DoCryptWindow(string sPassInf, int iMode)
        {
            InitializeComponent();

            // 変数初期化
            this.sPassInf = sPassInf;
            this.iMode = iMode;
            
            // プログレスバーのMax値を対象ファイル数に設定
            progressBarMain.Maximum = ListEntity.EntList.ObsFileList.Count;

            // BackgroundWorkerインスタンスの初期化
            setBackgroundWorker();

            // 処理を開始する
            bwProgress.RunWorkerAsync();
        }

        /// <summary>
        /// BackgroundWorkerインスタンスの初期化
        /// </summary>
        private void setBackgroundWorker()
        {
            // BackgroundWorkerの生成
            bwProgress = new BackgroundWorker();

            // 進捗の更新を可能に設定
            bwProgress.WorkerReportsProgress = true;

            // キャンセルを可能に設定
            bwProgress.WorkerSupportsCancellation = true;

            // バックグラウンドで実行する処理を設定
            bwProgress.DoWork += bwProgress_DoWork;

            // 進捗更新処理を設定
            bwProgress.ProgressChanged += bwProgress_ProgressChanged;

            // 完了時の処理を設定
            bwProgress.RunWorkerCompleted += bwProgress_RunWorkerCompleted;
        }


        /// <summary>
        /// 暗号化完了後に行う処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bwProgress_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // OKボタンを有効化する
            btnOK.IsEnabled = true;

            // キャンセルボタンを無効化する
            btnCancel.IsEnabled = false;
        }


        /// <summary>
        /// 進捗が更新された場合の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bwProgress_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // 進捗率をプログレスバーに反映させる
            progressBarMain.Value = e.ProgressPercentage;

            // 進捗率をラベルに表示
            lblStatus.Content = e.ProgressPercentage.ToString();
        }


        /// <summary>
        /// バックグラウンドで暗号化する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bwProgress_DoWork(object sender, DoWorkEventArgs e)
        {
            // ロック用オブジェクトを準備
            object objLock = new object();

            // 進捗更新用変数を準備
            int iProgress = 0;

            // キャンセルフラグを初期化する
            isCancel = false;

            Parallel.For(0, ListEntity.EntList.ObsFileList.Count, (iCnt, loopState) =>
                {
                    // キャンセルが選択されたかどうかチェック
                    if (bwProgress.CancellationPending)
                    {
                        // キャンセルの場合
                        // 処理を止める
                        loopState.Stop();

                        // 戻る
                        return;
                    }

                    // 100ms待機
                    Thread.Sleep(100);

                    // 暗号化処理用インスタンス生成
                    Encryptor enc = new RijndaelEncryptor(ListEntity.EntList.ObsFileList[iCnt]);

                    // 暗号化開始
                    int iRet = enc.encrypt(sPassInf, iMode, 256, 128);
                    if (iRet != 0)
                    {
                        // エラーコードが飛んできた場合
                        // 処理を止める
                        loopState.Stop();

                        // エラー内容で分ける
                        onError(iRet);

                        // 戻る
                        return;
                    }

                    // ロックを取得する
                    lock (objLock)
                    {
                        // 進捗率を更新する
                        iProgress += getPercentage();
                        bwProgress.ReportProgress(iProgress);
                    }
                });

            // 全ファイル処理後、進捗率が100%にならない場合は強制的に100%にする（これも苦肉）
            if (iProgress < 100)
            {
                bwProgress.ReportProgress(100);
            }
        }


        /// <summary>
        /// OKボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            // 処理がキャンセルされていない場合
            if (!isCancel)
            {
                // リストを初期化する
                ListEntity.EntList.ObsFileList.Clear();
            }

            // ダイアログを閉じる
            this.Close();
        }


        /// <summary>
        /// キャンセルボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // 処理を中断する
            bwProgress.CancelAsync();

            // キャンセルフラグをオンにする
            isCancel = true;
        }


        /// <summary>
        /// エラーコード取得時の処理
        /// </summary>
        /// <param name="iErrCd">エラーコード</param>
        private void onError(int iErrCd)
        {
            // 処理を中断する
            bwProgress.CancelAsync();

            // キャンセルフラグをオンにする
            isCancel = true;

            // エラーコードで場合分け
        }


        /// <summary>
        /// 1ファイル処理が処理全体の何％に当たるかを計算する
        /// </summary>
        /// <returns></returns>
        private int getPercentage()
        {
            // 全対象を100で割った値を返す（小数点以下切捨て）
            return ListEntity.EntList.ObsFileList.Count / 100;
        }
    }
}
