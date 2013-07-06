//-----------------------------------------------------------------------
// <summary>鍵情報入力ダイアログ表示のActionクラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-04-06 ‏‎23:09:37  +9:00 $</date>
// <copyright file="$Name: TransceiveAction.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using mgcloud;
using mgcloud.CloudOperator;
using mgcrypt;
using mgcrypt.Rijndael;
using MoonGate.Component.ViewModel;
using MoonGate.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MoonGate.Component.Action
{
    /// <summary>
    /// InputPassWindow表示Actionクラス
    /// </summary>
    class TransceiveAction : TriggerAction<FrameworkElement>
    {
        /// <summary>
        /// 
        /// </summary>
        private const string USERFILE_PATH = "./user/consumer.xml";

        /// <summary>
        /// 
        /// </summary>
        public enum TransceiveMode
        {
            Upload,
            UploadAll,
            Download,
            DownloadAll
        }

        /// <summary>
        /// パスワード
        /// </summary>
        internal SecureString PassWord { get; private set; }

        /// <summary>
        /// パスファイル
        /// </summary>
        internal FileInfo PassFile { get; private set; }

        /// <summary>
        /// パスドライブ
        /// </summary>
        internal string PassDrive { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ListItemViewModel> FileList
        {
            get { return (ObservableCollection<ListItemViewModel>)GetValue(FileListProperty); }
            set { SetValue(FileListProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ProgressBar PrgBar
        {
            get { return (ProgressBar)GetValue(PrgBarProperty); }
            set { SetValue(PrgBarProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public object CloudId
        {
            get { return (object)GetValue(CloudIdProperty); }
            set { SetValue(CloudIdProperty, value); }
        }

        /// <summary>
        /// 実行モード
        /// </summary>
        public TransceiveMode TMode
        {
            get { return (TransceiveMode)GetValue(TModeProperty); }
            set { SetValue(TModeProperty, value); }
        }

        /// <summary>
        /// 実行コマンド(依存関係プロパティ)
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty FileListProperty =
            DependencyProperty.Register("FileList", typeof(ObservableCollection<ListItemViewModel>), typeof(TransceiveAction), new UIPropertyMetadata());

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty PrgBarProperty =
            DependencyProperty.Register("PrgBar", typeof(ProgressBar), typeof(TransceiveAction), new UIPropertyMetadata());

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CloudIdProperty =
            DependencyProperty.Register("CloudId", typeof(object), typeof(TransceiveAction), new UIPropertyMetadata());

        /// <summary>
        /// 依存関係プロパティの登録
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TransceiveAction), new UIPropertyMetadata());

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TModeProperty =
            DependencyProperty.Register("TMode", typeof(TransceiveMode), typeof(TransceiveAction), new UIPropertyMetadata());


        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        protected override void Invoke(object param)
        {
            if (FileList == null || FileList.Count == 0)
            {
                return;
            }

            if (TMode == TransceiveMode.Upload || TMode == TransceiveMode.Download)
            {
                bool selectFlg = false;

                foreach (var item in FileList)
                {
                    if (item.IsSelected)
                    {
                        selectFlg = true;
                        break;
                    }
                }

                if (selectFlg == false)
                {
                    return;
                }
            }

            InputPassWindow dialogInputPass = new InputPassWindow();

            var resDlg = dialogInputPass.ShowDialog();
            if ((bool)resDlg == false)
            {
                return;
            }

            PassWord = dialogInputPass.pswdInput.SecurePassword;
            PassFile = new FileInfo(dialogInputPass.ConKeyFile.Tag.ToString());
            PassDrive = dialogInputPass.DriveList.SelectedItem.ToString();

            List<ListItemViewModel> listTargets = new List<ListItemViewModel>();

            switch (TMode)
            {
                case TransceiveMode.Upload: // 選択アップロード

                    foreach (var item in FileList)
                    {
                        if (!item.IsCloud && item.IsSelected)
                        {
                            listTargets.Add(item);
                        }
                    }

                    InvokeUpload(this.CloudId, listTargets);
                    
                    break;

                case TransceiveMode.UploadAll:  // 全アップロード

                    foreach (var item in FileList)
                    {
                        if (!item.IsCloud)
                        {
                            listTargets.Add(item);
                        }
                    }

                    InvokeUpload(this.CloudId, listTargets);
                   
                    break;

                case TransceiveMode.Download:   // 選択ダウンロード

                    foreach (var item in FileList)
                    {
                        if (item.IsSelected && item.IsCloud)
                        {
                            listTargets.Add(item);
                        }
                    }

                    InvokeDownload(this.CloudId, listTargets);
                    
                    break;

                case TransceiveMode.DownloadAll:    // 全ダウンロード

                    foreach (var item in FileList)
                    {
                        if (item.IsCloud)
                        {
                            listTargets.Add(item);
                        }
                    }

                    InvokeDownload(this.CloudId, listTargets);
                    
                    break;

                default:
                    break;
            }

            if (PassWord != null)
            {
                PassWord.Dispose();
            }
            PassFile = null;
            PassDrive = null;

            Command.Execute(null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cloudId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private void InvokeUpload(object cloudId, List<ListItemViewModel> list)
        {
            int iRes = 0;
            char[] cKey = null;
            char[] cSec = null;

            // プログレスバー初期化
            PrgBar.Maximum = list.Count;
            PrgBar.Value = 0;
            PrgBar.Visibility = Visibility.Visible;

            // データ送受信モデル初期化
            DataTransceiverModel transceiver = new DataTransceiverModel();
            transceiver.PassWord = this.PassWord;
            transceiver.PassFile = this.PassFile;
            transceiver.PassDrive = this.PassDrive;

            // コンシューマ情報読み込み
            transceiver.LoadConsumerInfo(cloudId, out cKey, out cSec);
            if (cKey == null || cKey.Length == 0)
            {
                MessageBox.Show(Properties.Resources.mesErrFailToLoadId, Properties.Resources.titleErr, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (cSec == null || cSec.Length == 0)
            {
                MessageBox.Show(Properties.Resources.mesErrFailToLoadSec, Properties.Resources.titleErr, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (Encryptor encryptor = new RijndaelEncryptor())
            {
                using (BaseCloudOperator oprCld = new GoogleDriveOperator(cKey, cSec))
                {
                    try
                    {
                        // 認証情報のロード
                        oprCld.LoadAuthInfo();

                        foreach (var item in list)
                        {
                            iRes = transceiver.Upload(encryptor, oprCld, item);

                            if (iRes < 0)
                            {
                                // エラーメッセージ表示

                                break;
                            }

                            // アップロード正常終了時はフラグオン
                            item.IsTransceived = true;

                            // プログレスバー更新
                            PrgBar.Value = PrgBar.Value + 1;
                        }

                        // 認証情報のセーブ
                        oprCld.SaveAuthInfo();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, Properties.Resources.titleErr, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            // 完了メッセージ表示

            // コンシューマ情報保存
            if (!transceiver.SaveConsumerInfo(cloudId, cKey, cSec))
            {
                
            }

            cKey = null;
            cSec = null;
            PrgBar.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cloudId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private void InvokeDownload(object cloudId, List<ListItemViewModel> list)
        {
            int iRes = 0;
            char[] cKey;
            char[] cSec;
            string errBuffer = string.Empty;

            // プログレスバー初期化
            PrgBar.Maximum = list.Count;
            PrgBar.Value = 0;
            PrgBar.Visibility = Visibility.Visible;

            // データ送受信モデル初期化
            DataTransceiverModel transceiver = new DataTransceiverModel();
            transceiver.PassWord = this.PassWord;
            transceiver.PassFile = this.PassFile;
            transceiver.PassDrive = this.PassDrive;

            // コンシューマ情報読み込み
            transceiver.LoadConsumerInfo(cloudId, out cKey, out cSec);
            if (cKey == null || cKey.Length == 0)
            {
                MessageBox.Show(Properties.Resources.mesErrFailToLoadId, Properties.Resources.titleErr, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (cSec == null || cSec.Length == 0)
            {
                MessageBox.Show(Properties.Resources.mesErrFailToLoadId, Properties.Resources.titleErr, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (Decryptor decryptor = new RijndaelDecryptor())
            {
                using (BaseCloudOperator oprCld = new GoogleDriveOperator(cKey, cSec))
                {
                    try
                    {
                        // 認証情報のロード
                        oprCld.LoadAuthInfo();

                        foreach (var item in list)
                        {
                            iRes = transceiver.Download(decryptor, oprCld, item);

                            if (iRes < 0)
                            {

                                break;
                            }

                            // アップロード正常終了時はフラグオン
                            item.IsTransceived = true;

                            // プログレスバー更新
                            PrgBar.Value = PrgBar.Value + 1;
                        }

                        // 認証情報のセーブ
                        oprCld.SaveAuthInfo();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, Properties.Resources.titleErr, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            // 完了メッセージ表示

            // コンシューマ情報保存
            if (!transceiver.SaveConsumerInfo(cloudId, cKey, cSec))
            {
                
            }

            cKey = null;
            cSec = null;
            PrgBar.Visibility = Visibility.Hidden;
        }
    }
}
