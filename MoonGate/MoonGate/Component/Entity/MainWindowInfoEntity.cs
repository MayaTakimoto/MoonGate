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
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// 複数ファイルを一つに圧縮するかプロパティ
        /// </summary>
        public bool IsBundle { get; set; }

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
            //CloudInfoEntity c = new CloudInfoEntity();
            //c.Key = "CS02";
            //c.Value = "SkyDrive";
            //ListCloudInfo.Add(c);

            SetCommands();
        }


        /// <summary>
        /// コマンドをセットする
        /// </summary>
        private void SetCommands()
        {
            AddFilesCommand = new CommandSetter(
                param => this.AddFiles()
            );

            AddFoldersCommand = new CommandSetter(
                param => this.AddFolders()
            );

            RemoveItemsCommand = new CommandSetter(
                param => this.RemoveItems(),
                param =>
                {
                    return IsSelected();
                }
            );

            UploadCommand = new CommandSetter(
                param => this.Upload(param),
                param =>
                {
                    return IsSelected();
                }
            );

            UploadAllCommand = new CommandSetter(
                param => this.UproadAll(param),
                param =>
                {
                    if (ObsFileList.Count == 0)
                    {
                        return false;
                    }

                    return true;
                }
            );

            CloudSetupCommand = new CommandSetter(
                param => this.CallCldSetup()
            );

            ExitCommand = new CommandSetter(
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
            SelectFileMessage selectFileMessage = new SelectFileMessage(this);

            selectFileMessage.Title = "Select Files";
            selectFileMessage.CheckPathExists = true;
            selectFileMessage.Multiselect = true;
            selectFileMessage.FileName = string.Empty;
            selectFileMessage.DefaultExt = "*.*";
            selectFileMessage.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            Indicator.Instance.Order<SelectFileMessage>(this, selectFileMessage);

            if (selectFileMessage.Result == true)
            {
                foreach (string filePath in selectFileMessage.FileNames)
                {
                    ListItemEntity listItem = new ListItemEntity();

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
            SelectFolderMessage selectFolderMessage = new SelectFolderMessage(this);

            Indicator.Instance.Order<SelectFolderMessage>(this, selectFolderMessage);

            if (selectFolderMessage.Result == true)
            {
                foreach (string folderPath in selectFolderMessage.FolderNames)
                {
                    ListItemEntity listItem = new ListItemEntity();

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
        private void Upload(object param)
        {
            char[] cKey;
            char[] cSec;

            LoadConsumerInfo(param, out cKey, out cSec);
            if (cKey == null || cKey.Length == 0)
            {
                return;
            }
            if (cSec == null || cSec.Length == 0)
            {
                return;
            }

            InputPassMessage inputPassMessage = new InputPassMessage(this);
            Indicator.Instance.Order<InputPassMessage>(this, inputPassMessage);

            if (inputPassMessage.Result == false)
            {
                return;
            }

            using (Encryptor encryptor = new RijndaelEncryptor())
            {
                using (BaseCloudOperator oprCld = new GoogleDriveOperator(cKey, cSec))
                {
                    int iRes = -1;

                    // 認証情報のロード
                    oprCld.LoadAuthInfo();

                    foreach (var item in ObsFileList)
                    {
                        if (!item.IsSelected)
                        {
                            continue;
                        }
                        if (item.IsCloud)
                        {
                            continue;
                        }

                        // 暗号化対象をセット
                        encryptor.InitEncrypt(item.FilePath);

                        // 暗号化
                        byte[] encryptedData = null;
                        switch (inputPassMessage.SelectedIndex)
                        {
                            case 0:
                                iRes = encryptor.Encrypt(inputPassMessage.PassWord, out encryptedData);
                                break;
                            case 1:
                                iRes = encryptor.Encrypt(inputPassMessage.PassFile, out encryptedData);
                                break;
                            case 2:
                                iRes = encryptor.Encrypt(inputPassMessage.PassDrive, out encryptedData);
                                break;
                            default:
                                break;
                        }

                        if (iRes < 0)
                        {
                            return;
                        }

                        // アップロード
                        iRes = oprCld.UploadFile(item.FilePath, encryptedData);
                        if (iRes < 0)
                        {
                            return;
                        }
                    }

                    // 認証情報のセーブ
                    oprCld.SaveAuthInfo();
                }
            }

            inputPassMessage.PassWord.Dispose();
            inputPassMessage.PassFile = null;
            inputPassMessage.PassDrive = null;
            inputPassMessage = null;
            
            if (!SaveConsumerInfo(param, cKey, cSec))
            {

            }

            cKey = null;
            cSec = null;
            RemoveItems();
        }


        /// <summary>
        /// 全アップロード
        /// </summary>
        /// <param name="param"></param>
        private void UproadAll(object param)
        {
            char[] cKey;
            char[] cSec;

            LoadConsumerInfo(param, out cKey, out cSec);
            if (cKey == null || cKey.Length == 0)
            {
                return;
            }
            if (cSec == null || cSec.Length == 0)
            {
                return;
            }

            InputPassMessage inputPassMessage = new InputPassMessage(this);
            Indicator.Instance.Order<InputPassMessage>(this, inputPassMessage);

            if (inputPassMessage.Result == false)
            {
                return;
            }

            using (Encryptor encryptor = new RijndaelEncryptor())
            {
                using (BaseCloudOperator oprCld = new GoogleDriveOperator(cKey, cSec))
                {
                    int iRes = -1;

                    // 認証情報のロード
                    oprCld.LoadAuthInfo();

                    foreach (var item in ObsFileList)
                    {
                        if (item.IsCloud)
                        {
                            continue;
                        }

                        // 暗号化対象をセット
                        encryptor.InitEncrypt(item.FilePath);

                        // 暗号化
                        byte[] encryptedData = null;
                        switch (inputPassMessage.SelectedIndex)
                        {
                            case 0:
                                iRes = encryptor.Encrypt(inputPassMessage.PassWord, out encryptedData);
                                break;
                            case 1:
                                iRes = encryptor.Encrypt(inputPassMessage.PassFile, out encryptedData);
                                break;
                            case 2:
                                iRes = encryptor.Encrypt(inputPassMessage.PassDrive, out encryptedData);
                                break;
                            default:
                                break;
                        }

                        if (iRes < 0)
                        {
                            return;
                        }

                        // アップロード
                        iRes = oprCld.UploadFile(item.FilePath, encryptedData);
                        if (iRes < 0)
                        {
                            return;
                        }
                    }

                    // 認証情報のセーブ
                    oprCld.SaveAuthInfo();
                }
            }

            inputPassMessage.PassWord.Dispose();
            inputPassMessage.PassFile = null;
            inputPassMessage.PassDrive = null;
            inputPassMessage = null;

            if (!SaveConsumerInfo(param, cKey, cSec))
            {

            }

            cKey = null;
            cSec = null;
            RemoveItems();
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