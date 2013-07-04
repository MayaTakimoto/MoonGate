using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoonGate.Component.ViewModel
{
    /// <summary>
    /// ツリーノードクラス
    /// </summary>
    public class TreeNodeViewModel
    {
        /// <summary>
        /// ディレクトリ情報
        /// </summary>
        private readonly DirectoryInfo dirInfo;

        /// <summary>
        /// 各ノードが有するファイル情報
        /// </summary>
        public FileInfoViewModel FolderInfo { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public bool IsChecked { get; set; }

        /// <summary>
        /// 子ノードのリストのプロパティ
        /// </summary>
        public IEnumerable<TreeNodeViewModel> ListTreeNodes
        {
            get
            {
                try
                {
                    return Directory.EnumerateDirectories(this.dirInfo.FullName).Select(param => new TreeNodeViewModel(param));
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        ///// <summary>
        ///// プロパティ変更検知用イベントハンドラ
        ///// </summary>
        //public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TreeNodeViewModel(string dirPath)
        {  
            dirInfo = new DirectoryInfo(dirPath);

            FolderInfo = new FileInfoViewModel();
            FolderInfo.FileName = dirInfo.Name;
            FolderInfo.FilePath = dirInfo.FullName;
        }


        ///// <summary>
        ///// プロパティ変更イベントのコール
        ///// </summary>
        ///// <param name="strPropName">プロパティ名</param>
        //private void OnPropertyChanged(string strPropName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(strPropName));
        //    }
        //}
    }
}
