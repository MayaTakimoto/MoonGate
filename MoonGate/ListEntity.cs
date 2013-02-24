using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MoonGate.util
{
    public class ListEntity : INotifyPropertyChanged
    {
        /*************************************************
         *  フィールド                                   *
         *************************************************/

        // ファイルリスト
        private ObservableCollection<string> obsFileList;

        // このクラスのインスタンスを格納する変数
        private static ListEntity _listEntity;

        // ファイルリストのプロパティ
        public ObservableCollection<string> ObsFileList
        {
            set 
            { 
                this.obsFileList = value;
                OnPropertyChanged("ObsFileList");
            }
            get { return this.obsFileList; }
        }

        // このクラスのエンティティを取得する
        public static ListEntity listEntity
        {
            get
            {
                // インスタンスが未生成の場合はコンストラクタを呼び出す
                if (_listEntity == null)
                {
                    _listEntity = new ListEntity();
                }

                return _listEntity;
            }
        }

        // プロパティ変更検知用イベントハンドラ
        public event PropertyChangedEventHandler PropertyChanged;

                
        /*************************************************
         *  コンストラクタ                               *
         *************************************************/

        private ListEntity()
        {
            // ファイルリストの生成
            ObsFileList = new ObservableCollection<string>();
        }


        /*************************************************
         *  メソッド                                     *
         *************************************************/

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
