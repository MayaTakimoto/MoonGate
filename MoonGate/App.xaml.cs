

using System.Threading;
using System.Windows;

namespace MoonGate.util
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : System.Windows.Application
    {
        /*************************************************
         *  フィールド                                   *
         *************************************************/

        // ミューテックス
        private static Mutex mtxMoonGate;

        /*************************************************
         *  メソッド                                     *
         *************************************************/

        /// <summary>        
        /// MoonGateの終了        
        /// </summary>        
        /// <param name="e"></param>        
        protected override void OnExit(ExitEventArgs e)
        {
            // ミューテックスが取得されていない場合は何もしない
            if (mtxMoonGate == null)
            {
                return;
            }

            // アプリケーション設定の保存    

            // ミューテックスを解放    
            releaseMutex();
        }


        /// <summary>        
        /// MoonGateの開始        
        /// </summary>        
        /// <param name="e"></param>        
        protected override void OnStartup(StartupEventArgs e)
        {
            // ミューテックスを取得    
            mtxMoonGate = new Mutex(false, "MoonGate-{982F430F-AD10-4449-A020-EFC5A5479DA5}");

            // 起動済みかどうかチェック
            if (!chkIsNotStarted())
            {
                // 起動済みの場合は終了する
                this.Shutdown();
            }

            // メイン画面を表示    
            MainWindow window = new MainWindow();
            window.Show();
        }


        /// <summary>
        /// 多重起動防止
        /// </summary>
        /// <param name="mutex">ミューテックス</param>
        /// <returns></returns>
        private static bool chkIsNotStarted()
        {
            if (!mtxMoonGate.WaitOne(0, false))
            {
                // すでに起動していると判断した場合
                // ミューテックスを破棄する
                mtxMoonGate.Close();
                mtxMoonGate = null;

                // 偽を返す
                return false;
            }

            // 起動していない場合は真を返す
            return true;
        }


        /// <summary>
        /// 終了前にミューテックスを解放する
        /// </summary>
        private static void releaseMutex()
        {
            // ミューテックスの解放
            mtxMoonGate.ReleaseMutex();

            // ミューテックスを破棄する
            mtxMoonGate.Close();
            mtxMoonGate = null;
        }

    }
}
