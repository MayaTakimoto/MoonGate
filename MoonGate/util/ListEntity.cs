//-----------------------------------------------------------------------
// <summary>処理対象ファイルリストの保持クラス</summary>
// <author>MayaTakimoto</author> 
// <date>$Date: 2013-02-13 14:00:00  GMT $</date>
// <copyright file="$Name: ListEntity.cs $" > 
//     Copyright(c) 2013 MayaTakimoto All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MoonGate.util
{
    /// <summary>
    /// ファイルリストのエンティティ
    /// </summary>
    public class ListEntity : INotifyPropertyChanged
    {
        /// <summary>
        /// ファイルリスト
        /// </summary>
        private ObservableCollection<string> obsFileList;

        /// <summary>
        /// このクラスのインスタンスを格納する変数
        /// </summary>
        private static ListEntity entList = new ListEntity();

        /// <summary>
        /// ファイルリストのプロパティ
        /// </summary>
        public ObservableCollection<string> ObsFileList
        {
            set
            {
                this.obsFileList = value;
                OnPropertyChanged("ObsFileList");
            }
            get { return this.obsFileList; }
        }

        /// <summary>
        /// このクラスのインスタンスを取得する
        /// </summary>
        public static ListEntity EntList
        {
            get { return entList; }
        }

        /// <summary>
        /// プロパティ変更検知用イベントハンドラ
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ListEntity()
        {
            // ファイルリストの生成
            ObsFileList = new ObservableCollection<string>();
        }


        /// <summary>
        /// プロパティ変更イベントのコール
        /// </summary>
        /// <param name="strPropName">プロパティ名</param>
        protected void OnPropertyChanged(string strPropName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(strPropName));
            }
        }
    }
}
