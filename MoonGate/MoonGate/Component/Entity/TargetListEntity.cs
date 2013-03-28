//-----------------------------------------------------------------------
// <summary>処理対象ファイルリスト保持クラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-02-13 14:00:00  +9:00 $</date>
// <copyright file="$Name: TargetListEntity.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using MoonGate.Component.Message;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace MoonGate.Component.Entity
{
    /// <summary>
    /// ターゲットリストクラス
    /// </summary>
    class TargetListEntity : INotifyPropertyChanged
    {
        /// <summary>
        /// ファイルリストのプロパティ
        /// </summary>
        public ObservableCollection<TargetEntity> ObsFileList { get; set; }

        /// <summary>
        /// トグルボタン状態プロパティ
        /// </summary>
        public bool IsToggled { get; set; }

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
        /// 暗号化コマンドプロパティ
        /// </summary>
        public ICommand EncryptCommand { get; private set; }

        /// <summary>
        /// 復号コマンドプロパティ
        /// </summary>
        public ICommand DecryptCommand { get; private set; }

        /// <summary>
        /// アップデートコマンドプロパティ
        /// </summary>
        public ICommand UploadCommand { get; private set; }

        /// <summary>
        /// ダウンロードコマンドプロパティ
        /// </summary>
        public ICommand DownloadCommand { get; private set; }

        /// <summary>
        /// 設定画面表示コマンドプロパティ
        /// </summary>
        public ICommand SettingCommand { get; private set; }

        /// <summary>
        /// 終了コマンドプロパティ
        /// </summary>
        public ICommand ExitCommand { get; private set; }

        public ICommand Test { get; set; }

        /// <summary>
        /// プロパティ変更検知用イベントハンドラ
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TargetListEntity()
        {
            ObsFileList = new ObservableCollection<TargetEntity>();

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

            Test = new CommandSetter(
                param => this.TestMes(param)
            );
           
            ExitCommand = new CommandSetter(
                param => this.Shutdown()
            );
        }

        private void TestMes(object param)
        {
            ComboItemEntity c = param as ComboItemEntity;
            MessageBox.Show(c.Key);
        }


        /// <summary>
        /// 選択項目の存在有無判定
        /// </summary>
        /// <returns></returns>
        private bool IsSelected()
        {
            foreach (TargetEntity item in ObsFileList)
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
                    TargetEntity listItem = new TargetEntity();

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
                    TargetEntity listItem = new TargetEntity();

                    listItem.FileName = Path.GetFileName(folderPath);
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