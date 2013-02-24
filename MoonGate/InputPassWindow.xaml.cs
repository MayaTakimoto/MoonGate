using MoonGate.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    /// InputPassWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class InputPassWindow : Window
    {
        /*************************************************
         *  フィールド                                   *
         *************************************************/

        // 秘密鍵生成に使う情報
        private string sPassInf;


        /*************************************************
         *  コンストラクタ                               *
         *************************************************/

        public InputPassWindow()
        {
            InitializeComponent();

            // 左端のタブにフォーカス
            TabPassword.Focus();
        }


        /*************************************************
         *  メソッド                                     *
         *************************************************/
        
        /// <summary>
        /// 選択タブが変化した場合のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // メッセージ領域を初期化
            lblInfo.Content = string.Empty;

            // 選ばれたタブによって場合分け
            switch (tabMain.SelectedIndex)
            {
                case 0:
                    // パスワード暗号化タブの文字色を変更
                    TabPassword.Foreground = Brushes.Red;
                    TabPassfile.Foreground = Brushes.Black;
                    TabPassDrive.Foreground = Brushes.Black;
                    break;
                case 1:
                    // 鍵ファイル暗号化タブの文字色を変更
                    TabPassword.Foreground = Brushes.Black;
                    TabPassfile.Foreground = Brushes.Red;
                    TabPassDrive.Foreground = Brushes.Black;
                    break;
                case 2:
                    // 鍵ドライブ暗号化タブの文字色を変更
                    TabPassword.Foreground = Brushes.Black;
                    TabPassfile.Foreground = Brushes.Black;
                    TabPassDrive.Foreground = Brushes.Red;
                    break;
            }
        }


        /// <summary>
        /// OKボタン押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (tabMain.SelectedIndex == 0)
            {
                // 背景色を白にする
                PassBox.Background = Brushes.White;
                PassBox_Conf.Background = Brushes.White;

                // 入力に不備がないかチェック
                int iRet = isPasswordOK();
                if (iRet < 0)
                {
                    // エラーコードが返ってきた場合、その値で場合分け
                    switch (iRet)
                    {
                        case -1:    // 入力欄が空
                            // メッセージ欄にエラーメッセージを表示
                            lblInfo.Content = Properties.Resources.mesErrValisNull;

                            // 入力欄の背景色を変更する
                            PassBox.Background = Brushes.LightCoral;

                            break;

                        case -2:    // 確認入力欄が空
                            // メッセージ欄にエラーメッセージを表示
                            lblInfo.Content = Properties.Resources.mesErrValisNull;

                            // 確認入力欄の背景色を変更する
                            PassBox_Conf.Background = Brushes.LightCoral;

                            break;

                        case -3:
                            // メッセージ欄にエラーメッセージを表示
                            lblInfo.Content = Properties.Resources.mesErrInputMiss;

                            // 確認入力欄を初期化
                            PassBox_Conf.Password = string.Empty;

                            // 確認入力欄の背景色を変更する
                            PassBox_Conf.Background = Brushes.LightCoral;

                            break;
                    }
                }
                else
                {
                    // エラー無しの場合はパスワードを保持
                    sPassInf = PassBox_Conf.Password;
                }
            }
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }


        private int isPasswordOK()
        {
            if (string.IsNullOrEmpty(PassBox.Password))
            {
                // 入力欄が空である場合
                
                
                
                // 偽を返す
                return -1;
            }
            else if (string.IsNullOrEmpty(PassBox_Conf.Password))
            {
                // 確認入力欄が空である場合
                

                // 偽を返す
                return -2;
            }
            else if (PassBox.Password != PassBox_Conf.Password)
            {
                // 入力欄と確認入力欄で入力されたパスワードが異なる場合
                

                // 偽を返す
                return -3;
            }

            // 正常なら真を返す
            return 0;
        }
    }
}
