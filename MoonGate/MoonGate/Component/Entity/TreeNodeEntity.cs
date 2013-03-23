using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MoonGate.Component.Entity
{
    /// <summary>
    /// ツリーノードクラス
    /// </summary>
    class TreeNodeEntity
    {
        /// <summary>
        /// ディレクトリ情報
        /// </summary>
        private readonly DirectoryInfo dirInfo;

        /// <summary>
        /// 各ノードが有するファイル情報
        /// </summary>
        public FileInfoEntity FolderInfo { get; set; }

        /// <summary>
        /// 子ノードのリストのプロパティ
        /// </summary>
        public IEnumerable<TreeNodeEntity> ListTreeNodes
        {
            get
            {
                try
                {
                    return Directory.EnumerateDirectories(this.dirInfo.FullName).Select(param => new TreeNodeEntity(param));
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
        public TreeNodeEntity(string dirPath)
        {  
            dirInfo = new DirectoryInfo(dirPath);

            FolderInfo = new FileInfoEntity();
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
