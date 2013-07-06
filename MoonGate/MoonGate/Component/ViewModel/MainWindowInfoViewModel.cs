//-----------------------------------------------------------------------
// <summary>メインウィンドウViewModel</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-02-13 14:00:00  +9:00 $</date>
// <copyright file="$Name: TargetListEntity.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using MoonGate.Model;
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

namespace MoonGate.Component.ViewModel
{
    /// <summary>
    /// MainWindowのViewModel
    /// </summary>
    public class MainWindowInfoViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        private const string MASTERFILE_PATH = "./mst.xml";
  
        /// <summary>
        /// リストのデータソースプロパティ
        /// </summary>
        public ObservableCollection<ListItemViewModel> ObsFileList { get; set; }

        /// <summary>
        /// 使用可能クラウドストレージのリストプロパティ
        /// </summary>
        public List<CloudInfoViewModel> ListCloudInfo { get; set; }

        ///// <summary>
        ///// 複数ファイルを一つに圧縮するかプロパティ
        ///// </summary>
        //public bool IsBundle { get; set; }

        ///// <summary>
        ///// ローカルに暗号化ファイルを残すかプロパティ
        ///// </summary>
        //public bool IsLocal { get; set; }

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
        public ICommand RemoveSelectedItemsCommand { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand RemoveTransceivedItemCommand { get; private set; }

        ///// <summary>
        ///// 選択アップロードコマンドプロパティ
        ///// </summary>
        //public ICommand UploadCommand { get; private set; }

        ///// <summary>
        ///// 全アップロードコマンドプロパティ
        ///// </summary>
        //public ICommand UploadAllCommand { get; private set; }

        ///// <summary>
        ///// 選択ダウンロードコマンドプロパティ
        ///// </summary>
        //public ICommand DownloadCommand { get; private set; }

        ///// <summary>
        ///// 全ダウンロードコマンドプロパティ
        ///// </summary>
        //public ICommand DownloadAllCommand { get; private set; }

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
        public MainWindowInfoViewModel()
        {
            ObsFileList = new ObservableCollection<ListItemViewModel>();

            object list = new List<CloudInfoViewModel>();
            if (DataSerializerModel.TryDeserialize(MASTERFILE_PATH, ref list))
            {
                ListCloudInfo = list as List<CloudInfoViewModel>;
            }
            else
            {
                ListCloudInfo = new List<CloudInfoViewModel>();
            }

            SetCommands();
        }


        /// <summary>
        /// コマンドをセットする
        /// </summary>
        private void SetCommands()
        {
            AddFilesCommand = new RelayCommand(
                param => this.AddFile(param)
            );

            AddFoldersCommand = new RelayCommand(
                param => this.AddFolder(param)
            );

            AddCloudFilesCommand = new RelayCommand(
                param => this.AddCloudFiles(param)
            );

            RemoveSelectedItemsCommand = new RelayCommand(
                param => this.RemoveSelectedItems(),
                param =>
                {
                    foreach (ListItemViewModel item in ObsFileList)
                    {
                        if (item.IsSelected)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            );

            RemoveTransceivedItemCommand = new RelayCommand(
                param => this.RemoveTransceivedItems()
            );

            //UploadCommand = new RelayCommand(
            //    param => this.UploadSelectedItems(param),
            //    param =>
            //    {
            //        return IsSelected();
            //    }
            //);

            //UploadAllCommand = new RelayCommand(
            //    param => this.UproadAllItems(param),
            //    param =>
            //    {
            //        if (ObsFileList.Count == 0)
            //        {
            //            return false;
            //        }

            //        return true;
            //    }
            //);

            //DownloadCommand = new RelayCommand(
            //    param => this.DownloadSelectedItems(param),
            //    param =>
            //    {
            //        return IsSelected();
            //    }
            //);

            //DownloadAllCommand = new RelayCommand(
            //    param => this.DownloadAll(param),
            //    param =>
            //    {
            //        if (ObsFileList.Count == 0)
            //        {
            //            return false;
            //        }

            //        return true;
            //    }
            //);

            CloudSetupCommand = new RelayCommand(
                param => this.CallCldSetup()
            );

            ExitCommand = new RelayCommand(
                param => this.Shutdown()
            );
        }


        /// <summary>
        /// ファイルを追加する
        /// </summary>
        /// <param name="files"></param>
        /// <param name="isDirectory"></param>
        /// <param name="isCloud"></param>
        private void AddFile(object param)
        {
            ListItemViewModel listItem = new ListItemViewModel();

            // 新規項目に情報をセットする
            listItem.FileName = Path.GetFileName(param.ToString());
            listItem.FilePath = param.ToString();
            listItem.IsCloud = false;
            listItem.IsDirectory = false;
            listItem.IsTransceived = false;
            listItem.IconPath = Imaging.CreateBitmapSourceFromHBitmap(
                Properties.Resources.glyphicons_036_file.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            ObsFileList.Add(listItem);
        }


        /// <summary>
        /// フォルダを追加する
        /// </summary>
        /// <returns></returns>
        private void AddFolder(object param)
        {
            ListItemViewModel listItem = listItem = new ListItemViewModel();
           
            string dirName = Path.GetFileName(param.ToString());
            if (string.IsNullOrEmpty(dirName))
            {
                listItem.FileName = param.ToString();
            }
            else
            {
                listItem.FileName = dirName;
            }
            listItem.FilePath = param.ToString();
            listItem.IsCloud = false;
            listItem.IsDirectory = true;
            listItem.IsTransceived = false;
            listItem.IconPath = Imaging.CreateBitmapSourceFromHBitmap(
                Properties.Resources.glyphicons_149_folder_new.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            ObsFileList.Add(listItem);
        }


        /// <summary>
        /// 選択されたアイテムの削除
        /// </summary>
        private void RemoveSelectedItems()
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
        /// 
        /// </summary>
        private void RemoveTransceivedItems()
        {
            for (int index = 0; index < ObsFileList.Count; index++)
            {
                if (ObsFileList[index].IsTransceived)
                {
                    ObsFileList.RemoveAt(index);
                    index--;
                }
            }
        }


        ///// <summary>
        ///// 選択アップロード
        ///// </summary>
        ///// <param name="param"></param>
        //private void UploadSelectedItems(object param)
        //{
        //    int iRes = -1;
        //    List<ListItemViewModel> listTargets = new List<ListItemViewModel>();

            
        //    InputPassMessage inputPassMessage = new InputPassMessage(this);
        //    Messenger.Instance.Order<InputPassMessage>(this, inputPassMessage);

        //    if (inputPassMessage.Result == true)
        //    {
        //        foreach (var item in ObsFileList)
        //        {
        //            if (item.IsSelected && !item.IsCloud)
        //            {
        //                item.IsTransceived = false;
        //                listTargets.Add(item);
        //            }
        //        }

        //        DataTransceiverModel transceiver = new DataTransceiverModel();
        //        iRes = transceiver.Upload(param, inputPassMessage, listTargets);

        //        if (iRes < 0)
        //        {

        //        }

        //        inputPassMessage.PassWord.Dispose();
        //        inputPassMessage.PassFile = null;
        //        inputPassMessage.PassDrive = null;
        //        inputPassMessage = null;

        //        // アップロードの完了したアイテムはリストから削除
        //        RemoveTransceivedItems();
        //    }
        //}


        ///// <summary>
        ///// 全アップロード
        ///// </summary>
        ///// <param name="param"></param>
        //private void UproadAllItems(object param)
        //{
        //    int iRes = -1;
        //    List<ListItemViewModel> listTargets = new List<ListItemViewModel>();


        //    InputPassMessage inputPassMessage = new InputPassMessage(this);
        //    Messenger.Instance.Order<InputPassMessage>(this, inputPassMessage);

        //    if (inputPassMessage.Result == true)
        //    {
        //        foreach (var item in ObsFileList)
        //        {
        //            if (!item.IsCloud)
        //            {
        //                item.IsTransceived = false;
        //                listTargets.Add(item);
        //            }
        //        }

        //        DataTransceiverModel transceiver = new DataTransceiverModel();
        //        iRes = transceiver.Upload(param, inputPassMessage, listTargets);

        //        if (iRes < 0)
        //        {

        //        }

        //        inputPassMessage.PassWord.Dispose();
        //        inputPassMessage.PassFile = null;
        //        inputPassMessage.PassDrive = null;
        //        inputPassMessage = null;

        //        // アップロードの完了したアイテムはリストから削除
        //        RemoveTransceivedItems();
        //    }
        //}


        ///// <summary>
        ///// ダウンロード
        ///// </summary>
        ///// <param name="param"></param>
        //private void DownloadSelectedItems(object param)
        //{
        //    int iRes = -1;
        //    List<ListItemViewModel> listTargets = new List<ListItemViewModel>();

        //    InputPassMessage inputPassMessage = new InputPassMessage(this);
        //    Messenger.Instance.Order<InputPassMessage>(this, inputPassMessage);

        //    if (inputPassMessage.Result == true)
        //    {
        //        foreach (var item in ObsFileList)
        //        {
        //            if (item.IsSelected && item.IsCloud)
        //            {
        //                item.IsTransceived = false;
        //                listTargets.Add(item);
        //            }
        //        }

        //        DataTransceiverModel transceiver = new DataTransceiverModel();
        //        iRes = transceiver.Download(param, inputPassMessage, listTargets);

        //        if (iRes < 0)
        //        {

        //        }

        //        inputPassMessage.PassWord.Dispose();
        //        inputPassMessage.PassFile = null;
        //        inputPassMessage.PassDrive = null;
        //        inputPassMessage = null;

        //        // ダウンロードの完了したアイテムはリストから削除
        //        RemoveTransceivedItems();
        //    }
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="param"></param>
        //private void DownloadAll(object param)
        //{
        //    int iRes = -1;
        //    List<ListItemViewModel> listTargets = new List<ListItemViewModel>();

        //    InputPassMessage inputPassMessage = new InputPassMessage(this);
        //    Messenger.Instance.Order<InputPassMessage>(this, inputPassMessage);

        //    if (inputPassMessage.Result == true)
        //    {
        //        foreach (var item in ObsFileList)
        //        {
        //            if (item.IsCloud)
        //            {
        //                item.IsTransceived = false;
        //                listTargets.Add(item);
        //            }
        //        }

        //        DataTransceiverModel transceiver = new DataTransceiverModel();
        //        iRes = transceiver.Download(param, inputPassMessage, listTargets);

        //        if (iRes < 0)
        //        {

        //        }

        //        inputPassMessage.PassWord.Dispose();
        //        inputPassMessage.PassFile = null;
        //        inputPassMessage.PassDrive = null;
        //        inputPassMessage = null;

        //        // ダウンロードの完了したアイテムはリストから削除
        //        RemoveTransceivedItems();
        //    }
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        private void AddCloudFiles(object param)
        {
            ListItemViewModel listItem = null;
            HybridDictionary hDic = null;

            DataTransceiverModel transceiver = new DataTransceiverModel();
            hDic = transceiver.GetCloudFileList(param);

            if (hDic == null)
            {
                return;
            }

            IDictionaryEnumerator hDicEnu = hDic.GetEnumerator();
            while (hDicEnu.MoveNext())
            {
                listItem = new ListItemViewModel();

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