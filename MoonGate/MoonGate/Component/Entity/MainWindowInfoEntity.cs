//-----------------------------------------------------------------------
// <summary>メインウィンドウVMクラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-02-13 14:00:00  +9:00 $</date>
// <copyright file="$Name: TargetListEntity.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using mgcloud;
using mgcloud.CloudOperator;
using mgcrypt;
using mgcrypt.Rijndael;
using MoonGate.Component.Message;
using MoonGate.utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace MoonGate.Component.Entity
{
    /// <summary>
    /// MainWindowのVMクラス
    /// </summary>
    public class MainWindowInfoEntity : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        private const string MASTERFILE_PATH = "./mst.xml";

        /// <summary>
        /// 
        /// </summary>
        private const string USERFILE_PATH = "./user/consumer.xml";

        /// <summary>
        /// 
        /// </summary>
        private List<ConsumerInfoEntity> listConsumerInfo;

        /// <summary>
        /// リストのデータソースプロパティ
        /// </summary>
        public ObservableCollection<ListItemEntity> ObsFileList { get; set; }

        /// <summary>
        /// 使用可能クラウドストレージのリストプロパティ
        /// </summary>
        public List<CloudInfoEntity> ListCloudInfo { get; set; }

        ///// <summary>
        ///// 複数ファイルを一つに圧縮するかプロパティ
        ///// </summary>
        //public bool IsBundle { get; set; }

        /// <summary>
        /// ローカルに暗号化ファイルを残すかプロパティ
        /// </summary>
        public bool IsLocal { get; set; }

        /// <summary>
        /// 処理進捗率プロパティ
        /// </summary>
        public int ProgressInfo { get; private set; }

        /// <summary>
        /// ファイル項目追加コマンドプロパティ
        /// </summary>
        public ICommand AddFilesCommand { get; private set; }

        /// <summary>
        /// フォルダ項目追加コマンドプロパティ
        /// </summary>
        public ICommand AddFoldersCommand { get; private set; }

        /// <summary>
        /// クラウド上ファイル追加コマンドプロパティ
        /// </summary>
        public ICommand AddCloudFilesCommand { get; private set; }

        /// <summary>
        /// 項目削除コマンドプロパティ
        /// </summary>
        public ICommand RemoveItemsCommand { get; private set; }

        /// <summary>
        /// 選択アップロードコマンドプロパティ
        /// </summary>
        public ICommand UploadCommand { get; private set; }

        /// <summary>
        /// 全アップロードコマンドプロパティ
        /// </summary>
        public ICommand UploadAllCommand { get; private set; }

        /// <summary>
        /// 選択ダウンロードコマンドプロパティ
        /// </summary>
        public ICommand DownloadCommand { get; private set; }

        /// <summary>
        /// 全ダウンロードコマンドプロパティ
        /// </summary>
        public ICommand DownloadAllCommand { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand CloudSetupCommand { get; private set; }

        /// <summary>
        /// 設定画面表示コマンドプロパティ
        /// </summary>
        public ICommand SettingCommand { get; private set; }

        /// <summary>
        /// 終了コマンドプロパティ
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        /// <summary>
        /// プロパティ変更検知用イベントハンドラ
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowInfoEntity()
        {
            ObsFileList = new ObservableCollection<ListItemEntity>();

            object list = new List<CloudInfoEntity>();
            if (DataSerializer.TryDeserialize(MASTERFILE_PATH, ref list))
            {
                ListCloudInfo = list as List<CloudInfoEntity>;
            }

            SetCommands();
        }


        /// <summary>
        /// コマンドをセットする
        /// </summary>
        private void SetCommands()
        {
            AddFilesCommand = new RelayCommand(
                param => this.AddFiles()
            );

            AddFoldersCommand = new RelayCommand(
                param => this.AddFolders()
            );

            AddCloudFilesCommand = new RelayCommand(
                param => this.GetCloudFileList(param)
            );

            RemoveItemsCommand = new RelayCommand(
                param => this.RemoveItems(),
                param =>
                {
                    return IsSelected();
                }
            );

            UploadCommand = new RelayCommand(
                param => this.UploadSelectedItems(param),
                param =>
                {
                    return IsSelected();
                }
            );

            UploadAllCommand = new RelayCommand(
                param => this.UproadAllItems(param),
                param =>
                {
                    if (ObsFileList.Count == 0)
                    {
                        return false;
                    }

                    return true;
                }
            );

            DownloadCommand = new RelayCommand(
                param => this.DownloadSelectedItems(param),
                param =>
                {
                    return IsSelected();
                }
            );

            DownloadAllCommand = new RelayCommand(
                param => this.DownloadAll(param),
                param =>
                {
                    if (ObsFileList.Count == 0)
                    {
                        return false;
                    }

                    return true;
                }
            );

            CloudSetupCommand = new RelayCommand(
                param => this.CallCldSetup()
            );

            ExitCommand = new RelayCommand(
                param => this.Shutdown()
            );
        }


        /// <summary>
        /// 選択項目の存在有無判定
        /// </summary>
        /// <returns></returns>
        private bool IsSelected()
        {
            foreach (ListItemEntity item in ObsFileList)
            {
                if (item.IsSelected)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// ファイルを追加する
        /// </summary>
        /// <param name="files"></param>
        /// <param name="isDirectory"></param>
        /// <param name="isCloud"></param>
        private void AddFiles()
        {
            ListItemEntity listItem = null;
            SelectFileMessage selectFileMessage = new SelectFileMessage(this);

            selectFileMessage.Title = "Select Files";
            selectFileMessage.CheckPathExists = true;
            selectFileMessage.Multiselect = true;
            selectFileMessage.FileName = string.Empty;
            selectFileMessage.DefaultExt = "*.*";
            selectFileMessage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            Messenger.Instance.Order<SelectFileMessage>(this, selectFileMessage);

            if (selectFileMessage.Result == true)
            {
                foreach (string filePath in selectFileMessage.FileNames)
                {
                    listItem = new ListItemEntity();

                    // 新規項目に情報をセットする
                    listItem.FileName = Path.GetFileName(filePath);
                    listItem.FilePath = filePath;
                    listItem.IsCloud = false;
                    listItem.IsDirectory = false;
                    listItem.IconPath = Imaging.CreateBitmapSourceFromHBitmap(
                        Properties.Resources.glyphicons_036_file.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    ObsFileList.Add(listItem);
                }
            }
        }


        /// <summary>
        /// フォルダを追加する
        /// </summary>
        /// <returns></returns>
        private void AddFolders()
        {
            ListItemEntity listItem = null;
            SelectFolderMessage selectFolderMessage = new SelectFolderMessage(this);

            Messenger.Instance.Order<SelectFolderMessage>(this, selectFolderMessage);

            if (selectFolderMessage.Result == true)
            {
                foreach (string folderPath in selectFolderMessage.FolderNames)
                {
                    listItem = new ListItemEntity();

                    string dirName = Path.GetFileName(folderPath);
                    if (string.IsNullOrEmpty(dirName))
                    {
                        listItem.FileName = folderPath;
                    }
                    else
                    {
                        listItem.FileName = dirName;
                    }
                    listItem.FilePath = folderPath;
                    listItem.IsCloud = false;
                    listItem.IsDirectory = true;
                    listItem.IconPath = Imaging.CreateBitmapSourceFromHBitmap(
                        Properties.Resources.glyphicons_149_folder_new.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                    ObsFileList.Add(listItem);
                }
            }
        }


        /// <summary>
        /// 選択されたアイテムの削除
        /// </summary>
        private void RemoveItems()
        {
            for (int index = 0; index < ObsFileList.Count; index++)
            {
                if (ObsFileList[index].IsSelected)
                {
                    ObsFileList.RemoveAt(index);
                    index--;
                }
            }
        }


        /// <summary>
        /// アップロード
        /// </summary>
        /// <param name="param"></param>
        private void UploadSelectedItems(object param)
        {
            int iRes = -1;
            List<ListItemEntity> listTargets = new List<ListItemEntity>();

            foreach (var item in ObsFileList)
            {
                if (item.IsSelected && !item.IsCloud)
                {
                    listTargets.Add(item);
                }
            }

            iRes = Upload(param, listTargets);
        }


        /// <summary>
        /// 全アップロード
        /// </summary>
        /// <param name="param"></param>
        private void UproadAllItems(object param)
        {
            int iRes = -1;
            List<ListItemEntity> listTargets = new List<ListItemEntity>();

            foreach (var item in ObsFileList)
            {
                if (!item.IsCloud)
                {
                    listTargets.Add(item);
                }
            }

            iRes = Upload(param, listTargets);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private int Upload(object param, List<ListItemEntity> list)
        {
            int iRes = 0;
            char[] cKey = null;
            char[] cSec = null;
            byte[] encryptedData = null;

            LoadConsumerInfo(param, out cKey, out cSec);
            if (cKey == null || cKey.Length == 0)
            {
                return -101;
            }
            if (cSec == null || cSec.Length == 0)
            {
                return -101;
            }

            InputPassMessage inputPassMessage = new InputPassMessage(this);
            Messenger.Instance.Order<InputPassMessage>(this, inputPassMessage);

            if (inputPassMessage.Result == false)
            {
                if (!SaveConsumerInfo(param, cKey, cSec))
                {
                    cKey = null;
                    cSec = null;

                    return -121;
                }

                cKey = null;
                cSec = null;

                return 0;
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
                            // 暗号化
                            encryptedData = null;
                            switch (inputPassMessage.SelectedIndex)
                            {
                                case 0:
                                    iRes = encryptor.Encrypt(item.FilePath, inputPassMessage.PassWord, out encryptedData);
                                    break;
                                case 1:
                                    iRes = encryptor.Encrypt(item.FilePath, inputPassMessage.PassFile, out encryptedData);
                                    break;
                                case 2:
                                    iRes = encryptor.Encrypt(item.FilePath, inputPassMessage.PassDrive, out encryptedData);
                                    break;
                                default:
                                    break;
                            }

                            if (iRes < 0)
                            {
                                break;
                            }

                            // アップロード
                            iRes = oprCld.UploadFile(item.FilePath, encryptedData);
                            if (iRes < 0)
                            {
                                break;
                            }

                            if (!item.IsSelected)
                            {
                                item.IsSelected = true;
                            }
                        }

                        // 認証情報のセーブ
                        oprCld.SaveAuthInfo();
                    }
                    catch(Exception e)
                    {
                        string s = e.StackTrace;
                    }
                }
            }

            inputPassMessage.PassWord.Dispose();
            inputPassMessage.PassFile = null;
            inputPassMessage.PassDrive = null;
            inputPassMessage = null;

            if (!SaveConsumerInfo(param, cKey, cSec))
            {
                cKey = null;
                cSec = null;

                return -121;
            }

            cKey = null;
            cSec = null;

            RemoveItems();

            return iRes;
        }


        /// <summary>
        /// ダウンロード
        /// </summary>
        /// <param name="param"></param>
        private void DownloadSelectedItems(object param)
        {
            int iRes = -1;
            List<ListItemEntity> listTargets = new List<ListItemEntity>();

            foreach (var item in ObsFileList)
            {
                if (item.IsSelected && item.IsCloud)
                {
                    listTargets.Add(item);
                }
            }

            iRes = Download(param, listTargets);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        private void DownloadAll(object param)
        {
            int iRes = -1;
            List<ListItemEntity> listTargets = new List<ListItemEntity>();

            foreach (var item in ObsFileList)
            {
                if (item.IsCloud)
                {
                    listTargets.Add(item);
                }
            }

            iRes = Download(param, listTargets);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private int Download(object param, List<ListItemEntity> list)
        {
            int iRes = 0;
            char[] cKey;
            char[] cSec;
            byte[] downloadData = null;
            
            LoadConsumerInfo(param, out cKey, out cSec);
            if (cKey == null || cKey.Length == 0)
            {
                return -101;
            }
            if (cSec == null || cSec.Length == 0)
            {
                return -101;
            }

            InputPassMessage inputPassMessage = new InputPassMessage(this);
            Messenger.Instance.Order<InputPassMessage>(this, inputPassMessage);

            if (inputPassMessage.Result == false)
            {
                if (!SaveConsumerInfo(param, cKey, cSec))
                {
                    cKey = null;
                    cSec = null;

                    return -121;
                }

                cKey = null;
                cSec = null;

                return 0;
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
                            // ダウンロード
                            downloadData = null;
                            iRes = oprCld.DownloadFile(item.FilePath, out downloadData);
                            if (iRes < 0)
                            {
                                break;
                            }

                            // 復号
                            switch (inputPassMessage.SelectedIndex)
                            {
                                case 0:
                                    iRes = decryptor.Decrypt(item.FileName, inputPassMessage.PassWord, downloadData);
                                    break;
                                case 1:
                                    iRes = decryptor.Decrypt(item.FileName, inputPassMessage.PassFile, downloadData);
                                    break;
                                case 2:
                                    iRes = decryptor.Decrypt(item.FileName, inputPassMessage.PassDrive, downloadData);
                                    break;
                                default:
                                    break;
                            }

                            if (iRes < 0)
                            {
                                break;
                            }

                            if (!item.IsSelected)
                            {
                                item.IsSelected = true;
                            }
                        }

                        // 認証情報のセーブ
                        oprCld.SaveAuthInfo();
                    }
                    catch (Exception e)
                    {
                        string s = e.StackTrace;
                    }
                }
            }

            inputPassMessage.PassWord.Dispose();
            inputPassMessage.PassFile = null;
            inputPassMessage.PassDrive = null;
            inputPassMessage = null;

            if (!SaveConsumerInfo(param, cKey, cSec))
            {
                cKey = null;
                cSec = null;

                return -121;
            }

            cKey = null;
            cSec = null;
            RemoveItems();

            return iRes;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        private void GetCloudFileList(object param)
        {
            char[] cKey;
            char[] cSec;
            ListItemEntity listItem = null;
            HybridDictionary hDic = new HybridDictionary(true);

            LoadConsumerInfo(param, out cKey, out cSec);
            if (cKey == null || cKey.Length == 0)
            {
                return;
            }
            if (cSec == null || cSec.Length == 0)
            {
                return;
            }

            using (BaseCloudOperator oprCld = new GoogleDriveOperator(cKey, cSec))
            {
                try
                {
                    oprCld.LoadAuthInfo();
                    hDic = oprCld.GetFileList();
                    oprCld.SaveAuthInfo();
                }
                catch (Exception e)
                {
                    string s = e.StackTrace;

                    if (!SaveConsumerInfo(param, cKey, cSec))
                    {
                        cKey = null;
                        cSec = null;
                    }

                    return;
                }
            }

            IDictionaryEnumerator hDicEnu = hDic.GetEnumerator();
            while (hDicEnu.MoveNext())
            {
                listItem = new ListItemEntity();

                listItem.FileName = hDicEnu.Key.ToString();
                listItem.FilePath = hDicEnu.Value.ToString();
                listItem.IsCloud = true;
                listItem.IsDirectory = false;
                listItem.IconPath = Imaging.CreateBitmapSourceFromHBitmap(
                        Properties.Resources.glyphicons_232_cloud.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());

                ObsFileList.Add(listItem);
            }

            if (!SaveConsumerInfo(param, cKey, cSec))
            {
                cKey = null;
                cSec = null;

                return;
            }

            cKey = null;
            cSec = null;
        }


        /// <summary>
        /// メッセージ表示
        /// </summary>
        /// <param name="param"></param>
        private void ShowMessageBox(int iMesType, string param)
        {

        }


        /// <summary>
        /// CloudSetup起動
        /// </summary>
        /// <returns></returns>
        private void CallCldSetup()
        {
            Process.Start("ConsumerSignup.exe");
        }


        /// <summary>
        /// アプリケーションの終了
        /// </summary>
        /// <returns></returns>
        private void Shutdown()
        {
            App.Current.Shutdown();
        }


        /// <summary>
        /// コンシューマ情報の読み込み
        /// </summary>
        /// <param name="param"></param>
        /// <param name="cKey"></param>
        /// <param name="cSec"></param>
        private void LoadConsumerInfo(object param, out char[] cKey, out char[] cSec)
        {
            cKey = null;
            cSec = null;
            DataCipher dcp = new DataCipher();

            object objConsumerInfo = new List<ConsumerInfoEntity>();
            if (!DataSerializer.TryDeserialize(USERFILE_PATH, ref objConsumerInfo))
            {
                return;
            }

            listConsumerInfo = objConsumerInfo as List<ConsumerInfoEntity>;
            ConsumerInfoEntity conInfo = new ConsumerInfoEntity();
            foreach (var item in listConsumerInfo)
            {
                if (param.ToString() == item.StorageKey)
                {
                    conInfo = item;
                    listConsumerInfo.Remove(item);
                    break;
                }
            }

            cKey = dcp.DecryptRsa(conInfo.ConsumerKey, param.ToString());
            cSec = dcp.DecryptRsa(conInfo.ConsumerSecret, param.ToString());

            // 秘密鍵の削除
            dcp.DeleteKeys(param.ToString());
        }


        /// <summary>
        /// コンシューマ情報リスト保存
        /// </summary>
        /// <param name="conInfo"></param>
        private bool SaveConsumerInfo(object param, char[] cKey, char[] cSec)
        {
            DataCipher dcp = new DataCipher();

            ConsumerInfoEntity conInfo = new ConsumerInfoEntity();
            conInfo.StorageKey = param.ToString();
            conInfo.ConsumerKey = dcp.EncryptRsa(cKey, param.ToString());
            conInfo.ConsumerSecret = dcp.EncryptRsa(cSec, param.ToString());

            listConsumerInfo.Add(conInfo);

            if (!DataSerializer.TrySerialize(USERFILE_PATH, listConsumerInfo))
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// プロパティ変更イベントのコール
        /// </summary>
        /// <param name="strPropName">プロパティ名</param>
        private void OnPropertyChanged(string strPropName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(strPropName));
            }
        }
    }
}